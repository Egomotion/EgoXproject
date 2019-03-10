using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class GameCenterCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void GameCenter()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.GameCenter, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("GameCenter.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("GameCenter.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void GameCenterTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.GameCenter, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("GameCenter.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("GameCenter.plist", TestInfoPlistFilePath);
        }
    }
}
