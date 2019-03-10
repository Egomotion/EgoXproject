// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class InAppPurchaseCapability : BaseCapability
    {
        public InAppPurchaseCapability()
        {
        }

        public InAppPurchaseCapability(PListDictionary dic)
        {
        }

        public InAppPurchaseCapability(InAppPurchaseCapability other)
        : base (other)
        {
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            return dic;
        }

        public override BaseCapability Clone()
        {
            return new InAppPurchaseCapability(this);
        }

        #endregion
    }
}

