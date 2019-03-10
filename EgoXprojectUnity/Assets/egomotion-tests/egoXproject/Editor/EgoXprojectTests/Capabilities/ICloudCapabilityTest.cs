using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class ICloudCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void None()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloud.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudNone.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void KeyValue()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            var capability = cf.Capabilities.Capability(SystemCapability.iCloud) as ICloudCapability;
            capability.KeyValueStorage = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloud.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudKeyValue.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void Documents()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            var capability = cf.Capabilities.Capability(SystemCapability.iCloud) as ICloudCapability;
            capability.iCloudDocuments = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloud.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudDocuments.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void DocumentsCustomContainers()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            var capability = cf.Capabilities.Capability(SystemCapability.iCloud) as ICloudCapability;
            capability.iCloudDocuments = true;
            capability.UseCustomContainers = true;
            capability.CustomContainers.Add("iCloud.$(CFBundleIdentifier)");
            capability.CustomContainers.Add("iCloud.my.sample.egoxproject.container");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloud.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudDocumentsContainers.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void CloudKit()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            var capability = cf.Capabilities.Capability(SystemCapability.iCloud) as ICloudCapability;
            capability.CloudKit = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloudCloudKit.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudCloudKit.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void CloudKitCustomContainers()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            var capability = cf.Capabilities.Capability(SystemCapability.iCloud) as ICloudCapability;
            capability.CloudKit = true;
            capability.UseCustomContainers = true;
            capability.CustomContainers.Add("iCloud.$(CFBundleIdentifier)");
            capability.CustomContainers.Add("iCloud.my.sample.egoxproject.container");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloudCloudKit.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudCloudKitContainers.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void All()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            var capability = cf.Capabilities.Capability(SystemCapability.iCloud) as ICloudCapability;
            capability.KeyValueStorage = true;
            capability.iCloudDocuments = true;
            capability.CloudKit = true;
            capability.UseCustomContainers = true;
            capability.CustomContainers.Add("iCloud.$(CFBundleIdentifier)");
            capability.CustomContainers.Add("iCloud.my.sample.egoxproject.container");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloudCloudKit.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudAll.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void NoneTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloud.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudNone.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void KeyValueTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            var capability = cf.Capabilities.Capability(SystemCapability.iCloud) as ICloudCapability;
            capability.KeyValueStorage = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloud.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudKeyValue.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void CloudKitTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            var capability = cf.Capabilities.Capability(SystemCapability.iCloud) as ICloudCapability;
            capability.CloudKit = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloudCloudKit.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudCloudKit.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void CloudKitCustomContainersTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            var capability = cf.Capabilities.Capability(SystemCapability.iCloud) as ICloudCapability;
            capability.CloudKit = true;
            capability.UseCustomContainers = true;
            capability.CustomContainers.Add("iCloud.$(CFBundleIdentifier)");
            capability.CustomContainers.Add("iCloud.my.sample.egoxproject.container");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloudCloudKit.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudCloudKitContainers.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void AllTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.iCloud, true);
            var capability = cf.Capabilities.Capability(SystemCapability.iCloud) as ICloudCapability;
            capability.KeyValueStorage = true;
            capability.CloudKit = true;
            capability.UseCustomContainers = true;
            capability.CustomContainers.Add("iCloud.$(CFBundleIdentifier)");
            capability.CustomContainers.Add("iCloud.my.sample.egoxproject.container");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("iCloudCloudKit.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("iCloudAllTVOS.entitlements", TestEntitlementsFilePath);
        }
    }
}
