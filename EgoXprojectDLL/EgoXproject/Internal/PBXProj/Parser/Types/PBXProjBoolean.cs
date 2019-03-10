//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System.Collections;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXProjBoolean : IPBXProjExpression
    {
        bool _value;

        public PBXProjBoolean(bool value)
        {
            Value = value;
        }

        public bool Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public override string ToString()
        {
            return _value ? XcodeBool.YES : XcodeBool.NO;
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
