using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class PersonalVPNCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void PersonalVPN()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.PersonalVPN, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("PersonalVPN.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("PersonalVPN.entitlements", TestEntitlementsFilePath);
        }
    }
}
