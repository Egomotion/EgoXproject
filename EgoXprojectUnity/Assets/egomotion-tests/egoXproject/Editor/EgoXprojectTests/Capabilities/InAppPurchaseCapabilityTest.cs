using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class InAppPurchaseCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void InAppPurchase()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.InAppPurchase, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("InAppPurchase.pbxproj", TestPBXFilePath);
        }

        [Test]
        public void InAppPurchaseTVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.InAppPurchase, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("InAppPurchase.pbxproj", TestPBXFilePath);
        }

    }
}
