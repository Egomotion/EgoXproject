//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXProjDictionary : Dictionary<string, IPBXProjExpression>, IPBXProjExpression
    {
        Dictionary<string, string> _keyComment = new Dictionary<string, string>();
        Dictionary<string, string> _preComment = new Dictionary<string, string>();
        Dictionary<string, string> _postComment = new Dictionary<string, string>();

        public T Element<T>(string key) where T : class, IPBXProjExpression
        {
            IPBXProjExpression value = null;

            if (this.TryGetValue(key, out value))
            {
                return value as T;
            }
            else
            {
                return null;
            }
        }

        public void SetCommentForKey(string key, string comment)
        {
            _keyComment[key] = comment;
        }

        public string GetCommentForKey(string key)
        {
            string value = "";
            _keyComment.TryGetValue(key, out value);
            return value;
        }

        public void SetPreCommentForKey(string key, string comment)
        {
            _preComment[key] = comment;
        }

        public string GetPreCommentForKey(string key)
        {
            string value = "";
            _preComment.TryGetValue(key, out value);
            return value;
        }

        public void SetPostCommentForKey(string key, string comment)
        {
            _postComment[key] = comment;
        }

        public string GetPostCommentForKey(string key)
        {
            string value = "";
            _postComment.TryGetValue(key, out value);
            return value;
        }

        public override string ToString()
        {
            string s = "{\n";

            foreach (var kvp in this)
            {
                s += "\t" + kvp.Key + " = " + kvp.Value + ";\n";
            }

            s += "}";
            return s;
        }

        public enum PrintStyle
        {
            Inline,
            Indented
        };

        public string KeyValueToString(string key, int indentLevel, PrintStyle style)
        {
            string output = "";
            IPBXProjExpression value;

            if (!TryGetValue(key, out value))
            {
                return output;
            }

            string preComment = GetPreCommentForKey(key);

            if (!string.IsNullOrEmpty(preComment))
            {
                output += "\n" + preComment + "\n";
            }

            string tabs = "";

            for (int ii = 0; ii < indentLevel; ++ii)
            {
                tabs += "\t";
            }

            output += tabs + key;
            string keyComment = GetCommentForKey(key);

            if (!string.IsNullOrEmpty(keyComment))
            {
                output += " " + keyComment;
            }

            output += " = ";

            switch (style)
            {
            case PrintStyle.Indented:
                output += IndentedEntry(value, indentLevel);
                break;

            case PrintStyle.Inline:
            default:
                output += InlineEntry(value);
                break;
            }

            output += ";\n";
            string postComment = GetPostCommentForKey(key);

            if (!string.IsNullOrEmpty(postComment))
            {
                output += postComment + "\n";
            }

            return output;
        }

        string InlineEntry(IPBXProjExpression value)
        {
            string output = "";

            if (value is PBXProjDictionary)
            {
                output += (value as PBXProjDictionary).ToInlineString();
            }
            else if (value is PBXProjArray)
            {
                output += (value as PBXProjArray).ToInlineString();
            }
            else
            {
                value.ToStringWithComment();
            }

            return output;
        }

        string IndentedEntry(IPBXProjExpression value, int indentLevel)
        {
            string output = "";

            if (value is PBXProjDictionary)
            {
                var dic = value as PBXProjDictionary;
                output += "{\n";

                foreach (var key in dic.Keys)
                {
                    output += dic.KeyValueToString(key, indentLevel + 1, PrintStyle.Indented);
                }

                for (int ii = 0; ii < indentLevel; ++ii)
                {
                    output += "\t";
                }

                output += "}";
            }
            else if (value is PBXProjArray)
            {
                output += (value as PBXProjArray).ToString(indentLevel);
            }
            else
            {
                output += value.ToStringWithComment();
            }

            return output;
        }

        public string ToInlineString()
        {
            string s = "{";

            foreach (var kvp in this)
            {
                s += kvp.Key + " = ";

                if (kvp.Value is PBXProjDictionary)
                {
                    s += (kvp.Value as PBXProjDictionary).ToInlineString();
                }
                else if (kvp.Value is PBXProjArray)
                {
                    s += (kvp.Value as PBXProjArray).ToInlineString();
                }
                else
                {
                    s += kvp.Value.ToStringWithComment();
                }

                s += "; ";
            }

            s += "}";
            return s;
        }

        public void Add(string key, string value)
        {
            Add(key, new PBXProjString(value));
        }

        public void Add(string key, int value)
        {
            Add(key, value.ToString());
        }

        public void Add(string key, bool value)
        {
            Add(key, new PBXProjBoolean(value));
        }

        public string StringValue(string key)
        {
            var value = Element<PBXProjString>(key);

            if (value == null)
            {
                return "";
            }

            return value.Value;
        }

        public bool BoolValue(string key)
        {
            var value = Element<PBXProjBoolean>(key);

            if (value == null)
            {
                return false;
            }

            return value.Value;
        }

        public PBXProjDictionary DictionaryValue(string key)
        {
            return Element<PBXProjDictionary>(key);
        }

        //TODO potential issue - single array entry may be parsed as a string, causing this to return null;
        public PBXProjArray ArrayValue(string key)
        {
            return Element<PBXProjArray>(key);
        }

        #region IPBXProjExpression implementation

        public string ToStringWithComment()
        {
            string s = "{\n";

            foreach (var kvp in this)
            {
                s += "\t" + kvp.Key + " = " + kvp.Value.ToStringWithComment() + ";\n";
            }

            s += "}";

            if (!string.IsNullOrEmpty(Comment))
            {
                s += " " + Comment;
            }

            return s;
        }

        public string Comment
        {
            get;
            set;
        }


        #endregion

    }
}
