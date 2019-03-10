// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//

using UnityEditor;
using UnityEngine;

using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal static class XcodeFinder
    {
        public const string DEFAULT_XCODE_PATH = "/Applications/Xcode.app";
        const string DEFAULT_IOS_PLATFORM_SUBPATH = "Contents/Developer/Platforms/iPhoneOS.platform";
        const string CUSTOM_XCODE_KEY = "uk.co.egomotion.egoXproject.XcodeLocation";

        public static void FindXcode()
        {
            if (FindXcode(XcodeLocation))
            {
                _isFound = true;
            }
            else if (FindXcode(DEFAULT_XCODE_PATH))
            {
                _isFound = true;
            }
            else
            {
                _isFound = false;
            }
        }

        static bool _isFound = false;
        static bool _firstRun = true;

        public static bool IsFound
        {
            get
            {
                if (_firstRun)
                {
                    _firstRun = false;
                    FindXcode();
                }

                return _isFound;
            }
        }

        public static bool UsingCustomLocation
        {
            get
            {
                return !string.IsNullOrEmpty(EditorPrefs.GetString(CUSTOM_XCODE_KEY, ""));
            }
        }

        public static string XcodeLocation
        {
            get
            {
                var custom = EditorPrefs.GetString(CUSTOM_XCODE_KEY, "");

                if (string.IsNullOrEmpty(custom))
                {
                    return DEFAULT_XCODE_PATH;
                }

                return custom;
            }
        }

        public static bool SetXcodeLocation(string path)
        {
            if (FindXcode(path))
            {
                _isFound = true;
                EditorPrefs.SetString(CUSTOM_XCODE_KEY, path);
                return true;
            }

            return false;
        }

        public static void ClearCustomXcodeLocation()
        {
            EditorPrefs.DeleteKey(CUSTOM_XCODE_KEY);
            _isFound = FindXcode(DEFAULT_XCODE_PATH);
        }

        static bool FindXcode(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                return false;
            }

            //windows & linux
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                return false;
            }

            var sdkPath = Path.Combine(location, DEFAULT_IOS_PLATFORM_SUBPATH);

            if (!Directory.Exists(sdkPath))
            {
                return false;
            }

            return true;
        }

    }
}
