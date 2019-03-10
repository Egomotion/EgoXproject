//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Xml.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class PListString : IPListElement, System.IEquatable<PListString>
    {
        public PListString()
        {
            Value = "";
        }

        public PListString(string value)
        {
            Value = value;
        }

        public string Value
        {
            get;
            set;
        }

        public XElement Xml()
        {
            return new XElement("string", Value);
        }

        public IPListElement Copy()
        {
            return new PListString(Value);
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PListString);
        }

        public bool Equals(PListString element)
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

            return this.Value.Equals(element.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
