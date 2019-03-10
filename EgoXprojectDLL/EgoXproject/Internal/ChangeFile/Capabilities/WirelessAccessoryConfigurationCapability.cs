// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class WirelessAccessoryConfigurationCapability : BaseCapability
    {
        public WirelessAccessoryConfigurationCapability()
        {
        }

        public WirelessAccessoryConfigurationCapability(PListDictionary dic)
        {
        }

        public WirelessAccessoryConfigurationCapability(WirelessAccessoryConfigurationCapability other)
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
            return new WirelessAccessoryConfigurationCapability(this);
        }

        #endregion
    }
}

