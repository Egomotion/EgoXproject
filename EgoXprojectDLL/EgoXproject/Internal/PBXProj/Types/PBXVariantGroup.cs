// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXVariantGroup : PBXGroup
    {
        public PBXVariantGroup(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXVariantGroup, uid, dict)
        {
        }

        public override string FullPath
        {
            get
            {
                if (ParentGroup != null)
                {
                    return ParentGroup.FullPath;
                }
                else
                { return ""; }
            }
        }

    }
}

