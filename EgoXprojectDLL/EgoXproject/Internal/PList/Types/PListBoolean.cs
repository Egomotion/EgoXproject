//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Xml.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class PListBoolean : IPListElement, System.IEquatable<PListBoolean>
    {
        public PListBoolean()
        {
            Value = false;
        }

        public PListBoolean(bool value)
        {
            Value = value;
        }

        public bool Value
        {
            get;
            set;
        }

        public XElement Xml()
        {
            return new XElement(Value ? "true" : "false");
        }

        public IPListElement Copy()
        {
            return new PListBoolean(Value);
        }


        public override string ToString()
        {
            return Value.ToString();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PListBoolean);
        }

        public bool Equals(PListBoolean element)
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
