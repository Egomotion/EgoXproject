//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXShellScriptBuildPhase : PBXBaseBuildPhase
    {
        const string NAME_KEY = "name";
        const string INPUT_PATH_KEY = "inputPaths";
        const string OUTPUT_PATHS_KEY = "outputPaths";
        const string SHELL_PATH_KEY = "shellPath";
        const string SHELL_SCRIPT_KEY = "shellScript";

        const string DEFAULT_SHELL = "/bin/sh";

        public PBXShellScriptBuildPhase(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXShellScriptBuildPhase, uid, dict)
        {
        }

        public static PBXShellScriptBuildPhase Create(string uid, string name, string script, string shell)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException(nameof (uid), "uid cannot be null or empty");
            }

            PBXProjDictionary emptyDic = new PBXProjDictionary();
            emptyDic.Add(isaKey, PBXTypes.PBXShellScriptBuildPhase.ToString());
            PBXBaseBuildPhase.PopulateEmptyDictionary(emptyDic);
            emptyDic.Add(INPUT_PATH_KEY, new PBXProjArray());
            emptyDic.Add(OUTPUT_PATHS_KEY, new PBXProjArray());
            emptyDic.Add(SHELL_PATH_KEY, DEFAULT_SHELL);
            emptyDic.Add(SHELL_SCRIPT_KEY, "\"\"");
            var buildPhase = new PBXShellScriptBuildPhase(uid, emptyDic);

            if (!string.IsNullOrEmpty(name))
            {
                buildPhase.Name = name;
            }

            buildPhase.ShellPath = shell;
            buildPhase.Script = script;
            return buildPhase;
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            base.Populate(allObjects);
        }

        #endregion


        public string Script
        {
            get
            {
                return Dict.StringValue(SHELL_SCRIPT_KEY).FromLiteral();
            }
            set
            {
                var s = value.ToLiteral();
                Dict[SHELL_SCRIPT_KEY] = new PBXProjString(s);
            }
        }

        public string ShellPath
        {
            get
            {
                return Dict.Element<PBXProjString>(SHELL_PATH_KEY).Value;
            }
            set
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    value = DEFAULT_SHELL;
                }

                Dict[SHELL_PATH_KEY] = new PBXProjString(value);
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

        //TODO the other options
    }
}
