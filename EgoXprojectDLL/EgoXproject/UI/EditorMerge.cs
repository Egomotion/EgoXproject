// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using System.IO;

namespace Egomotion.EgoXproject.UI
{
    internal static class EditorMerge
    {
        [MenuItem("Window/EgoXproject/Merge Selected Change Files", false, 20)]
        static void MergeSelection()
        {
            var objects = Selection.objects;

            if (objects == null || objects.Length < 2)
            {
                EditorUtility.DisplayDialog("Merge Error", "You need to select at least 2 EgoXproject change files to merge.", "OK");
                return;
            }

            List<XcodeChangeFile> changeFiles = new List<XcodeChangeFile>();

            foreach (var obj in objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (Path.GetExtension(path) != XcodeChangeFile.Extension)
                {
                    continue;
                }

                XcodeChangeFile changeFile = XcodeChangeFile.Load(path);

                if (changeFile != null)
                {
                    changeFiles.Add(changeFile);
                }
            }

            if (changeFiles.Count < 2)
            {
                EditorUtility.DisplayDialog("Merge Error", "You need to select at least 2 EgoXproject change files to merge.", "OK");
                return;
            }

            XcodeChangeFile mergedFile = new XcodeChangeFile();

            for (int ii = 0; ii < changeFiles.Count; ++ii)
            {
                mergedFile.Merge(changeFiles[ii]);
            }

            string ext = XcodeChangeFile.Extension.Substring(1); //can't have the . in the extension name for unity.
            string savePath = EditorUtility.SaveFilePanelInProject("Save Merged file", "Merged", ext, "Save the merged change file");

            if (!string.IsNullOrEmpty(savePath))
            {
                mergedFile.Save(savePath);
                AssetDatabase.ImportAsset(savePath);
            }
        }

        [MenuItem("Window/EgoXproject/Merge Selected Change Files", true, 20)]
        static bool ValidateMergeSelection()
        {
            var objects = Selection.objects;

            if (objects == null || objects.Length < 2)
            {
                return false;
            }

            int count = 0;

            foreach (var obj in objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (Path.GetExtension(path) == XcodeChangeFile.Extension)
                {
                    count++;
                }
            }

            return count > 1;
        }
    }
}

