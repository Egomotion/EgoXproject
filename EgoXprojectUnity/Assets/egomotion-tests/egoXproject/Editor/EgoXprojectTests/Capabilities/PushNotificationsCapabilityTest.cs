using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class PushNotificationsCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void PushNotifications()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.PushNotifications, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("PushNotifications.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("PushNotifications.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void PushNotificationsTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.PushNotifications, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("PushNotifications.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("PushNotifications.entitlements", TestEntitlementsFilePath);
        }

    }
}
