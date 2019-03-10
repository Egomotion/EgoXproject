using UnityEngine;
using System.Collections;
using UnityEditor;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.PBXProjTests
{
    [TestFixture]
    public class BoolTest
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void AssignmentTest()
        {
            var b = new PBXProjBoolean(true);
            Assert.IsTrue(b.Value);
            b.Value = false;
            Assert.IsFalse(b.Value);
            b = new PBXProjBoolean(false);
            Assert.IsFalse(b.Value);
            b.Value = true;
            Assert.IsTrue(b.Value);
        }

        [Test]
        public void ToStringTest()
        {
            var b = new PBXProjBoolean(true);
            Assert.AreEqual("YES", b.ToString());
            b.Value = false;
            Assert.AreEqual("NO", b.ToString());
        }
    }
}
