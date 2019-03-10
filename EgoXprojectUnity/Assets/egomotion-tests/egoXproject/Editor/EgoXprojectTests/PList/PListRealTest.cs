using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Egomotion.EgoXproject.Internal;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace Egomotion.EgoXprojectTests.PListTests
{
    [TestFixture]
    class PListRealTest
    {
        PListReal _element;

        [SetUp]
        public void SetUp()
        {
            _element = new PListReal();
        }

        [Test]
        public void DefaultConstructor()
        {
            Assert.AreEqual(_element.FloatValue, 0.0f);
        }

        [Test]
        public void SpecifiedConstructor()
        {
            PListReal f = new PListReal(10.12f);
            Assert.AreEqual(f.FloatValue, 10.12f);
        }

        [Test]
        public void SetValue()
        {
            Assert.AreEqual(_element.FloatValue, 0);
            _element.FloatValue = 10.11f;
            Assert.AreEqual(_element.FloatValue, 10.11f);
        }

        [Test]
        public void XML()
        {
            Assert.AreEqual("real", _element.Xml().Name.ToString());
            Assert.AreEqual("0", _element.Xml().Value.ToString());
            _element.FloatValue = 12.34f;
            Assert.AreEqual("real", _element.Xml().Name.ToString());
            Assert.IsTrue(_element.Xml().Value.ToString().StartsWith("12.34"));
            _element.FloatValue = 3.14159265358979f;
            Assert.IsTrue(_element.Xml().Value.ToString().StartsWith("3.14159274"));
        }

        [Test]
        public void Copy()
        {
            _element.FloatValue = 3.14f;
            var copy = _element.Copy() as PListReal;
            Assert.AreNotSame(copy, _element);
            Assert.AreEqual(_element.FloatValue, copy.FloatValue);
        }

        [Test]
        public void Float()
        {
            float original = Mathf.PI;
            _element.FloatValue = original;
            float f = _element.FloatValue;
            Assert.IsTrue(Mathf.Approximately(f, original));
        }
    }
}
