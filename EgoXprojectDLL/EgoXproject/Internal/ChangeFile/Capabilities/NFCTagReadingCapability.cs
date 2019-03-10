// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class NFCTagReadingCapability : BaseCapability
    {
        public NFCTagReadingCapability ()
        {
        }

        public NFCTagReadingCapability (PListDictionary dic)
        {
        }

        public NFCTagReadingCapability (NFCTagReadingCapability other)
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
            return new NFCTagReadingCapability (this);
        }

        #endregion
    }
}
