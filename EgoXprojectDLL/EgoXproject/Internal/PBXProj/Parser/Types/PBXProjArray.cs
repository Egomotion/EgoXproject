//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXProjArray : List<IPBXProjExpression>, IPBXProjExpression
    {
        public override string ToString()
        {
            string s = "(\n";

            foreach (IPBXProjExpression e in this)
            {
                s += "\t" + e.ToString() + ",\n";
            }

            s += ")";
            return s;
        }

        public string ToStringWithComment()
        {
            string s = "(\n";

            foreach (IPBXProjExpression e in this)
            {
                s += "\t" + e.ToStringWithComment() + ",\n";
            }

            s += ")";

            if (!string.IsNullOrEmpty(Comment))
            {
                s += " " + Comment;
            }

            return s;
        }

        public string ToString(int indentLevel)
        {
            string tabs = "";

            for (int ii = 0; ii < indentLevel; ++ii)
            {
                tabs += "\t";
            }

            string tabsPlus1 = tabs + "\t";
            string s = "(\n";

            foreach (IPBXProjExpression e in this)
            {
                s += tabsPlus1 + e.ToStringWithComment() + ",\n";
            }

            s += tabs + ")";

            if (!string.IsNullOrEmpty(Comment))
            {
                s += " " + Comment;
            }

            return s;
        }

        public T Element<T>(int index) where T : class, IPBXProjExpression
        {
            return this[index] as T;
        }

        public string ToInlineString()
        {
            string s = "(";

            foreach (IPBXProjExpression e in this)
            {
                if (e is PBXProjDictionary)
                {
                    s += (e as PBXProjDictionary).ToInlineString();
                }
                else if (e is PBXProjArray)
                {
                    s += (e as PBXProjArray).ToInlineString();
                }
                else
                {
                    s += e.ToStringWithComment();
                }

                s += ", ";
            }

            s += ")";

            if (!string.IsNullOrEmpty(Comment))
            {
                s += " " + Comment;
            }

            return s;
        }

        public void Add(string value)
        {
            Add(new PBXProjString(value));
        }

        public void Add(int value)
        {
            Add(value.ToString());
        }

        public void Add(bool value)
        {
            Add(new PBXProjBoolean(value));
        }

        public string StringValue(int index)
        {
            var value = Element<PBXProjString>(index);

            if (value == null)
            {
                return "";
            }

            return value.Value;
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

        public string Comment
        {
            get;
            set;
        }
    }
}
