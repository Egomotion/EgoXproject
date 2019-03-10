using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class DataProtectionCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void DataProtection()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.DataProtection, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("DataProtection.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("DataProtection.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void DataProtectionTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.DataProtection, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("DataProtection.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("DataProtection.entitlements", TestEntitlementsFilePath);
        }
        }
}
