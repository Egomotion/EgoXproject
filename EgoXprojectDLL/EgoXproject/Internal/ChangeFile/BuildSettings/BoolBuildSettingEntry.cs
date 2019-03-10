// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

namespace Egomotion.EgoXproject.Internal
{
    internal class BoolBuildSettingEntry : BaseBuildSettingEntry
    {
        const string VALUE_KEY = "Value";

        public bool Value
        {
            get;
            set;
        }

        public BoolBuildSettingEntry(string name, bool value)
        : base(name)
        {
            Value = value;
        }

        public BoolBuildSettingEntry(PListDictionary dic)
        : base(dic)
        {
            string s = dic.StringValue(VALUE_KEY);
            Value = (s == XcodeBool.YES);
        }

        public BoolBuildSettingEntry(BoolBuildSettingEntry other)
        : base(other)
        {
            Value = other.Value;
        }

        #region implemented abstract members of BaseChangeEntry

        public override PListDictionary Serialize()
        {
            var dic = base.Serialize();
            dic.Add(VALUE_KEY, Value ? XcodeBool.YES : XcodeBool.NO);
            return dic;
        }

        #endregion
    }
}

