// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//
using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Egomotion.EgoXproject.Internal
{
    internal static class DllUtils
    {
        public static string DllLocation()
        {
            string path = Application.dataPath;
            var asm = System.Reflection.Assembly.GetExecutingAssembly();

            if (asm != null)
            {
                path = Path.GetDirectoryName(asm.Location);
            }
            else
            {
                var allPaths = Directory.GetFiles(Application.dataPath, "EgoXproject.dll", SearchOption.AllDirectories);

                if (allPaths != null && allPaths.Length > 0)
                {
                    path = Path.GetDirectoryName(allPaths[0]);
                }
            }

            return path;
        }

        public static string VersionString()
        {
            string version = "";
            var verObj = Version();

            if (verObj != null)
            {
                version = verObj.ToString(3);
            }

            return version;
        }

        public static System.Version Version()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();

            if (asm != null)
            {
                return asm.GetName().Version;
            }

            return null;
        }

        // > 0 oltherVersion is older, < 0 otherVersion is newer, 0 same version
        public static int CompareVersions(string otherVersion)
        {
            var thisVersion = Version();
            var thatVersion = new System.Version(otherVersion);
            return thisVersion.CompareTo(thatVersion);
        }

        public static bool IsNewer(string otherVersion)
        {
            return CompareVersions(otherVersion) > 0;
        }
    }
}

