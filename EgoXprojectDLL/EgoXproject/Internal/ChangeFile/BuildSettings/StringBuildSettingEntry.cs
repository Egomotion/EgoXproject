// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

namespace Egomotion.EgoXproject.Internal
{
    internal class StringBuildSettingEntry : BaseBuildSettingEntry
    {
        const string VALUE_KEY = "Value";
        string _value = "";


        public StringBuildSettingEntry(string name, string value = "")
        : base(name)
        {
            Value = value;
        }

        public StringBuildSettingEntry(PListDictionary dic)
        : base(dic)
        {
            Value = dic.StringValue(VALUE_KEY);
        }

        public StringBuildSettingEntry(StringBuildSettingEntry other)
        : base(other)
        {
            Value = other.Value;
        }

        #region implemented abstract members of BaseChangeEntry

        public override PListDictionary Serialize()
        {
            var dic = base.Serialize();
            dic.Add(VALUE_KEY, Value);
            return dic;
        }

        #endregion

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
                    _value = value.Trim();
                }
            }
        }
    }
}