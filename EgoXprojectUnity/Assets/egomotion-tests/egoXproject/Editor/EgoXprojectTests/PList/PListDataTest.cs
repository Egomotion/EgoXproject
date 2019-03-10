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
    class PListDataTest
    {
        PListData _element;

        [SetUp]
        public void SetUp()
        {
            _element = new PListData();
        }

        [Test]
        public void DefaultConstructor()
        {
            Assert.AreEqual(_element.Value, "");
        }

        [Test]
        public void SpecifiedConstructor()
        {
            PListData b = new PListData("bXkgcGhvdG8=");
            Assert.AreEqual(b.Value, "bXkgcGhvdG8=");
        }

        [Test]
        public void SetValue()
        {
            Assert.AreEqual(_element.Value, "");
            _element.Value = "ASNFZw==";
            Assert.AreEqual(_element.Value, "ASNFZw==");
        }

        [Test]
        public void XML()
        {
            Assert.AreEqual("data", _element.Xml().Name.ToString());
            Assert.AreEqual("", _element.Xml().Value.ToString());
            _element.Value = "ASNFZw==";
            Assert.AreEqual("data", _element.Xml().Name.ToString());
            Assert.AreEqual("ASNFZw==", _element.Xml().Value.ToString());
        }

        [Test]
        public void Copy()
        {
            _element.Value = "ASNFZw==";
            var copy = _element.Copy() as PListData;
            Assert.AreNotSame(copy, _element);
            Assert.AreEqual(_element.Value, copy.Value);
        }
    }
}