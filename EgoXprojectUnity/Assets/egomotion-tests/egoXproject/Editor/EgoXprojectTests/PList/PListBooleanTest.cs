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
    class PListBooleanTest
    {
        PListBoolean _element;

        [SetUp]
        public void SetUp()
        {
            _element = new PListBoolean();
        }

        [Test]
        public void DefaultConstructor()
        {
            Assert.IsFalse(_element.Value);
        }

        [Test]
        public void SpecifiedConstructor()
        {
            PListBoolean b = new PListBoolean(true);
            Assert.IsTrue(b.Value);
        }

        [Test]
        public void SetValue()
        {
            Assert.IsFalse(_element.Value);
            _element.Value = true;
            Assert.IsTrue(_element.Value);
        }

        [Test]
        public void XML()
        {
            Assert.AreEqual("false", _element.Xml().Name.ToString());
            _element.Value = true;
            Assert.AreEqual("true", _element.Xml().Name.ToString());
        }

        [Test]
        public void Copy()
        {
            _element.Value = true;
            var copy = _element.Copy() as PListBoolean;
            Assert.AreNotSame(copy, _element);
            Assert.AreEqual(_element.Value, copy.Value);
        }
    }
}