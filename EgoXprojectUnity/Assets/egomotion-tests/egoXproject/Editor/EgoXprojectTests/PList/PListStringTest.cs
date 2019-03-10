using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Egomotion.EgoXproject.Internal;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Egomotion.EgoXprojectTests.PListTests
{
    [TestFixture]
    class PListStringTest
    {
        PListString _element;

        [SetUp]
        public void SetUp()
        {
            _element = new PListString();
        }

        [Test]
        public void DefaultConstructor()
        {
            Assert.AreEqual(_element.Value, "");
        }

        [Test]
        public void SpecifiedConstructor()
        {
            PListString b = new PListString("Hello");
            Assert.AreEqual(b.Value, "Hello");
        }

        [Test]
        public void SetValue()
        {
            Assert.AreEqual(_element.Value, "");
            _element.Value = "Test";
            Assert.AreEqual(_element.Value, "Test");
        }

        [Test]
        public void XML()
        {
            Assert.AreEqual("string", _element.Xml().Name.ToString());
            Assert.AreEqual("", _element.Xml().Value.ToString());
            _element.Value = "Qwerty";
            Assert.AreEqual("string", _element.Xml().Name.ToString());
            Assert.AreEqual("Qwerty", _element.Xml().Value.ToString());
        }

        [Test]
        public void Copy()
        {
            _element.Value = "Foo";
            var copy = _element.Copy() as PListString;
            Assert.AreNotSame(copy, _element);
            Assert.AreEqual(_element.Value, copy.Value);
        }
    }
}
