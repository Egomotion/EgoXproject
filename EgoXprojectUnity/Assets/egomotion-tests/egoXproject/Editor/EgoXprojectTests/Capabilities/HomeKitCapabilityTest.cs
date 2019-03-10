using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class HomeKitCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void HomeKit()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.HomeKit, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("HomeKit.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("HomeKit.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void HomeKitTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.HomeKit, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("HomeKit.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("HomeKit.entitlements", TestEntitlementsFilePath);
        }

    }
}
