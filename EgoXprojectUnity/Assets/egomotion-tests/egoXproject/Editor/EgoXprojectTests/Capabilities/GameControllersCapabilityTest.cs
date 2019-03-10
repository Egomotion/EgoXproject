using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class GameControllersCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void NoneTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.GameControllers, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("GameControllersTVOS.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("GameControllersNoneTVOS.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void AllTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.GameControllers, true);
            var capability = cf.Capabilities.Capability(SystemCapability.GameControllers) as GameControllersCapability;
            capability.GameControllers = new GameControllersCapability.GameControllerType[] { GameControllersCapability.GameControllerType.ExtendedGamepad, GameControllersCapability.GameControllerType.MicroGamepad };
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("GameControllersTVOS.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("GameControllersAllTVOS.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void ExtendedTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.GameControllers, true);
            var capability = cf.Capabilities.Capability(SystemCapability.GameControllers) as GameControllersCapability;
            capability.GameControllers = new GameControllersCapability.GameControllerType[] { GameControllersCapability.GameControllerType.ExtendedGamepad };
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("GameControllersTVOS.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("GameControllersExtendedTVOS.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void MicroTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.GameControllers, true);
            var capability = cf.Capabilities.Capability(SystemCapability.GameControllers) as GameControllersCapability;
            capability.GameControllers = new GameControllersCapability.GameControllerType[] { GameControllersCapability.GameControllerType.MicroGamepad };
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("GameControllersTVOS.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("GameControllersMicroTVOS.plist", TestInfoPlistFilePath);
        }
    }
}
