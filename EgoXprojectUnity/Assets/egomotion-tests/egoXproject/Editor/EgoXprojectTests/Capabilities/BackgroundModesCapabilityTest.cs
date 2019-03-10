using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class BackgroundModesCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void Empty()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesEmpty.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void ActsAsBTLEAcc()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.ActsAsBTLEAcc = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesActsAsBTLEAcc.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void AudioAirplayPIP()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.AudioAirplayPIP = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesAudioAirplayPIP.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void BackgroundFetch()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.BackgroundFetch = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesBackgroundFetch.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void ExternalAccComms()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.ExternalAccComms = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesExternalAccComms.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void LocationUpdates()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.LocationUpdates = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesLocationUpdates.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void NewsstandDownloads()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.NewsstandDownloads = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesNewsstandDownloads.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void RemoteNotifications()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.RemoteNotifications = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesRemoteNotifications.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void UsesBTLEAcc()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.UsesBTLEAcc = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesUsesBTLEAcc.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void VOIP()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.VOIP = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesVOIP.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void All()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.ActsAsBTLEAcc = true;
            capability.AudioAirplayPIP = true;
            capability.BackgroundFetch = true;
            capability.ExternalAccComms = true;
            capability.LocationUpdates = true;
            capability.NewsstandDownloads = true;
            capability.RemoteNotifications = true;
            capability.UsesBTLEAcc = true;
            capability.VOIP = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModes.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesAll.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void EmptyTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModesTVOS.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesEmpty.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void AllTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.AudioAirplayPIP = true;
            capability.BackgroundFetch = true;
            capability.ExternalAccComms = true;
            capability.RemoteNotifications = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModesTVOS.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesAllTVOS.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void RemoteNotificationsTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.RemoteNotifications = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModesTVOS.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesRemoteNotifications.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void AudioAirplayPIPTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.AudioAirplayPIP = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModesTVOS.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesAudioAirplayPIP.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void BackgroundFetchTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.BackgroundFetch = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModesTVOS.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesBackgroundFetch.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void ExternalAccCommsTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.BackgroundModes, true);
            var capability = cf.Capabilities.Capability(SystemCapability.BackgroundModes) as BackgroundModesCapability;
            capability.ExternalAccComms = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("BackgroundModesTVOS.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("BackgroundModesExternalAccComms.plist", TestInfoPlistFilePath);
        }
    }
}
