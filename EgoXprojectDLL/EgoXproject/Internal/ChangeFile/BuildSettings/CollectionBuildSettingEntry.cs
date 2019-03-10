// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class CollectionBuildSettingEntry : BaseBuildSettingEntry
    {
        const string OLD_VALUE_KEY = "Values";
        const string VALUE_KEY = "Value";
        const string MERGE_KEY = "Merge";

        public List<string> Values
        {
            get;
            set;
        }

        public MergeMethod Merge
        {
            get;
            set;
        }

        public CollectionBuildSettingEntry(string name)
        : base(name)
        {
            Values = new List<string>();
        }

        public CollectionBuildSettingEntry(PListDictionary dic)
        : base(dic)
        {
            //try to get array with new key
            var array = dic.ArrayValue(VALUE_KEY);

            //then try with old key
            if (array == null)
            {
                array = dic.ArrayValue(OLD_VALUE_KEY);
            }

            //get the values
            if (array != null)
            {
                Values = new List<string>();
                Values.AddRange(array.ToStringArray());
            }
            //if all failed see if it is a string (could be a custom string that is now known about.
            else
            {
                var strVal = dic.StringValue(VALUE_KEY);
                Values = StringUtils.StringListToList(strVal);
            }

            MergeMethod m;

            if (dic.EnumValue (MERGE_KEY, out m))
            {
                Merge = m;
            }
            else
            {
                Merge = MergeMethod.Append;
            }
        }

        public CollectionBuildSettingEntry(CollectionBuildSettingEntry other)
        : base(other)
        {
            Values = new List<string>(other.Values);
            Merge = other.Merge;
        }

        #region implemented abstract members of BaseChangeEntry

        public override PListDictionary Serialize()
        {
            //remove any extra spaces
            for (int ii = 0; ii < Values.Count; ++ii)
            {
                Values[ii] = Values[ii].Trim();
            }

            var dic = base.Serialize();
            dic.Add(VALUE_KEY, new PListArray(Values));
            dic.Add(MERGE_KEY, Merge.ToString());
            return dic;
        }

        #endregion
    }
}