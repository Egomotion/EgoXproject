// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//
using UnityEditor;
using UnityEngine;
using Egomotion.EgoXproject.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal class IOSXcodeEditorWindow : XcodeEditorWindow
    {
        [MenuItem("Window/EgoXproject/iOS Xcode Project Editor", false, 1)]
        static void CreateWindow()
        {
            var win = EditorWindow.GetWindow<IOSXcodeEditorWindow>("iOS Xcode Editor");
            win.minSize = new Vector2(800, 400);
            win.Platform = BuildPlatform.iOS;
            win.Show();
        }
    }
}
