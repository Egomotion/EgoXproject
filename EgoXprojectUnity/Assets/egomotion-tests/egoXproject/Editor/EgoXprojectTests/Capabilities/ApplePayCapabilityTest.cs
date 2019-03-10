using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class ApplePayCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void Empty()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.ApplePay, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("ApplePay.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("ApplePayEmpty.entitlements", TestEntitlementsFilePath);
        }

        [Test]
        public void Entry()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.ApplePay, true);
            var capability = cf.Capabilities.Capability(SystemCapability.ApplePay) as ApplePayCapability;
            capability.MerchantIds.Add("merchant.uk.co.egomotion.egoxproject.merch1");
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("ApplePay.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("ApplePayEntry.entitlements", TestEntitlementsFilePath);
        }

    }
}
