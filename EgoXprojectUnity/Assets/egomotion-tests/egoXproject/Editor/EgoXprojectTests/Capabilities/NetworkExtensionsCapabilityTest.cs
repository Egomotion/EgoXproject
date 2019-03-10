using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class NetworkExtensionsCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void NoneSelected()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.NetworkExtensions, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("NetworkExtensions.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("NetworkExtensionsNone.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void AllSelected()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.NetworkExtensions, true);
            var capability = cf.Capabilities.Capability(SystemCapability.NetworkExtensions) as NetworkExtensionsCapability;
            capability.AppProxy = true;
            capability.ContentFilter = true;
            capability.PacketTunnel = true;
            capability.DNSProxy = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("NetworkExtensions.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("NetworkExtensionsAll.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void AppProxy()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.NetworkExtensions, true);
            var capability = cf.Capabilities.Capability(SystemCapability.NetworkExtensions) as NetworkExtensionsCapability;
            capability.AppProxy = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("NetworkExtensions.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("NetworkExtensionsAppProxy.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void ContentFilter()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.NetworkExtensions, true);
            var capability = cf.Capabilities.Capability(SystemCapability.NetworkExtensions) as NetworkExtensionsCapability;
            capability.ContentFilter = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("NetworkExtensions.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("NetworkExtensionsContentFilter.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void PacketTunnel()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.NetworkExtensions, true);
            var capability = cf.Capabilities.Capability(SystemCapability.NetworkExtensions) as NetworkExtensionsCapability;
            capability.PacketTunnel = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("NetworkExtensions.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("NetworkExtensionsPacketTunnel.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void DNSProxy()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.NetworkExtensions, true);
            var capability = cf.Capabilities.Capability(SystemCapability.NetworkExtensions) as NetworkExtensionsCapability;
            capability.DNSProxy = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("NetworkExtensions.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("NetworkExtensionsDNSProxy.entitlements", TestEntitlementsFilePath);
        }
    }
}
