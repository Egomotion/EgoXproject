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
    internal class PListDictionary : Dictionary<string, IPListElement>, IPListElement
    {
        public XElement Xml()
        {
            var dict = new XElement("dict");

            foreach (var kvp in this)
            {
                dict.Add(new XElement("key", kvp.Key));
                dict.Add(kvp.Value.Xml());
            }

            return dict;
        }

        public IPListElement Copy()
        {
            var copy = new PListDictionary();

            foreach (var kvp in this)
            {
                copy.Add(kvp.Key, kvp.Value.Copy());
            }

            return copy;
        }

        public T Element<T>(string key) where T : class, IPListElement
        {
            IPListElement value = null;
            this.TryGetValue(key, out value);
            return value as T;
        }

        public override string ToString()
        {
            string s = "{\n";

            foreach (var kvp in this)
            {
                s += "\t" + kvp.Key + " : " + kvp.Value + ";\n";
            }

            s += " }";
            return s;
        }

        public void Add(string key, string value)
        {
            Add(key, new PListString(value));
        }

        public void AddIfNotEmpty(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Add(key, new PListString(value));
            }
        }

        public void Add(string key, int value)
        {
            Add(key, new PListInteger(value));
        }

        public void Add(string key, bool value)
        {
            Add(key, new PListBoolean(value));
        }

        public void AddIfTrue(string key, bool value)
        {
            if (value)
            {
                Add(key, new PListBoolean(value));
            }
        }

        public void Add(string key, float value)
        {
            Add(key, new PListReal(value));
        }

        public void Add(string key, System.DateTime value)
        {
            Add(key, new PListDate(value));
        }

        public string StringValue(string key)
        {
            var value = Element<PListString>(key);

            if (value == null)
            {
                return "";
            }

            return value.Value;
        }

        public bool EnumValue<T>(string key, out T value)
        {
            System.Type type = typeof(T);

            if (!type.IsEnum)
            {
                throw new System.Exception("Parameter type must be of type System.Enum");
            }

            try
            {
                var s = StringValue(key);
                value = (T) System.Enum.Parse(type, s);
                return true;
            }
            catch
            {
                value = default (T);
                return false;
            }
        }

        public int IntValue(string key)
        {
            var value = Element<PListInteger>(key);

            if (value == null)
            {
                return 0;
            }

            return value.IntValue;
        }

        public bool BoolValue(string key)
        {
            var value = Element<PListBoolean>(key);

            if (value == null)
            {
                return false;
            }

            return value.Value;
        }

        public float FloatValue(string key)
        {
            var value = Element<PListReal>(key);

            if (value == null)
            {
                return 0.0f;
            }

            return value.FloatValue;
        }

        public System.DateTime DateValue(string key)
        {
            var value = Element<PListDate>(key);

            if (value == null)
            {
                return default(System.DateTime);
            }

            return value.Value;
        }

        public string DateString(string key)
        {
            var value = Element<PListDate>(key);

            if (value == null)
            {
                return "";
            }

            return value.StringValue;
        }

        public string DataValue(string key)
        {
            var value = Element<PListData>(key);

            if (value == null)
            {
                return "";
            }

            return value.Value;
        }

        public PListDictionary DictionaryValue(string key)
        {
            return Element<PListDictionary>(key);
        }

        public PListArray ArrayValue(string key)
        {
            return Element<PListArray>(key);
        }
    }
}
