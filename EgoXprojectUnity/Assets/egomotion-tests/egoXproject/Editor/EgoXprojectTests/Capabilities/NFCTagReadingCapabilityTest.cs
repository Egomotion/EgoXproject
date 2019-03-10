using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;


namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class NFCTagReadingCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void NFCTagReading()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.NFCTagReading, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("NFCTagReading.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("NFCTagReading.entitlements", TestEntitlementsFilePath);
        }
    }
}
