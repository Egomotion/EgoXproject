// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal abstract class BaseCapability
    {
        protected BaseCapability()
        {
        }

        protected BaseCapability(BaseCapability other)
        {
            if (other == null)
            {
                throw new System.ArgumentNullException(nameof(other), "BaseCapability cannot be null");
            }
        }

        public abstract PListDictionary Serialize();

        public abstract BaseCapability Clone();
    }
}

