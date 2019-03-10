using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class HealthKitCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void HealthKit()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.HealthKit, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("HealthKit.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("HealthKit.entitlements", TestEntitlementsFilePath);
            CompareInfoPlistFiles("HealthKit.plist", TestInfoPlistFilePath);
        }
    }
}
