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
    class PListDateTest
    {
        PListDate _element;

        [SetUp]
        public void SetUp()
        {
            _element = new PListDate();
        }

        [Test]
        public void DefaultConstructor()
        {
            Assert.IsNotNull(_element.Value);
        }

        [Test]
        public void SpecifiedConstructor()
        {
            System.DateTime d = System.DateTime.Now;
            PListDate b = new PListDate(d);
            Assert.AreEqual(b.Value, d);
        }

        [Test]
        public void StringConstructor()
        {
            PListDate b = new PListDate("2014-03-08T13:31:13Z");
            Assert.AreEqual(b.StringValue, "2014-03-08T13:31:13Z");
        }

        [Test]
        public void SetValue()
        {
            System.DateTime d = System.DateTime.Now;
            Assert.AreNotEqual(_element.Value, d);
            _element.Value = d;
            Assert.AreEqual(_element.Value, d);
        }

        [Test]
        public void StringValue()
        {
            System.DateTime d = System.DateTime.Now;
            _element.Value = d;
            Assert.AreEqual(_element.StringValue, d.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
            d = new System.DateTime();
            Assert.AreNotEqual(d.ToString(), _element.Value.ToString());
            _element.StringValue = d.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            Assert.AreEqual(d.ToString(), _element.Value.ToString());
        }

        [Test]
        public void XML()
        {
            _element.StringValue = "2014-03-08T13:31:13Z";
            Assert.AreEqual("date", _element.Xml().Name.ToString());
            Assert.AreEqual("2014-03-08T13:31:13Z", _element.Xml().Value.ToString());
        }

        [Test]
        public void InvalidDateString()
        {
            var d = System.DateTime.Now;
            _element.Value = d;
            _element.StringValue = "Foo";
            Assert.AreEqual(d, _element.Value);
        }

        [Test]
        public void Copy()
        {
            _element.StringValue = "2014-03-08T13:31:13Z";
            var copy = _element.Copy() as PListDate;
            Assert.AreNotSame(copy, _element);
            Assert.AreEqual(_element.Value, copy.Value);
        }
    }
}