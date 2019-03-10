// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class HotspotConfigurationCapability : BaseCapability
    {
        public HotspotConfigurationCapability ()
        {
        }

        public HotspotConfigurationCapability (PListDictionary dic)
        {
        }

        public HotspotConfigurationCapability (HotspotConfigurationCapability other)
        : base (other)
        {
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize ()
        {
            var dic = new PListDictionary ();
            return dic;
        }

        public override BaseCapability Clone ()
        {
            return new HotspotConfigurationCapability (this);
        }

        #endregion
    }
}
