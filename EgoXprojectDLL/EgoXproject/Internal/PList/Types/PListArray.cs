//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class PListArray : List<IPListElement>, IPListElement
    {
        public PListArray()
        {
        }

        public PListArray(List<string> values)
        {
            foreach (var v in values)
            {
                Add(v);
            }
        }

        public PListArray(string[] values)
        {
            foreach (var v in values)
            {
                Add(v);
            }
        }

        public PListArray(string value)
        {
            Add(value);
        }

        public PListArray(IPListElement[] values)
        {
            foreach (var v in values)
            {
                Add(v);
            }
        }

        public PListArray(IPListElement value)
        {
            Add(value);
        }

        public XElement Xml()
        {
            var array = new XElement("array");

            foreach (var element in this)
            {
                array.Add(element.Xml());
            }

            return array;
        }

        public IPListElement Copy()
        {
            var copy = new PListArray();

            foreach (var element in this)
            {
                copy.Add(element.Copy());
            }

            return copy;
        }

        public T Element<T>(int index) where T : class, IPListElement
        {
            return this[index] as T;
        }

        public override string ToString()
        {
            string s = "( ";

            foreach (var element in this)
            {
                s += element + ",";
            }

            s += " )";
            return s;
        }

        public void Add(string value)
        {
            Add(new PListString(value));
        }

        public void Add(int value)
        {
            Add(new PListInteger(value));
        }

        public void Add(bool value)
        {
            Add(new PListBoolean(value));
        }

        public void Add(float value)
        {
            Add(new PListReal(value));
        }

        public void Add(System.DateTime value)
        {
            Add(new PListDate(value));
        }

        public string StringValue(int index)
        {
            var value = Element<PListString>(index);

            if (value == null)
            {
                return "";
            }

            return value.Value;
        }

        public bool EnumValue<T> (int index, out T value)
        {
            System.Type type = typeof (T);

            if (!type.IsEnum)
            {
                throw new System.Exception ("Parameter type must be of type System.Enum");
            }

            try
            {
                var s = StringValue (index);
                value = (T)System.Enum.Parse (type, s);
                return true;
            }
            catch
            {
                value = default (T);
                return false;
            }
        }

        public int IntValue(int index)
        {
            var value = Element<PListInteger>(index);

            if (value == null)
            {
                return 0;
            }

            return value.IntValue;
        }

        public bool BoolValue(int index)
        {
            var value = Element<PListBoolean>(index);

            if (value == null)
            {
                return false;
            }

            return value.Value;
        }

        public float FloatValue(int index)
        {
            var value = Element<PListReal>(index);

            if (value == null)
            {
                return 0.0f;
            }

            return value.FloatValue;
        }

        public string[] ToStringArray()
        {
            List<string> values = new List<string>();

            for (int ii = 0; ii < Count; ++ii)
            {
                var s = StringValue(ii);

                if (!string.IsNullOrEmpty(s))
                {
                    values.Add(s);
                }
            }

            return values.ToArray();
        }

        public PListDictionary DictionaryValue(int index)
        {
            return Element<PListDictionary>(index);
        }

        public PListArray ArrayValue(int index)
        {
            return Element<PListArray>(index);
        }
    }
}

