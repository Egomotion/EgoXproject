// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class MultipathCapability : BaseCapability
    {
        public MultipathCapability ()
        {
        }

        public MultipathCapability (PListDictionary dic)
        {
        }

        public MultipathCapability (MultipathCapability other)
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
            return new MultipathCapability (this);
        }

        #endregion
    }
}
