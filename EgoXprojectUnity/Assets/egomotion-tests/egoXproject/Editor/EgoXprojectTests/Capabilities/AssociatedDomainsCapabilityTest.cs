using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class AssociatedDomainsCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void Empty()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.AssociatedDomains, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("AssociatedDomains.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("AssociatedDomainsEmpty.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void Entry()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.AssociatedDomains, true);
            var capability = cf.Capabilities.Capability(SystemCapability.AssociatedDomains) as AssociatedDomainsCapability;
            capability.AssociatedDomains.Add("webcredentials:egomotion.co.uk");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("AssociatedDomains.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("AssociatedDomainsEntry.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void EmptyTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.AssociatedDomains, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("AssociatedDomains.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("AssociatedDomainsEmpty.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void EntryTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.AssociatedDomains, true);
            var capability = cf.Capabilities.Capability(SystemCapability.AssociatedDomains) as AssociatedDomainsCapability;
            capability.AssociatedDomains.Add("webcredentials:egomotion.co.uk");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("AssociatedDomains.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("AssociatedDomainsEntry.entitlements", TestEntitlementsFilePath);
        }
    }
}
