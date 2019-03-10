//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class PListDate : IPListElement, System.IEquatable<PListDate>
    {
        const string DATE_FORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";

        public PListDate()
        {
            Value = DateTime.UtcNow;
        }

        public PListDate(DateTime date)
        {
            Value = date;
        }

        public PListDate(string dateString)
        {
            StringValue = dateString;
        }

        public DateTime Value
        {
            get;
            set;
        }

        public string StringValue
        {
            get
            {
                return Value.ToString(DATE_FORMAT);
            }
            set
            {
                DateTime date;

                if (string.IsNullOrEmpty(value))
                {
                    Value = DateTime.UtcNow;
                }
                else if (DateTime.TryParseExact(value, DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    Value = date;
                }

                //purposely not set if parse fails
            }
        }

        public XElement Xml()
        {
            return new XElement("date", StringValue);
        }

        public IPListElement Copy()
        {
            return new PListDate(Value);
        }

        public override string ToString()
        {
            return StringValue;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PListDate);
        }

        public bool Equals(PListDate element)
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