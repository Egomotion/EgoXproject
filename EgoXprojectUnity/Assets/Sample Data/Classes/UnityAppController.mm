#import "UnityAppController.h"
#import "iPhone_Sensors.h"

#import <CoreGraphics/CoreGraphics.h>
#import <QuartzCore/QuartzCore.h>
#import <QuartzCore/CADisplayLink.h>
#import <UIKit/UIKit.h>
#import <Availability.h>

#import <OpenGLES/EAGL.h>
#import <OpenGLES/EAGLDrawable.h>
#import <OpenGLES/ES2/gl.h>
#import <OpenGLES/ES2/glext.h>

#include <mach/mach_time.h>

// MSAA_DEFAULT_SAMPLE_COUNT was moved to iPhone_GlesSupport.h
// ENABLE_INTERNAL_PROFILER and related defines were moved to iPhone_Profiler.h
// kFPS define for removed: you can use Application.targetFrameRate (30 fps by default)
// DisplayLink is the only run loop mode now - all others were removed

#include "CrashReporter.h"
#include "iPhone_OrientationSupport.h"
#include "iPhone_Profiler.h"
#include "iPhone_View.h"

#include "UI/Keyboard.h"
#include "UI/UnityView.h"
#include "Unity/DisplayManager.h"
#include "Unity/EAGLContextHelper.h"
#include "Unity/GlesHelper.h"
#include "PluginBase/AppDelegateListener.h"



// Time to process events in seconds.
#define kInputProcessingTime                    0.001

// --- Unity ------------------------------------------------------------------
//

bool	_ios42orNewer			= false;
bool	_ios43orNewer			= false;
bool	_ios50orNewer			= false;
bool	_ios60orNewer			= false;
bool	_ios70orNewer			= false;

bool	_supportsDiscard		= false;
bool	_supportsMSAA			= false;
bool	_supportsPackedStencil	= false;

bool	_glesContextCreated		= false;
bool	_unityLevelReady		= false;
bool	_skipPresent			= false;

static DisplayConnection* _mainDisplay = 0;

void UnityInitJoysticks();

// --- OpenGLES --------------------------------------------------------------------
//

CADisplayLink*	_displayLink;

// This is set to true when applicationWillResignActive gets called. It is here
// to prevent calling SetPause(false) from applicationDidBecomeActive without
// previous call to applicationWillResignActive
BOOL			_didResignActive = NO;


void PresentMainView()
{
	if(_skipPresent || _didResignActive)
		return;
	[_mainDisplay present];
}


extern "C" int CreateContext_UnityCallback(UIWindow** window, int* screenWidth, int* screenHeight,  int* openglesVersion)
{
	extern void QueryTargetResolution(int* targetW, int* targetH);

	int resW=0, resH=0;
	QueryTargetResolution(&resW, &resH);

	[_mainDisplay createContext:nil];

	*window			= UnityGetMainWindow();
	*screenWidth	= resW;
	*screenHeight	= resH;
	*openglesVersion= _mainDisplay->surface.context.API;

	[EAGLContext setCurrentContext:_mainDisplay->surface.context];

	return true;
}

extern "C" int GfxInited_UnityCallback(int screenWidth, int screenHeight)
{
	InitGLES();
	_glesContextCreated = true;

	[GetAppController() shouldAttachRenderDelegate];
	[GetAppController().renderDelegate mainDisplayInited:&_mainDisplay->surface];
	[GetAppController().unityView recreateGLESSurface];

	_mainDisplay->surface.allowScreenshot = true;

	SetupUnityDefaultFBO(&_mainDisplay->surface);
	glViewport(0, 0, _mainDisplay->surface.targetW, _mainDisplay->surface.targetH);

	return true;
}

extern "C" void PresentContext_UnityCallback(struct UnityFrameStats const* unityFrameStats)
{
	Profiler_FrameEnd();
	PresentMainView();
	Profiler_FrameUpdate(unityFrameStats);
}


extern "C" void NotifyFramerateChange(int targetFPS)
{
	if( targetFPS <= 0 )
		targetFPS = 60;

	int animationFrameInterval = (60.0 / (targetFPS));
	if (animationFrameInterval < 1)
		animationFrameInterval = 1;

	[_displayLink setFrameInterval:animationFrameInterval];
}

bool LogToNSLogHandler(LogType logType, const char* log, va_list list)
{
	NSLogv([NSString stringWithUTF8String:log], list);
	return true;
}

