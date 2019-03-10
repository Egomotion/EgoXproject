using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.SettingsTests
{
    [TestFixture]
    public class XcodeSettingsTest
    {
        [Test]
        public void AutoRunTest()
        {
            var settings = new XcodeSettings(Application.dataPath);
            Assert.IsTrue(settings.AutoRunEnabled);
            settings.AutoRunEnabled = false;
            Assert.IsFalse(settings.AutoRunEnabled);
        }

        //TODO ignored files
        //TODO xcode location
        //TODO save/load
    }
}
