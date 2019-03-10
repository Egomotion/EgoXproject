using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class HotspotConfigurationCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void HotspotConfiguration()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.HotspotConfiguration, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("HotspotConfiguration.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("HotspotConfiguration.entitlements", TestEntitlementsFilePath);
        }
    }
}
