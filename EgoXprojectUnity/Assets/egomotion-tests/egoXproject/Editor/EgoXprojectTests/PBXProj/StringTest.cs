using UnityEngine;
using System.Collections;
using UnityEditor;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.PBXProjTests
{
    [TestFixture]
    public class StringTest
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
            var b = new PBXProjString("Foo");
            Assert.AreEqual("Foo", b.Value);
            b.Value = "Bar";
            Assert.AreEqual("Bar", b.Value);
        }

        [Test]
        public void QuotedAssignmentTest()
        {
            var b = new PBXProjString("\"Foo\"");
            Assert.AreEqual("\"Foo\"", b.Value);
        }

        [Test]
        public void ToStringTest()
        {
            var b = new PBXProjString("Foo");
            Assert.AreEqual("Foo", b.ToString());
            b.Value = "\"Foo\"";
            Assert.AreEqual("\"Foo\"", b.ToString());
        }
    }
}