//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXResourcesBuildPhase : PBXBaseBuildPhase
    {
        public PBXResourcesBuildPhase(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXResourcesBuildPhase, uid, dict)
        {
        }

        public static PBXResourcesBuildPhase Create(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException(nameof (uid), "uid cannot be null or empty");
            }

            PBXProjDictionary emptyDic = new PBXProjDictionary();
            emptyDic.Add(isaKey, PBXTypes.PBXResourcesBuildPhase.ToString());
            PopulateEmptyDictionary(emptyDic);
            return new PBXResourcesBuildPhase(uid, emptyDic);
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            base.Populate(allObjects);
        }

        #endregion

    }
}
