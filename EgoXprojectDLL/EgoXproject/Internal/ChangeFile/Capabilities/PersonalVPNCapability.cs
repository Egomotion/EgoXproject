// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class PersonalVPNCapability : BaseCapability
    {
        public PersonalVPNCapability()
        {
        }

        public PersonalVPNCapability(PListDictionary dic)
        {
        }

        public PersonalVPNCapability(PersonalVPNCapability other)
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
            return new PersonalVPNCapability(this);
        }

        #endregion
    }
}

