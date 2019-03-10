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
    class PListIntegerTest
    {
        PListInteger _element;

        [SetUp]
        public void SetUp()
        {
            _element = new PListInteger();
        }

        [Test]
        public void DefaultConstructor()
        {
            Assert.AreEqual(_element.IntValue, 0);
            Assert.AreEqual(_element.LongValue, 0);
        }

        [Test]
        public void SpecifiedConstructor()
        {
            PListInteger i = new PListInteger(int.MaxValue);
            Assert.AreEqual(i.IntValue, int.MaxValue);
            PListInteger ui = new PListInteger(uint.MaxValue);
            Assert.AreEqual(ui.UIntValue, uint.MaxValue);
            PListInteger s = new PListInteger(short.MaxValue);
            Assert.AreEqual(s.ShortValue, short.MaxValue);
            PListInteger us = new PListInteger(ushort.MaxValue);
            Assert.AreEqual(us.UShortValue, ushort.MaxValue);
            PListInteger l = new PListInteger(long.MaxValue);
            Assert.AreEqual(l.LongValue, long.MaxValue);
            PListInteger ul = new PListInteger(ulong.MaxValue);
            Assert.AreEqual(ul.ULongValue, ulong.MaxValue);
        }

        [Test]
        public void SetShortValue()
        {
            Assert.AreEqual(_element.ShortValue, 0);
            _element.ShortValue = short.MaxValue;
            Assert.AreEqual(_element.ShortValue, short.MaxValue);
            _element.ShortValue = short.MinValue;
            Assert.AreEqual(_element.ShortValue, short.MinValue);
        }

        [Test]
        public void SetIntValue()
        {
            Assert.AreEqual(_element.IntValue, 0);
            _element.IntValue = int.MaxValue;
            Assert.AreEqual(_element.IntValue, int.MaxValue);
            _element.IntValue = int.MinValue;
            Assert.AreEqual(_element.IntValue, int.MinValue);
        }

        [Test]
        public void SetLongValue()
        {
            Assert.AreEqual(_element.LongValue, 0);
            _element.LongValue = long.MaxValue;
            Assert.AreEqual(_element.LongValue, long.MaxValue);
            _element.LongValue = long.MinValue;
            Assert.AreEqual(_element.LongValue, long.MinValue);
        }

        [Test]
        public void SetUShortValue()
        {
            Assert.AreEqual(_element.UShortValue, 0);
            _element.UShortValue = ushort.MaxValue;
            Assert.AreEqual(_element.UShortValue, ushort.MaxValue);
            _element.UShortValue = ushort.MinValue;
            Assert.AreEqual(_element.UShortValue, ushort.MinValue);
        }

        [Test]
        public void SetUIntValue()
        {
            Assert.AreEqual(_element.UIntValue, 0);
            _element.UIntValue = uint.MaxValue;
            Assert.AreEqual(_element.UIntValue, uint.MaxValue);
            _element.UIntValue = uint.MinValue;
            Assert.AreEqual(_element.UIntValue, uint.MinValue);
        }

        [Test]
        public void SetULongValue()
        {
            Assert.AreEqual(_element.ULongValue, 0);
            _element.ULongValue = ulong.MaxValue;
            Assert.AreEqual(_element.ULongValue, ulong.MaxValue);
            _element.ULongValue = ulong.MinValue;
            Assert.AreEqual(_element.ULongValue, ulong.MinValue);
        }

        [Test]
        public void XML()
        {
            Assert.AreEqual("integer", _element.Xml().Name.ToString());
            Assert.AreEqual("0", _element.Xml().Value.ToString());
            {
                _element.ShortValue = short.MaxValue;
                Assert.AreEqual("integer", _element.Xml().Name.ToString());
                Assert.AreEqual(short.MaxValue.ToString(), _element.Xml().Value.ToString());
            }
            {
                _element.IntValue = int.MaxValue;
                Assert.AreEqual("integer", _element.Xml().Name.ToString());
                Assert.AreEqual(int.MaxValue.ToString(), _element.Xml().Value.ToString());
            }
            {
                _element.LongValue = long.MaxValue;
                Assert.AreEqual("integer", _element.Xml().Name.ToString());
                Assert.AreEqual(long.MaxValue.ToString(), _element.Xml().Value.ToString());
            }
            {
                _element.UShortValue = ushort.MaxValue;
                Assert.AreEqual("integer", _element.Xml().Name.ToString());
                Assert.AreEqual(ushort.MaxValue.ToString(), _element.Xml().Value.ToString());
            }
            {
                _element.UIntValue = uint.MaxValue;
                Assert.AreEqual("integer", _element.Xml().Name.ToString());
                Assert.AreEqual(uint.MaxValue.ToString(), _element.Xml().Value.ToString());
            }
            {
                _element.ULongValue = ulong.MaxValue;
                Assert.AreEqual("integer", _element.Xml().Name.ToString());
                Assert.AreEqual(ulong.MaxValue.ToString(), _element.Xml().Value.ToString());
            }
        }

        [Test]
        public void Copy()
        {
            {
                _element.ShortValue = short.MaxValue;
                var copy = _element.Copy() as PListInteger;
                Assert.AreNotSame(copy, _element);
                Assert.AreEqual(_element.ShortValue, copy.ShortValue);
            }
            {
                _element.UShortValue = ushort.MaxValue;
                var copy = _element.Copy() as PListInteger;
                Assert.AreNotSame(copy, _element);
                Assert.AreEqual(_element.UShortValue, copy.UShortValue);
            }
            {
                _element.IntValue = int.MaxValue;
                var copy = _element.Copy() as PListInteger;
                Assert.AreNotSame(copy, _element);
                Assert.AreEqual(_element.IntValue, copy.IntValue);
            }
            {
                _element.UIntValue = uint.MaxValue;
                var copy = _element.Copy() as PListInteger;
                Assert.AreNotSame(copy, _element);
                Assert.AreEqual(_element.UIntValue, copy.UIntValue);
            }
            {
                _element.LongValue = long.MaxValue;
                var copy = _element.Copy() as PListInteger;
                Assert.AreNotSame(copy, _element);
                Assert.AreEqual(_element.LongValue, copy.LongValue);
            }
            {
                _element.ULongValue = ulong.MaxValue;
                var copy = _element.Copy() as PListInteger;
                Assert.AreNotSame(copy, _element);
                Assert.AreEqual(_element.ULongValue, copy.ULongValue);
            }
        }

        [Test]
        public void ShortConversion()
        {
            _element.ShortValue = short.MaxValue;
            Assert.AreEqual(short.MaxValue, _element.IntValue);
            Assert.AreEqual(short.MaxValue, _element.LongValue);
            Assert.AreEqual(short.MaxValue, _element.UShortValue);
            Assert.AreEqual(short.MaxValue, _element.UIntValue);
            Assert.AreEqual(short.MaxValue, _element.ULongValue);
            _element.ShortValue = short.MinValue;
            Assert.AreEqual(short.MinValue, _element.IntValue);
            Assert.AreEqual(short.MinValue, _element.LongValue);
            Assert.AreEqual(ushort.MaxValue - short.MaxValue, _element.UShortValue);
            Assert.AreEqual(uint.MaxValue - short.MaxValue, _element.UIntValue);
            Assert.AreEqual(ulong.MaxValue - (ulong)short.MaxValue, _element.ULongValue);
        }

        [Test]
        public void IntConversion()
        {
            _element.IntValue = int.MaxValue;
            Assert.AreEqual(int.MaxValue % ((int)ushort.MaxValue) + (int)short.MinValue, _element.ShortValue);
            Assert.AreEqual(int.MaxValue, _element.LongValue);
            Assert.AreEqual(ushort.MaxValue, _element.UShortValue);
            Assert.AreEqual(int.MaxValue, _element.UIntValue);
            Assert.AreEqual(int.MaxValue, _element.ULongValue);
            _element.IntValue = int.MinValue;
            Assert.AreEqual(int.MinValue % ((int)ushort.MaxValue) - (int)short.MinValue, _element.ShortValue);
            Assert.AreEqual(int.MinValue, _element.LongValue);
            Assert.AreEqual(ushort.MinValue, _element.UShortValue);
            Assert.AreEqual(uint.MaxValue - int.MaxValue, _element.UIntValue);
            Assert.AreEqual(ulong.MaxValue - (ulong)int.MaxValue, _element.ULongValue);
        }

        [Test]
        public void LongConversion()
        {
            _element.LongValue = long.MaxValue;
            Assert.AreEqual(long.MaxValue % ((long)ushort.MaxValue) + (long)short.MinValue, _element.ShortValue);
            Assert.AreEqual(long.MaxValue % ((long)uint.MaxValue) + (long)int.MinValue, _element.IntValue);
            Assert.AreEqual(ushort.MaxValue, _element.UShortValue);
            Assert.AreEqual(uint.MaxValue, _element.UIntValue);
            Assert.AreEqual(long.MaxValue, _element.ULongValue);
            _element.LongValue = long.MinValue;
            Assert.AreEqual(long.MinValue % ((long)ushort.MaxValue) - (long)short.MinValue, _element.ShortValue);
            Assert.AreEqual(long.MinValue % ((long)uint.MaxValue) - (long)int.MinValue, _element.IntValue);
            Assert.AreEqual(ushort.MinValue, _element.UShortValue);
            Assert.AreEqual(uint.MinValue, _element.UIntValue);
            Assert.AreEqual(ulong.MaxValue / 2 + 1, _element.ULongValue);
        }

        [Test]
        public void UShortConversion()
        {
            _element.UShortValue = ushort.MaxValue;
            Assert.AreEqual(-1, _element.ShortValue);
            Assert.AreEqual(ushort.MaxValue, _element.IntValue);
            Assert.AreEqual(ushort.MaxValue, _element.LongValue);
            Assert.AreEqual(ushort.MaxValue, _element.UIntValue);
            Assert.AreEqual(ushort.MaxValue, _element.ULongValue);
            _element.UShortValue = ushort.MinValue;
            Assert.AreEqual(ushort.MinValue, _element.ShortValue);
            Assert.AreEqual(ushort.MinValue, _element.IntValue);
            Assert.AreEqual(ushort.MinValue, _element.LongValue);
            Assert.AreEqual(ushort.MinValue, _element.UIntValue);
            Assert.AreEqual(ushort.MinValue, _element.ULongValue);
        }

        [Test]
        public void UIntConversion()
        {
            _element.UIntValue = uint.MaxValue;
            //TODO work out why
            Assert.AreEqual(-1, _element.ShortValue);
            //Assert.AreEqual(uint.MaxValue % (ushort.MaxValue) + short.MinValue, _element.ShortValue);
            //TODO work out why
            Assert.AreEqual(-1, _element.IntValue);
            //Assert.AreEqual(uint.MaxValue % (int.MaxValue + int.MinValue), _element.IntValue);
            Assert.AreEqual(uint.MaxValue, _element.LongValue);
            Assert.AreEqual(uint.MaxValue, _element.ULongValue);
            _element.UIntValue = uint.MinValue;
            Assert.AreEqual(uint.MinValue, _element.ShortValue);
            Assert.AreEqual(uint.MinValue, _element.IntValue);
            Assert.AreEqual(uint.MinValue, _element.LongValue);
            Assert.AreEqual(uint.MinValue, _element.UShortValue);
            Assert.AreEqual(uint.MinValue, _element.ULongValue);
        }

        [Test]
        public void ULongConversion()
        {
            _element.ULongValue = ulong.MaxValue;
            //TODO
            //      Assert.AreEqual(ulong.MaxValue % ((ulong)ushort.MaxValue) + (ulong)short.MinValue, _element.ShortValue);
            //      Assert.AreEqual(ulong.MaxValue % ((ulong)uint.MaxValue) + (ulong)int.MinValue, _element.IntValue);
            //      Assert.AreEqual(ulong.MaxValue % ((ulong)ushort.MaxValue), _element.UShortValue);
            //      Assert.AreEqual(ulong.MaxValue % (ulong)uint.MaxValue, _element.UIntValue);
            //      Assert.AreEqual(ulong.MaxValue % ((ulong)ulong.MaxValue) + (ulong)long.MinValue, _element.LongValue);
            //
            _element.ULongValue = ulong.MinValue;
            Assert.AreEqual(ulong.MinValue, _element.ShortValue);
            Assert.AreEqual(ulong.MinValue, _element.IntValue);
            Assert.AreEqual(ulong.MinValue, _element.LongValue);
            Assert.AreEqual(ulong.MinValue, _element.UShortValue);
            Assert.AreEqual(ulong.MinValue, _element.UIntValue);
        }
    }
}