// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal abstract class BaseChangeGroup
    {
        protected PListArray Serialize<T>(List<T> changeEntries) where T : BaseChangeEntry
        {
            var array = new PListArray();

            foreach (var item in changeEntries)
            {
                array.Add(item.Serialize());
            }

            return array;
        }

        public abstract IPListElement Serialize();

        public abstract bool HasChanges();

        public abstract void Clear();
    }
}

