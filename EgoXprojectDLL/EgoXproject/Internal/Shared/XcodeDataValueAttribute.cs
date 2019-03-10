//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    [AttributeUsage(AttributeTargets.All)]
    internal class XcodeDataValueAttribute : System.Attribute
    {
        public string Value
        {
            get;
            set;
        }

        public XcodeDataValueAttribute(string value)
        {
            Value = value;
        }
    }
}

