// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class HealthKitCapability : BaseCapability
    {
        public HealthKitCapability()
        {
        }

        public HealthKitCapability(PListDictionary dic)
        {
        }

        public HealthKitCapability(HealthKitCapability other)
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
            return new HealthKitCapability(this);
        }

        #endregion
    }
}

