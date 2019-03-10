// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXHeadersBuildPhase : PBXBaseBuildPhase
    {
        public PBXHeadersBuildPhase(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXHeadersBuildPhase, uid, dict)
        {
        }

        public static PBXHeadersBuildPhase Create(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException(nameof (uid), "uid cannot be null or empty");
            }

            PBXProjDictionary emptyDic = new PBXProjDictionary();
            emptyDic.Add(isaKey, PBXTypes.PBXHeadersBuildPhase.ToString());
            PopulateEmptyDictionary(emptyDic);
            return new PBXHeadersBuildPhase(uid, emptyDic);
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            base.Populate(allObjects);
        }

        #endregion


    }
}

