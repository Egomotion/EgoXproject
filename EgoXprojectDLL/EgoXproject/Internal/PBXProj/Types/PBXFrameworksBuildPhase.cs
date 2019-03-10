//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXFrameworksBuildPhase : PBXBaseBuildPhase
    {
        public PBXFrameworksBuildPhase(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXFrameworksBuildPhase, uid, dict)
        {
        }

        public static PBXFrameworksBuildPhase Create(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException(nameof (uid), "uid cannot be null or empty");
            }

            PBXProjDictionary emptyDic = new PBXProjDictionary();
            emptyDic.Add(isaKey, PBXTypes.PBXFrameworksBuildPhase.ToString());
            PBXBaseBuildPhase.PopulateEmptyDictionary(emptyDic);
            return new PBXFrameworksBuildPhase(uid, emptyDic);
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            base.Populate(allObjects);
        }

        #endregion

    }
}
