using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class WirlessAccessoryConfigurationCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void WirlessAccessoryConfiguration()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.WirelessAccessoryConfiguration, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("WirelessAccessoryConfiguration.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("WirelessAccessoryConfiguration.entitlements", TestEntitlementsFilePath);
        }
    }
}
