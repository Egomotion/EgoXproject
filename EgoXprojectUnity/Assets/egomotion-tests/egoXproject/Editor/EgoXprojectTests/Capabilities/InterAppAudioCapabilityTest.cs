using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class InterAppAudioCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void InterAppAudio()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.InterAppAudio, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("InterAppAudio.pbxproj", TestPBXFilePath);
            CompareEntitlementFiles("InterAppAudio.entitlements", TestEntitlementsFilePath);
        }
    }
}
