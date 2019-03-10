// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class WalletCapability : BaseCapability
    {
        const string ALLOW_SUBSET_OF_PASS_TYPES_KEY = "AllowSubsetOfPassTypes";
        const string PASS_TYPE_SUBSETS_KEY = "PassTypeSubsets";

        public List<string> PassTypeSubsets
        {
            get;
            private set;
        }

        public WalletCapability()
        {
            PassTypeSubsets = new List<string>();
        }

        public WalletCapability(PListDictionary dic)
        {
            AllowSubsetOfPassTypes = dic.BoolValue(ALLOW_SUBSET_OF_PASS_TYPES_KEY);
            var groups = dic.ArrayValue(PASS_TYPE_SUBSETS_KEY);

            if (groups != null && groups.Count > 0)
            {
                PassTypeSubsets = new List<string>(groups.ToStringArray());
            }
            else
            {
                PassTypeSubsets = new List<string>();
            }
        }

        public WalletCapability(WalletCapability other)
        : base (other)
        {
            AllowSubsetOfPassTypes = other.AllowSubsetOfPassTypes;
            PassTypeSubsets = new List<string>(other.PassTypeSubsets);
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.AddIfTrue(ALLOW_SUBSET_OF_PASS_TYPES_KEY, AllowSubsetOfPassTypes);

            if (PassTypeSubsets.Count > 0)
            {
                dic.Add(PASS_TYPE_SUBSETS_KEY, new PListArray(PassTypeSubsets));
            }

            return dic;
        }

        public override BaseCapability Clone()
        {
            return new WalletCapability(this);
        }

        #endregion

        public bool AllowSubsetOfPassTypes
        {
            get;
            set;
        }
    }
}
