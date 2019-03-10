using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class AppGroupsCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void Empty()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator(  );
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.AppGroups, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("AppGroups.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("AppGroupsEmpty.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void Entry()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.AppGroups, true);
            var capability = cf.Capabilities.Capability(SystemCapability.AppGroups) as AppGroupsCapability;
            capability.AppGroups.Add("group.uk.co.egomotion.egoxproject.grp1");
            capability.AppGroups.Add("group.uk.co.egomotion.egoxproject.grp2");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("AppGroups.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("AppGroupsEntry.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void EmptyTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator(  );
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.AppGroups, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("AppGroups.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("AppGroupsEmpty.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void EntryTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.AppGroups, true);
            var capability = cf.Capabilities.Capability(SystemCapability.AppGroups) as AppGroupsCapability;
            capability.AppGroups.Add("group.uk.co.egomotion.egoxproject.grp1");
            capability.AppGroups.Add("group.uk.co.egomotion.egoxproject.grp2");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("AppGroups.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("AppGroupsEntry.entitlements", TestEntitlementsFilePath);
        }
    }
}
