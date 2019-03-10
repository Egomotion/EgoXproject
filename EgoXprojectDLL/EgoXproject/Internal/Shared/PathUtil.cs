// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal static class PathUtil
    {
        public static string[] ComponentsFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new string[] { "" };
            }

            List<string> components = new List<string>();

            do
            {
                var c = Path.GetFileName(path);
                path = Path.GetDirectoryName(path);

                if ((c == ".." || c == ".") && components.Count > 0)
                {
                    int end = components.Count - 1;
                    components[end] = Path.Combine(c, components[end]);
                }
                else
                {
                    components.Add(c);
                }
            }
            while (!string.IsNullOrEmpty(path));

            components.Reverse();
            return components.ToArray();
        }

        public static string PathFromComponents(string[] components)
        {
            string path = "";

            foreach (var c in components)
            {
                path = Path.Combine(path, c);
            }

            return path;
        }

        public static string MakePathRelativeToRootPath(string pathToMakeRelative, string rootPath)
        {
            rootPath = Path.GetFullPath(rootPath);
            pathToMakeRelative = Path.GetFullPath(pathToMakeRelative);
            rootPath = Path.Combine(rootPath, "d");
            System.Uri root = new System.Uri(rootPath, System.UriKind.Absolute);
            System.Uri path = new System.Uri(pathToMakeRelative, System.UriKind.Absolute);
            string relative = root.MakeRelativeUri(path).ToString();
            string unescaped = System.Uri.UnescapeDataString(relative);
            return unescaped;
        }
    }
}
