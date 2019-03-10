// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class SiriCapability : BaseCapability
    {
        public SiriCapability()
        {
        }

        public SiriCapability(PListDictionary dic)
        {
        }

        public SiriCapability(SiriCapability other)
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
            return new SiriCapability(this);
        }

        #endregion
    }
}

