//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXProjString : IPBXProjExpression
    {
        protected string _value = "";

        public PBXProjString(string value = "")
        {
            Value = value;
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _value = "";
                }
                else
                {
                    _value = value;
                }
            }
        }

        public override string ToString()
        {
            return _value;
        }

        public string Comment
        {
            get;
            set;
        }

        public string ToStringWithComment()
        {
            if (string.IsNullOrEmpty(Comment))
            {
                return ToString();
            }
            else
            {
                return ToString() + " " + Comment;
            }
        }
    }
}

