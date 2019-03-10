// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class GameControllersCapability : BaseCapability
    {
        public enum GameControllerType { ExtendedGamepad, MicroGamepad };

        const string SUPPORTED_GAME_CONTROLLERS_KEY = "SupportedGameControllers";

        public GameControllersCapability ()
        {
        }

        public GameControllersCapability (PListDictionary dic)
        {
            var controllers = dic.ArrayValue (SUPPORTED_GAME_CONTROLLERS_KEY);

            if (controllers != null && controllers.Count > 0)
            {
                var gc = new List<GameControllerType>();

                for (int ii = 0; ii < controllers.Count; ii++)
                {
                    GameControllerType c;

                    if (controllers.EnumValue (ii, out c))
                    {
                        gc.Add (c);
                    }
                }

                GameControllers = gc.ToArray ();
            }
        }

        public GameControllersCapability (GameControllersCapability other)
        : base (other)
        {
            if (other.GameControllers == null || other.GameControllers.Length <= 0)
            {
                GameControllers = null;
            }
            else
            {
                System.Array.Copy (other.GameControllers, GameControllers, other.GameControllers.Length);
            }
        }

        #region implemented abstract members of BaseCapability
        public override PListDictionary Serialize ()
        {
            var dic = new PListDictionary ();

            if (GameControllers != null && GameControllers.Length > 0)
            {
                var controllers = new PListArray ();

                foreach (var c in GameControllers)
                {
                    controllers.Add (c.ToString ());
                }

                dic.Add (SUPPORTED_GAME_CONTROLLERS_KEY, controllers);
            }

            return dic;
        }

        public override BaseCapability Clone ()
        {
            return new GameControllersCapability (this);
        }

        #endregion

        public GameControllerType [] GameControllers
        {
            get;
            set;
        }

    }
}
