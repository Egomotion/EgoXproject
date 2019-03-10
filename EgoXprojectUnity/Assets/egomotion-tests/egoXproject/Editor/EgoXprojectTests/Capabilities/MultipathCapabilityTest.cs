using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class MultipathCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void Multipath()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Multipath, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Multipath.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("Multipath.entitlements", TestEntitlementsFilePath);
        }
    }
}
