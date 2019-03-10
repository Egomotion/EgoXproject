//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Xml.Linq;

namespace Egomotion.EgoXproject.Internal
{
    //TODO handle double, long, etc. Make more like NSNumber
    internal class PListReal : IPListElement, System.IEquatable<PListReal>
    {
        public PListReal()
        {
            FloatValue = 0.0f;
        }

        public PListReal(float value)
        {
            FloatValue = value;
        }

        public float FloatValue
        {
            get;
            set;
        }

        public XElement Xml()
        {
            return new XElement("real", FloatValue);
        }

        public IPListElement Copy()
        {
            return new PListReal(FloatValue);
        }

        public override string ToString()
        {
            return FloatValue.ToString();
        }


        public override bool Equals(object obj)
        {
            return this.Equals(obj as PListReal);
        }

        public bool Equals(PListReal element)
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

            return Mathf.Approximately(this.FloatValue, element.FloatValue);
        }

        public override int GetHashCode()
        {
            return FloatValue.GetHashCode();
        }
    }
}

