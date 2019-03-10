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
    class PListArrayTest
    {
        PListArray _element;

        [SetUp]
        public void SetUp()
        {
            _element = new PListArray();
        }


        [Test]
        public void AddValue()
        {
            Assert.AreEqual(_element.Count, 0);
            _element.Add(new PListBoolean());
            Assert.AreEqual(_element.Count, 1);
        }

        [Test]
        public void RemoveValue()
        {
            Assert.AreEqual(_element.Count, 0);
            _element.Add(new PListBoolean());
            Assert.AreEqual(_element.Count, 1);
            _element.RemoveAt(0);
            Assert.AreEqual(_element.Count, 0);
        }

        [Test]
        public void XML()
        {
            Assert.AreEqual("array", _element.Xml().Name.ToString());
            Assert.AreEqual("", _element.Xml().Value.ToString());
        }

        [Test]
        public void Copy()
        {
            _element.Add(new PListString("Foo"));
            _element.Add(new PListInteger(10));
            var copy = _element.Copy() as PListArray;
            Assert.AreNotSame(copy, _element);
            Assert.AreEqual(_element.Count, copy.Count);

            for (int ii = 0; ii < _element.Count; ++ii)
            {
                Assert.AreNotSame(_element[ii], copy[ii]);
            }
        }
    }
}