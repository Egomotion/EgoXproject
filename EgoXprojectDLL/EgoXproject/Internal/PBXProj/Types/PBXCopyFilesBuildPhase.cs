//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXCopyFilesBuildPhase : PBXBaseBuildPhase
    {
        public abstract class CopyDestination
        {
            public const string ABSOLUTE_PATH = "0";
            public const string PRODUCTS_DIRECTORY = "16";
            public const string WRAPPER = "1";
            public const string EXECUTABLES = "6";
            public const string RESOURCES = "7";
            public const string JAVA_RESOURCES = "15";
            public const string FRAMEWORKS = "10";
            public const string SHARED_FRAMEWORKS = "11";
            public const string SHARED_SUPPORT = "12";
            public const string PLUGINS = "13";
            public const string XPC_SERVICES = "16";    //this also sets dstPath = "$(CONTENTS_FOLDER_PATH)/XPCServices
        }

        const string DST_PATH_KEY = "dstPath";
        const string DST_SUB_FOLDER_SPEC_KEY = "dstSubfolderSpec";
        const string NAME_KEY = "name";

        public PBXCopyFilesBuildPhase(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXCopyFilesBuildPhase, uid, dict)
        {
        }

        public static PBXCopyFilesBuildPhase Create(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException(nameof (uid), "uid cannot be null or empty");
            }

            PBXProjDictionary emptyDic = new PBXProjDictionary();
            emptyDic.Add(isaKey, PBXTypes.PBXCopyFilesBuildPhase.ToString());
            PopulateEmptyDictionary(emptyDic);
            return new PBXCopyFilesBuildPhase(uid, emptyDic);
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            base.Populate(allObjects);
        }

        #endregion

        public string DstPath
        {
            get
            {
                return Dict.StringValue(DST_PATH_KEY);
            }
            set
            {
                Dict[DST_PATH_KEY] = new PBXProjString(value);
            }
        }

        public string DstSubfolderSpec
        {
            get
            {
                return Dict.StringValue(DST_SUB_FOLDER_SPEC_KEY);
            }
            set
            {
                Dict[DST_SUB_FOLDER_SPEC_KEY] = new PBXProjString(value);
            }
        }

        public string Name
        {
            get
            {
                return Dict.StringValue(NAME_KEY).FromLiteral();
            }
            set
            {
                Dict[NAME_KEY] = new PBXProjString(value.ToLiteralIfRequired());
            }
        }
    }
}
