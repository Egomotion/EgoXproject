// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEditor;
using UnityEngine;

using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class XcodeSDKFinder
    {
        public const string DEFAULT_XCODE_PATH = "/Applications/Xcode.app";
        public const string DEFAULT_IOS_SDK_SUBPATH = "Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs";
        public const string DEFAULT_TVOS_SDK_SUBPATH = "Contents/Developer/Platforms/AppleTVOS.platform/Developer/SDKs";
        public const string DEFAULT_FRAMEWORKS_SUBPATH = "System/Library/Frameworks";
        public const string DEFUALT_LIB_SUBPATH = "usr/lib";

        const string DEFAULT_IOS_SDK_NAME = "iPhoneOS.sdk";
        const string DEFAULT_TVOS_SDK_NAME = "AppleTVOS.sdk";

        Dictionary<string, string> _frameworks = new Dictionary<string, string>();

        string _sdkPath;

        BuildPlatform _platform;

        public XcodeSDKFinder(BuildPlatform platform)
        {
            IsFound = false;
            _platform = platform;

            if (!XcodeFinder.IsFound)
            {
                XcodeFinder.FindXcode();
            }

            if (!XcodeFinder.IsFound)
            {
                return;
            }

            switch (_platform) {
            case BuildPlatform.iOS:
                string iosSdkPath = Path.Combine (XcodeFinder.XcodeLocation, DEFAULT_IOS_SDK_SUBPATH);
                IsFound = FindSDK (iosSdkPath, DEFAULT_IOS_SDK_NAME);
                break;
            case BuildPlatform.tvOS:
                string tvosSdkPath = Path.Combine (XcodeFinder.XcodeLocation, DEFAULT_TVOS_SDK_SUBPATH);
                IsFound = FindSDK (tvosSdkPath, DEFAULT_TVOS_SDK_NAME);
                break;
            }
        }

        public BuildPlatform Platform {
            get {
                return _platform;
            }
        }

        public string[] FrameworkNames
        {
            get {
                List<string> list = _frameworks.Keys.ToList ();
                list.Sort ();
                return list.ToArray ();
            }
        }

        public string FrameworkPath(string frameworkName)
        {
            string path = "";
            _frameworks.TryGetValue(frameworkName, out path);
            return path;
        }

        public bool IsFound { get; private set; }

        bool FindSDK (string sdkPath, string defaultSDKName)
        {
            var path = Path.Combine (sdkPath, defaultSDKName);

            if (Directory.Exists (path)) {
                if (FindFrameworksAndLibraries (path)) {
                    _sdkPath = path;
                    return true;
                }
            }

            var subDirs = Directory.GetDirectories (sdkPath);

            foreach (var dir in subDirs) {
                if (FindFrameworksAndLibraries (dir)) {
                    _sdkPath = dir;
                    return true;
                }
            }

            return false;
        }

        bool FindFrameworksAndLibraries(string pathToDK)
        {
            var frameworkPath = Path.Combine(pathToDK, DEFAULT_FRAMEWORKS_SUBPATH);
            var libPath = Path.Combine(pathToDK, DEFUALT_LIB_SUBPATH);

            if (!Directory.Exists(frameworkPath))
            {
                return false;
            }

            Dictionary<string, string> frameworks = new Dictionary<string, string>();
            var frameworkPaths = Directory.GetDirectories(frameworkPath, "*.framework", SearchOption.TopDirectoryOnly);

            if (frameworkPaths == null || frameworkPaths.Length <= 0)
            {
                return false;
            }

            foreach (var p in frameworkPaths)
            {
                var key = Path.GetFileName(p);
                frameworks.Add(key, p);
            }

            //only add static libs if they exist (ios yes, tvos no)
            if (Directory.Exists (libPath)) 
            {
                int libPathLength = libPath.Length + 1;
                var dynLibPaths = Directory.GetFiles (libPath, "*.dylib", SearchOption.AllDirectories);

                if (dynLibPaths.Length > 0) {
                    foreach (var p in dynLibPaths) {
                        var key = p.Remove (0, libPathLength);
                        frameworks.Add (key, p);
                    }
                }

                var tbdPaths = Directory.GetFiles (libPath, "*.tbd", SearchOption.AllDirectories);

                if (tbdPaths.Length > 0) {
                    foreach (var p in tbdPaths) {
                        var key = p.Remove (0, libPathLength);
                        frameworks.Add (key, p);
                    }
                }
            }

            _frameworks = frameworks;

            return frameworks.Count > 0;
        }
    }
}