void UnityInitTrampoline()
{
#if ENABLE_CRASH_REPORT_SUBMISSION
	SubmitCrashReportsAsync();
#endif
#if ENABLE_CUSTOM_CRASH_REPORTER
	InitCrashReporter();
#endif

	_ios42orNewer = [[[UIDevice currentDevice] systemVersion] compare: @"4.2" options: NSNumericSearch] != NSOrderedAscending;
	_ios43orNewer = [[[UIDevice currentDevice] systemVersion] compare: @"4.3" options: NSNumericSearch] != NSOrderedAscending;
	_ios50orNewer = [[[UIDevice currentDevice] systemVersion] compare: @"5.0" options: NSNumericSearch] != NSOrderedAscending;
	_ios60orNewer = [[[UIDevice currentDevice] systemVersion] compare: @"6.0" options: NSNumericSearch] != NSOrderedAscending;
	_ios70orNewer = [[[UIDevice currentDevice] systemVersion] compare: @"7.0" options: NSNumericSearch] != NSOrderedAscending;

	// Try writing to console and if it fails switch to NSLog logging
	fprintf(stdout, "\n");
	if (ftell(stdout) < 0)
		SetLogEntryHandler(LogToNSLogHandler);

	UnityInitJoysticks();
}


// --- AppController --------------------------------------------------------------------
//


@implementation UnityAppController

@synthesize unityView			= _unityView;
@synthesize rootView			= _rootView;
@synthesize rootViewController	= _rootController;
@synthesize renderDelegate		= _renderDelegate;


- (void)shouldAttachRenderDelegate	{}
- (void)preStartUnity				{}

- (void)startUnity:(UIApplication*)application
{
	char const* appPath = [[[NSBundle mainBundle] bundlePath]UTF8String];
	UnityInitApplication(appPath);

	[[DisplayManager Instance] updateDisplayListInUnity];

	OnUnityInited();

	UnityLoadApplication();
	Profiler_InitProfiler();

	_unityLevelReady = true;
	OnUnityReady();

	// Frame interval defines how many display frames must pass between each time the display link fires.
	int animationFrameInterval = 60.0 / (float)UnityGetTargetFPS();
	assert(animationFrameInterval >= 1);

	_displayLink = [CADisplayLink displayLinkWithTarget:self selector:@selector(repaintDisplayLink)];
	[_displayLink setFrameInterval:animationFrameInterval];
	[_displayLink addToRunLoop:[NSRunLoop currentRunLoop] forMode:NSRunLoopCommonModes];
}

- (void)repaint
{
	EAGLContextSetCurrentAutoRestore autorestore(_mainDisplay->surface.context);
	SetupUnityDefaultFBO(&_mainDisplay->surface);

	CheckOrientationRequest();
	[GetAppController().unityView recreateGLESSurfaceIfNeeded];

	Profiler_FrameStart();
	UnityInputProcess();
	UnityPlayerLoop();
}

- (void)repaintDisplayLink
{
	if(_didResignActive)
		return;

	[self repaint];

	[[DisplayManager Instance] presentAllButMain];
	SetupUnityDefaultFBO(&_mainDisplay->surface);
}

