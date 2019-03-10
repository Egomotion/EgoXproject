using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class WalletCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void Wallet()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Wallet, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Wallet.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("Wallet.entitlements", TestEntitlementsFilePath);
        }
    }
}
