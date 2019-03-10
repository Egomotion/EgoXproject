//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Xml.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class PListInteger : IPListElement, System.IEquatable<PListInteger>
    {
        long _value = 0;

        enum Type
        {
            Short,
            UShort,
            Int,
            UInt,
            Long,
            ULong
        };

        Type _type = Type.Int;

        public PListInteger()
        {
            IntValue = 0;
        }

        public PListInteger(short value)
        {
            ShortValue = value;
        }

        public PListInteger(ushort value)
        {
            UShortValue = value;
        }

        public PListInteger(int value)
        {
            IntValue = value;
        }

        public PListInteger(uint value)
        {
            UIntValue = value;
        }

        public PListInteger(long value)
        {
            LongValue = value;
        }

        public PListInteger(ulong value)
        {
            ULongValue = value;
        }

        public long LongValue
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _type = Type.Long;
            }
        }

        public ulong ULongValue
        {
            get
            {
                return (ulong) _value;
            }
            set
            {
                _value = (long) value;
                _type = Type.ULong;
            }
        }

        public int IntValue
        {
            get
            {
                return (int) _value;
            }
            set
            {
                _value = (long) value;
                _type = Type.Int;
            }
        }

        public uint UIntValue
        {
            get
            {
                return (uint) _value;
            }
            set
            {
                _value = (long) value;
                _type = Type.UInt;
            }
        }

        public short ShortValue
        {
            get
            {
                return (short) _value;
            }
            set
            {
                _value = (long) value;
                _type = Type.Short;
            }
        }

        public ushort UShortValue
        {
            get
            {
                return (ushort) _value;
            }
            set
            {
                _value = (long) value;
                _type = Type.UShort;
            }
        }

        public XElement Xml()
        {
            switch (_type)
            {
            case Type.Int:
                return new XElement("integer", IntValue);

            case Type.Short:
                return new XElement("integer", ShortValue);

            case Type.UInt:
                return new XElement("integer", UIntValue);

            case Type.ULong:
                return new XElement("integer", ULongValue);

            case Type.UShort:
                return new XElement("integer", UShortValue);

            case Type.Long:
            default:
                return new XElement("integer", LongValue);
            }
        }


        public IPListElement Copy()
        {
            switch (_type)
            {
            case Type.Int:
                return new PListInteger(IntValue);

            case Type.Short:
                return new PListInteger(ShortValue);

            case Type.UInt:
                return new PListInteger(UIntValue);

            case Type.ULong:
                return new PListInteger(ULongValue);

            case Type.UShort:
                return new PListInteger(UShortValue);

            case Type.Long:
            default:
                return new PListInteger(LongValue);
            }
        }


        public override string ToString()
        {
            switch (_type)
            {
            case Type.Int:
                return IntValue.ToString();

            case Type.Short:
                return ShortValue.ToString();

            case Type.UInt:
                return UIntValue.ToString();

            case Type.ULong:
                return ULongValue.ToString();

            case Type.UShort:
                return UShortValue.ToString();

            case Type.Long:
            default:
                return LongValue.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PListInteger);
        }

        public bool Equals(PListInteger element)
        {
            if (System.Object.ReferenceEquals(element, null))
            {
                return false;
            }

            if (System.Object.ReferenceEquals(element, this))
            {
                return true;
            }

            if (this.GetType() != element.GetType())
            {
                return false;
            }

            return this.LongValue.Equals(element.LongValue);
        }

        public override int GetHashCode()
        {
            return LongValue.GetHashCode();
        }
    }
}
