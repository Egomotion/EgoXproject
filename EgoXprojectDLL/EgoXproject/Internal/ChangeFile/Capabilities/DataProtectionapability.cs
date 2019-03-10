// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class DataProtectionapability : BaseCapability
    {
        public DataProtectionapability()
        {
        }

        public DataProtectionapability(PListDictionary dic)
        {
        }

        public DataProtectionapability(DataProtectionapability other)
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
            return new DataProtectionapability(this);
        }

        #endregion
    }
}

