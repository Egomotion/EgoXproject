// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class ApplePayCapability : BaseCapability
    {
        const string MERCHANT_IDS_KEY = "MerchantIds";

        public List<string> MerchantIds
        {
            get;
            private set;
        }

        public ApplePayCapability()
        {
            MerchantIds = new List<string>();
        }

        public ApplePayCapability(PListDictionary dic)
        {
            var groups = dic.ArrayValue(MERCHANT_IDS_KEY);

            if (groups != null && groups.Count > 0)
            {
                MerchantIds = new List<string>(groups.ToStringArray());
            }
            else
            {
                MerchantIds = new List<string>();
            }
        }

        public ApplePayCapability(ApplePayCapability other)
        : base (other)
        {
            MerchantIds = new List<string>(other.MerchantIds);
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();

            if (MerchantIds.Count > 0)
            {
                dic.Add(MERCHANT_IDS_KEY, new PListArray(MerchantIds));
            }

            return dic;
        }

        public override BaseCapability Clone()
        {
            return new ApplePayCapability(this);
        }

        #endregion
    }
}

