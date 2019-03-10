// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class GameCenterCapability : BaseCapability
    {
        public GameCenterCapability()
        {
        }

        public GameCenterCapability(PListDictionary dic)
        {
        }

        public GameCenterCapability(GameCenterCapability other)
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
            return new GameCenterCapability(this);
        }
        #endregion
    }
}

