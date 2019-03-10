// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class KeychainSharingCapability : BaseCapability
    {
        const string KEYCHAIN_GROUPS_KEY = "KeychainGroups";

        public List<string> KeychainGroups
        {
            get;
            private set;
        }

        public KeychainSharingCapability()
        {
            KeychainGroups = new List<string>();
        }

        public KeychainSharingCapability(PListDictionary dic)
        {
            var groups = dic.ArrayValue(KEYCHAIN_GROUPS_KEY);

            if (groups != null && groups.Count > 0)
            {
                KeychainGroups = new List<string>(groups.ToStringArray());
            }
            else
            {
                KeychainGroups = new List<string>();
            }
        }

        public KeychainSharingCapability(KeychainSharingCapability other)
        : base (other)
        {
            KeychainGroups = new List<string>(other.KeychainGroups);
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();

            if (KeychainGroups.Count > 0)
            {
                dic.Add(KEYCHAIN_GROUPS_KEY, new PListArray(KeychainGroups));
            }

            return dic;
        }

        public override BaseCapability Clone()
        {
            return new KeychainSharingCapability(this);
        }

        #endregion
    }
}

