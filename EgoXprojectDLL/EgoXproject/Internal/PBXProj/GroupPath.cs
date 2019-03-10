// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.IO;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    class GroupPath
    {
        public string RootPath
        {
            get;
            private set;
        }

        // will either point to a folder in the project or outside of the project. 
        // eg MyFolder or ../MyFolder/TargetFolder or ../../MyFolder etc.
        public string SubPath
        {
            get;
            private set;
        }

        // the current path for the remaining sub groups. empty if none, or 
        // eg. FolderA or FolderA/FolderB etc
        public GroupPath()
        : this("")
        {
        }

        public GroupPath(string rootPath)
        {
            if (rootPath == null)
            {
                RootPath = "";
            }
            else
            {
                RootPath = rootPath;
            }

            SubPath = "";
        }

        public GroupPath(GroupPath other)
        {
            RootPath = other.RootPath;
            SubPath = other.SubPath;
        }

        public void AddComponentToSubPath(string pathComponent)
        {
            SubPath = Path.Combine(SubPath, pathComponent);
        }

        public void SetSubPathFromRelativePath(string relativePath)
        {
            if (!relativePath.StartsWith(RootPath, System.StringComparison.InvariantCultureIgnoreCase))
            {
                throw new System.ArgumentException("relativePath must start with the root path", nameof (relativePath));
            }

            var p = relativePath.Remove(0, RootPath.Length);

            if (p.StartsWith("/", System.StringComparison.InvariantCultureIgnoreCase))
            {
                p = p.Remove(0, 1);
            }

            SubPath = p;
        }

        // Get the path components, eg ../MyFolder/TargetFolder/FolderA/FolderB 
        // would be [../MyFolder/TargetFolder, FolderA, FolderB]
        public string[] PathComponents
        {
            get
            {
                var components = new List<string>();

                if (!string.IsNullOrEmpty(RootPath))
                {
                    components.Add(RootPath);
                }

                if (!string.IsNullOrEmpty(SubPath))
                {
                    components.AddRange(PathUtil.ComponentsFromPath(SubPath));
                }

                return components.ToArray();
            }
        }

        public string RelativePath
        {
            get
            {
                return Path.Combine(RootPath, SubPath);
            }
        }


    }
}