- (UnityView*)initUnityViewImpl
{
	return [[UnityView alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
}
- (UnityView*)initUnityView
{
	_unityView = [self initUnityViewImpl];
	_unityView.contentScaleFactor = [UIScreen mainScreen].scale;
	_unityView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;

	return _unityView;
}

- (void)createViewHierarchyImpl
{
	_rootView = _unityView;
	_rootController = [[UnityDefaultViewController alloc] init];
}

- (void)createViewHierarchy
{
	[self createViewHierarchyImpl];
	NSAssert(_rootView != nil, @"createViewHierarchyImpl must assign _rootView");
	NSAssert(_rootController != nil, @"createViewHierarchyImpl must assign _rootController");

	_rootView.contentScaleFactor = [UIScreen mainScreen].scale;
	_rootView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;

	_rootController.wantsFullScreenLayout = TRUE;
	_rootController.view = _rootView;
	if([_rootController isKindOfClass: [UnityViewControllerBase class]])
		[(UnityViewControllerBase*)_rootController assignUnityView:_unityView];
}

- (void)showGameUI:(UIWindow*)window
{
	[window addSubview: _rootView];
	window.rootViewController = _rootController;
	[window bringSubviewToFront: _rootView];
}

- (void)onForcedOrientation:(ScreenOrientation)orient
{
	[_unityView willRotateTo:orient];
	OrientView(_rootView, orient);
	[_rootView layoutSubviews];
	[_unityView didRotate];
}

- (NSUInteger)application:(UIApplication *)application supportedInterfaceOrientationsForWindow:(UIWindow *)window
{
	// UIInterfaceOrientationMaskAll
	// it is the safest way of doing it:
	// - GameCenter and some other services might have portrait-only variant
	//     and will throw exception if portrait is not supported here
	// - When you change allowed orientations if you end up forbidding current one
	//     exception will be thrown
	// Anyway this is intersected with values provided from UIViewController, so we are good
	return   (1 << UIInterfaceOrientationPortrait) | (1 << UIInterfaceOrientationPortraitUpsideDown)
		   | (1 << UIInterfaceOrientationLandscapeRight) | (1 << UIInterfaceOrientationLandscapeLeft);
}

- (void)application:(UIApplication*)application didReceiveLocalNotification:(UILocalNotification*)notification
{
	AppController_SendNotificationWithArg(kUnityDidReceiveLocalNotification, notification);
	UnitySendLocalNotification(notification);
}

- (void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo
{
	AppController_SendNotificationWithArg(kUnityDidReceiveRemoteNotification, userInfo);
	UnitySendRemoteNotification(userInfo);
}

- (void)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken
{
	AppController_SendNotificationWithArg(kUnityDidRegisterForRemoteNotificationsWithDeviceToken, deviceToken);
	UnitySendDeviceToken(deviceToken);
}

- (void)application:(UIApplication*)application didFailToRegisterForRemoteNotificationsWithError:(NSError*)error
{
	AppController_SendNotificationWithArg(kUnityDidFailToRegisterForRemoteNotificationsWithError, error);
	UnitySendRemoteNotificationError(error);
}

- (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation
{
	NSMutableArray* keys	= [NSMutableArray arrayWithCapacity:3];
	NSMutableArray* values	= [NSMutableArray arrayWithCapacity:3];

	#define ADD_ITEM(item)	do{ if(item) {[keys addObject:@#item]; [values addObject:item];} }while(0)

	ADD_ITEM(url);
	ADD_ITEM(sourceApplication);
	ADD_ITEM(annotation);

	#undef ADD_ITEM

	NSDictionary* notifData = [NSDictionary dictionaryWithObjects:values forKeys:keys];
	AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);
	return YES;
}

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
	printf_console("-> applicationDidFinishLaunching()\n");
	// get local notification
	if (&UIApplicationLaunchOptionsLocalNotificationKey != nil)
	{
		UILocalNotification *notification = [launchOptions objectForKey:UIApplicationLaunchOptionsLocalNotificationKey];
		if (notification)
			UnitySendLocalNotification(notification);
	}

	// get remote notification
	if (&UIApplicationLaunchOptionsRemoteNotificationKey != nil)
	{
		NSDictionary *notification = [launchOptions objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey];
		if (notification)
			UnitySendRemoteNotification(notification);
	}

	if ([UIDevice currentDevice].generatesDeviceOrientationNotifications == NO)
		[[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];

	[DisplayManager Initialize];
	_mainDisplay = [[DisplayManager Instance] mainDisplay];
	[_mainDisplay createView:YES showRightAway:NO];

	[KeyboardDelegate Initialize];
	CreateViewHierarchy();

	[self preStartUnity];
	[self performSelector:@selector(startUnity:) withObject:application afterDelay:0];

	NSLog(@"Modified Unity App Controller");
	return NO;
}

- (void)applicationDidEnterBackground:(UIApplication *)application
{
	printf_console("-> applicationDidEnterBackground()\n");
}

- (void)applicationWillEnterForeground:(UIApplication *)application
{
	printf_console("-> applicationWillEnterForeground()\n");

	// if we were showing video before going to background - the view size may be changed while we are in background
	[GetAppController().unityView recreateGLESSurfaceIfNeeded];
}

- (void)applicationDidBecomeActive:(UIApplication*)application
{
	printf_console("-> applicationDidBecomeActive()\n");
	if (_didResignActive)
		UnityPause(false);

	_didResignActive = NO;
}

- (void)applicationWillResignActive:(UIApplication*)application
{
	printf_console("-> applicationWillResignActive()\n");
	UnityPause(true);

	extern void UnityStopVideoIfPlaying();
	UnityStopVideoIfPlaying();

	_didResignActive = YES;
}

- (void)applicationDidReceiveMemoryWarning:(UIApplication*)application
{
	printf_console("WARNING -> applicationDidReceiveMemoryWarning()\n");
}

- (void)applicationWillTerminate:(UIApplication*)application
{
	printf_console("-> applicationWillTerminate()\n");

	Profiler_UninitProfiler();
	UnityCleanup();
}

- (void)dealloc
{
	extern void SensorsCleanup();
	SensorsCleanup();

	extern void ReleaseViewHierarchy();
	ReleaseViewHierarchy();

	[super dealloc];
}
@end


void AppController_RenderPluginMethod(SEL method)
{
	id delegate = GetAppController().renderDelegate;
	if([delegate respondsToSelector:method])
		[delegate performSelector:method];
}
void AppController_RenderPluginMethodWithArg(SEL method, id arg)
{
	id delegate = GetAppController().renderDelegate;
	if([delegate respondsToSelector:method])
		[delegate performSelector:method withObject:arg];
}

void AppController_SendNotification(NSString* name)
{
	[[NSNotificationCenter defaultCenter] postNotificationName:name object:GetAppController()];
}
void AppController_SendNotificationWithArg(NSString* name, id arg)
{
	[[NSNotificationCenter defaultCenter] postNotificationName:name object:GetAppController() userInfo:arg];
}

