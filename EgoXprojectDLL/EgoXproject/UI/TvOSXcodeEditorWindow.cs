// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEditor;
using Egomotion.EgoXproject.Internal;
using UnityEngine;

namespace Egomotion.EgoXproject.UI
{
    internal class TvOSXcodeEditorWindow : XcodeEditorWindow
    {
        [MenuItem("Window/EgoXproject/tvOS Xcode Project Editor", false, 2)]
        static void CreatetvOSWindow()
        {
            var win = EditorWindow.GetWindow<TvOSXcodeEditorWindow>("tvOS Xcode Editor");
            win.minSize = new Vector2(400, 200);
            win.Platform = BuildPlatform.tvOS;
            win.Show();
        }
    }
}
