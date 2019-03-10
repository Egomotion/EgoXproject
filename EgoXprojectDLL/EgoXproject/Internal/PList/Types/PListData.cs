//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Xml.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class PListData : IPListElement, System.IEquatable<PListData>
    {
        public PListData()
        {
            Value = "";
        }

        public PListData(string data)
        {
            Value = data;
        }

        public string Value
        {
            get;
            set;
        }

        public XElement Xml()
        {
            return new XElement("data", Value);
        }

        public IPListElement Copy()
        {
            return new PListData(Value);
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PListData);
        }

        public bool Equals(PListData element)
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