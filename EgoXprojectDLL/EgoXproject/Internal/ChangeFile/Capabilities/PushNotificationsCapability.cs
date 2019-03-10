// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class PushNotificationsCapability : BaseCapability
    {
        public PushNotificationsCapability()
        {
        }

        public PushNotificationsCapability(PListDictionary dic)
        {
        }

        public PushNotificationsCapability(PushNotificationsCapability other)
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
            return new PushNotificationsCapability(this);
        }

        #endregion
    }
}

