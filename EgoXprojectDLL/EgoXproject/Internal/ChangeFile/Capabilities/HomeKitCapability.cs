// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class HomeKitCapability : BaseCapability
    {
        public HomeKitCapability()
        {
        }

        public HomeKitCapability(PListDictionary dic)
        {
        }

        public HomeKitCapability(HomeKitCapability other)
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
            return new HomeKitCapability(this);
        }

        #endregion
    }
}

