//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal abstract class PBXBaseBuildPhase : PBXBaseObject
    {
        public const string DEFAULT_BUILD_ACTION_MASK = "2147483647";
        const string BUILD_ACTION_MASK_KEY = "buildActionMask";
        const string FILES_KEY = "files";
        const string RUN_ONLY_FOR_DEPLOYMENT_POSTPROCESSING_KEY = "runOnlyForDeploymentPostprocessing";

        List<PBXBuildFile> _buildFiles = new List<PBXBuildFile>();

        //TODO could this reference a variant group?
        protected PBXBaseBuildPhase(PBXTypes isa, string uid, PBXProjDictionary dict)
        : base(isa, uid, dict)
        {
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            _buildFiles = PopulateObjects<PBXBuildFile>(FileIDs, allObjects);
        }

        #endregion

        protected static void PopulateEmptyDictionary(PBXProjDictionary emptyDic)
        {
            emptyDic.Add(BUILD_ACTION_MASK_KEY, DEFAULT_BUILD_ACTION_MASK);
            emptyDic.Add(FILES_KEY, new PBXProjArray());
            emptyDic.Add(RUN_ONLY_FOR_DEPLOYMENT_POSTPROCESSING_KEY, "0");
        }

        public string[] FileIDs
        {
            get
            {
                return Dict.ArrayValue(FILES_KEY).ToStringArray();
            }
        }

        public void AddFile(PBXBuildFile buildFile)
        {
            if (buildFile == null)
            {
                return;
            }

            if (FileIDs.Contains(buildFile.UID))
            {
                return;
            }

            FileUIDs.Add(buildFile.UID);
            _buildFiles.Add(buildFile);
        }

        public void RemoveFile(PBXBuildFile file)
        {
            if (file == null)
            {
                return;
            }

            RemoveFile(file.UID);
        }

        public void RemoveFile(string fileUID)
        {
            var files = FileUIDs;
            var uidsToRemove = files.Where(o => (o is PBXProjString) && (o as PBXProjString).Value == fileUID);

            foreach (var r in uidsToRemove)
            {
                files.Remove(r);
            }

            var filesToRemove = Files.Where(o => o.UID == fileUID);

            foreach (var r in filesToRemove)
            {
                _buildFiles.Remove(r);
            }
        }

        public string BuildActionMask
        {
            get
            {
                return Dict.StringValue(BUILD_ACTION_MASK_KEY);
            }
            set
            {
                Dict[BUILD_ACTION_MASK_KEY] = new PBXProjString(value);
            }
        }

        public string RunOnlyForDeploymentPostprocessing
        {
            get
            {
                return Dict.StringValue(RUN_ONLY_FOR_DEPLOYMENT_POSTPROCESSING_KEY);
            }
            set
            {
                Dict[RUN_ONLY_FOR_DEPLOYMENT_POSTPROCESSING_KEY] = new PBXProjString(value);
            }
        }

        public PBXBuildFile[] Files
        {
            get
            {
                return _buildFiles.ToArray();
            }
        }

        PBXProjArray FileUIDs
        {
            get
            {
                return Dict.ArrayValue(FILES_KEY);
            }
        }
    }
}
