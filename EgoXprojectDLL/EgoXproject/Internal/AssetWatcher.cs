//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

namespace Egomotion.EgoXproject.Internal
{
    internal class AssetWatcher : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool refreshChangeFile = false;

            if (CheckForMatches(importedAssets))
            {
                refreshChangeFile = true;
            }
            else if (CheckForMatches(deletedAssets))
            {
                refreshChangeFile = true;
            }
            else if (CheckForMatches(movedAssets))
            {
                refreshChangeFile = true;
            }
            else if (CheckForMatches(movedFromAssetPaths))
            {
                refreshChangeFile = true;
            }

            if (refreshChangeFile)
            {
                XcodeController.Instance().Refresh();
            }
        }

        static bool CheckForMatches(string[] paths)
        {
            foreach (var path in paths)
            {
                if (Path.GetExtension(path).EndsWith(XcodeChangeFile.Extension, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

    }

}