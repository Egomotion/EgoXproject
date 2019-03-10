//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using Egomotion.EgoXproject.Internal;
using Egomotion.EgoXproject.UI.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal class GeneralPListEditor : EditorWindow
    {
        PListDrawerMutable _drawer;
        string _lastPath;
        Styling _styling = new Styling();

        [MenuItem("Window/EgoXproject/PList Editor", false, 5)]
        static void CreateWindow()
        {
            var win = EditorWindow.GetWindow<GeneralPListEditor>("PList Editor");
            win.Show();
        }

        void OnEnable()
        {
            if (_drawer == null)
            {
                _drawer = new PListDrawerMutable(_styling);
            }

            _styling.Load();
        }

        void OnDisable()
        {
            _styling.Unload();
        }

        void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("New", GUILayout.Width(100), GUILayout.ExpandWidth(false)))
            {
                Create();
            }

            if (GUILayout.Button("Open", GUILayout.Width(100), GUILayout.ExpandWidth(false)))
            {
                Load("plist");
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10.0f);
            _styling.HorizontalLine();
            _drawer.Draw();
        }

        void Load(string extension = "plist")
        {
            if (string.IsNullOrEmpty(_lastPath))
            {
                _lastPath = Application.dataPath;
            }

            string fileName = EditorUtility.OpenFilePanel("Open PList", _lastPath, extension);

            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }

            _lastPath = Path.GetDirectoryName(fileName);
            var p = new PList();

            if (p.Load(fileName))
            {
                _drawer.Data = p;
            }
            else
            {
                EditorUtility.DisplayDialog("Error Opening File", "Could not open file: " + fileName, "OK");
            }
        }

        void Create()
        {
            if (string.IsNullOrEmpty(_lastPath))
            {
                _lastPath = Application.dataPath;
            }

            string fileName = EditorUtility.SaveFilePanel("Create PList", _lastPath, "MyPlist", "plist");

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string backupName = "";

            if (File.Exists(fileName))
            {
                int count = 1;

                do
                {
                    backupName = fileName + "-" + count;
                    count++;
                }
                while (File.Exists(backupName));

                File.Move(fileName, backupName);
            }

            _lastPath = Path.GetDirectoryName(fileName);
            var p = new PList();
            bool saved = p.Save(fileName);

            if (saved)
            {
                if (!string.IsNullOrEmpty(backupName))
                {
                    File.Delete(backupName);
                }

                AssetDatabase.ImportAsset(ProjectUtil.MakePathRelativeToProject(fileName));
                _drawer.Data = p;
            }
            else
            {
                if (!string.IsNullOrEmpty(backupName))
                {
                    File.Move(backupName, fileName);
                }

                EditorUtility.DisplayDialog("Error", "Failed to create PList: " + fileName, "Ok");
            }
        }
    }
}
