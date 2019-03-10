using System.Collections;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.PBXProjTests
{
    [TestFixture]
    public class ArrayTest
    {
        PBXProjArray _array;

        [SetUp]
        public void SetUp()
        {
            _array = new PBXProjArray();
            _array.Add(new PBXProjBoolean(true));
            _array.Add(new PBXProjString("\"2\""));
            _array.Add(new PBXProjString("foo"));
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("(\n\tYES,\n\t\"2\",\n\tfoo,\n)", _array.ToString());
        }

        [Test]
        public void ToInlineString()
        {
            Assert.AreEqual("(YES, \"2\", foo, )", _array.ToInlineString());
        }

    }
}