using System.Collections;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;
using System.Linq;

namespace Egomotion.EgoXprojectTests.PBXProjTests
{
    [TestFixture]
    public class DictionaryTest
    {
        PBXProjDictionary _dic;

        [SetUp]
        public void SetUp()
        {
            _dic = new PBXProjDictionary();
            Assert.AreEqual(0, _dic.Count);
            _dic.Add("a", new PBXProjBoolean(true));
            _dic.Add("b", new PBXProjString("\"2\""));
            _dic.Add("c", new PBXProjString("true"));
            Assert.AreEqual(3, _dic.Count);
        }

        [TearDown]
        public void TearDown()
        {
        }


        [Test]
        public void RemoveKey()
        {
            Assert.IsTrue(_dic.ContainsKey("a"));
            Assert.IsTrue(_dic.Remove("a"));
            Assert.AreEqual(2, _dic.Count);
            Assert.IsFalse(_dic.ContainsKey("a"));
        }

        [Test]
        public void RemoveObject()
        {
            _dic.Add("foo", new PBXProjBoolean(true));
            Assert.AreEqual(4, _dic.Count);
            Assert.IsTrue(_dic.ContainsKey("foo"));
            _dic.Remove("foo");
            Assert.AreEqual(3, _dic.Count);
            Assert.IsFalse(_dic.ContainsKey("foo"));
        }

        [Test]
        public void InvalidRemove()
        {
            Assert.IsFalse(_dic.Remove(""));
        }

        [Test]
        public void RemoveNonExistant()
        {
            Assert.IsFalse(_dic.Remove("non existant"));
        }

        void DupeTest()
        {
            _dic.Add("foo", true);
            _dic.Add("foo", "10");
        }

        [Test]
        public void AddDuplicate()
        {
            Assert.Throws<System.ArgumentException>(DupeTest);
        }

        [Test]
        public void Count()
        {
            Assert.AreEqual(3, _dic.Count);
        }

        [Test]
        public void ContainsKey()
        {
            Assert.IsTrue(_dic.ContainsKey("a"));
            Assert.IsTrue(_dic.ContainsKey("b"));
            Assert.IsTrue(_dic.ContainsKey("c"));
            Assert.IsFalse(_dic.ContainsKey("q"));
        }

        [Test]
        public void Keys()
        {
            var keys = _dic.Keys.ToList();
            Assert.AreEqual(3, keys.Count);
            Assert.AreEqual(_dic.Count, keys.Count);
            Assert.AreEqual("a", keys[0]);
            Assert.AreEqual("b", keys[1]);
            Assert.AreEqual("c", keys[2]);
        }

        [Test]
        public void ExpressionForKey()
        {
            var e = _dic["a"];
            Assert.IsNotNull(e);
            Assert.IsTrue(e is PBXProjBoolean);
        }

        void NonExistantKey()
        {
            var e = _dic["q"];
            e.GetType();
        }

        [Test]
        public void ExpressionForKeyNonExistant()
        {
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(NonExistantKey);
            IPBXProjExpression v;
            _dic.TryGetValue("q", out v);
            Assert.IsNull(v);
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("{\n\ta = YES;\n\tb = \"2\";\n\tc = true;\n}", _dic.ToString());
        }

        [Test]
        public void ToStringInline()
        {
            Assert.AreEqual("{a = YES; b = \"2\"; c = true; }", _dic.ToInlineString());
        }
    }
}
