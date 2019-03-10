//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal static class ProjectUtil
    {

        public static string ProjectPath
        {
            get
            {
                return Path.GetDirectoryName(Application.dataPath);
            }
        }

        public static string MakePathRelativeToProject(string path)
        {
            return PathUtil.MakePathRelativeToRootPath(path, ProjectPath);
        }

        public static string MakePathRelativeToAssets(string path)
        {
            return PathUtil.MakePathRelativeToRootPath(path, Application.dataPath);
        }
    }
}
