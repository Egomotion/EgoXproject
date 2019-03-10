using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class SiriCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void Siri()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Siri, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Siri.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("Siri.entitlements", TestEntitlementsFilePath);
        }
    }
}
