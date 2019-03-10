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
    class PListDictionaryTest
    {
        PListDictionary _element;

        [SetUp]
        public void SetUp()
        {
            _element = new PListDictionary();
        }

        [Test]
        public void AddValue()
        {
            Assert.AreEqual(_element.Count, 0);
            _element["key"] = new PListBoolean();
            Assert.AreEqual(_element.Count, 1);
            Assert.IsNotNull(_element["key"]);
        }

        [Test]
        public void RemoveValue()
        {
            Assert.AreEqual(_element.Count, 0);
            _element["key1"] = new PListBoolean();
            _element["key2"] = new PListBoolean();
            Assert.AreEqual(_element.Count, 2);
            _element.Remove("key1");
            Assert.AreEqual(_element.Count, 1);
            Assert.IsTrue(_element.ContainsKey("key2"));
            Assert.IsFalse(_element.ContainsKey("key1"));
        }

        [Test]
        public void XML()
        {
            Assert.AreEqual("dict", _element.Xml().Name.ToString());
            Assert.AreEqual("", _element.Xml().Value.ToString());
        }

        [Test]
        public void Copy()
        {
            _element.Add("A", new PListString("Foo"));
            _element.Add("B", new PListInteger(10));
            var copy = _element.Copy() as PListDictionary;
            Assert.AreNotSame(copy, _element);
            Assert.AreEqual(_element.Count, copy.Count);

            foreach (var kvp in _element)
            {
                Assert.AreNotSame(kvp.Value, copy[kvp.Key]);
            }
        }
    }
}