// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal abstract class BaseBuildSettingEntry : BaseChangeEntry
    {
        public const string NAME_KEY = "Name";

        public string Name
        {
            get;
            protected set;
        }

        protected BaseBuildSettingEntry(string name)
        {
            Name = name;
        }

        protected BaseBuildSettingEntry(PListDictionary dic)
        {
            if (dic == null)
            {
                throw new System.ArgumentNullException(nameof (dic), "Dictionary cannot be null");
            }

            if (!dic.ContainsKey(NAME_KEY))
            {
                throw new System.ArgumentException("No name key in dictionary");
            }

            Name = dic.StringValue(NAME_KEY);
        }

        protected BaseBuildSettingEntry(BaseBuildSettingEntry other)
        {
            Name = other.Name;
        }

        #region implemented abstract members of BaseChangeEntry

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.Add(NAME_KEY, Name);
            return dic;
        }

        #endregion
    }
}

