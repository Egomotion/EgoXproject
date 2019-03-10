// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class InterAppAudioCapability : BaseCapability
    {
        public InterAppAudioCapability()
        {
        }

        public InterAppAudioCapability(PListDictionary dic)
        {
        }

        public InterAppAudioCapability(InterAppAudioCapability other)
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
            return new InterAppAudioCapability(this);
        }

        #endregion
    }
}

