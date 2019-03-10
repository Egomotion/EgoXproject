using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class KeychainSharingCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void Empty()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.KeychainSharing, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("KeychainSharing.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("KeychainSharingEmpty.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void Entry()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.KeychainSharing, true);
            var capability = cf.Capabilities.Capability(SystemCapability.KeychainSharing) as KeychainSharingCapability;
            capability.KeychainGroups.Add("uk.co.egomotion.egoxproject.sampleapp");
            capability.KeychainGroups.Add("uk.co.egomotion.otherapp");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("KeychainSharing.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("KeychainSharingEntry.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void EmptyTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.KeychainSharing, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("KeychainSharing.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("KeychainSharingEmpty.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void EntryTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.KeychainSharing, true);
            var capability = cf.Capabilities.Capability(SystemCapability.KeychainSharing) as KeychainSharingCapability;
            capability.KeychainGroups.Add("uk.co.egomotion.egoxproject.sampleapp");
            capability.KeychainGroups.Add("uk.co.egomotion.otherapp");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("KeychainSharing.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("KeychainSharingEntry.entitlements", TestEntitlementsFilePath);
        }

    }
}
