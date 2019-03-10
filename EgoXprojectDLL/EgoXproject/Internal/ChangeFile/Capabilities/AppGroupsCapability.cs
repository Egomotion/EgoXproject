// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class AppGroupsCapability : BaseCapability
    {
        const string APP_GROUPS_KEY = "AppGroups";

        public List<string> AppGroups
        {
            get;
            private set;
        }

        public AppGroupsCapability()
        {
            AppGroups = new List<string>();
        }

        public AppGroupsCapability(PListDictionary dic)
        {
            var groups = dic.ArrayValue(APP_GROUPS_KEY);

            if (groups != null && groups.Count > 0)
            {
                AppGroups = new List<string>(groups.ToStringArray());
            }
            else
            {
                AppGroups = new List<string>();
            }
        }

        public AppGroupsCapability(AppGroupsCapability other)
        : base (other)
        {
            AppGroups = new List<string>(other.AppGroups);
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();

            if (AppGroups.Count > 0)
            {
                dic.Add(APP_GROUPS_KEY, new PListArray(AppGroups));
            }

            return dic;
        }

        public override BaseCapability Clone()
        {
            return new AppGroupsCapability(this);
        }

        #endregion
    }
}

