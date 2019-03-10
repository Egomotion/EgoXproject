// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using Egomotion.EgoXproject.UI.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal class SettingsWindow : EditorWindow
    {
        [MenuItem("Window/EgoXproject/Settings", false, 3)]
        static void CreateWindow()
        {
            var upgrader = new Upgrader();
            upgrader.Upgrade();
            var win = EditorWindow.GetWindow<SettingsWindow>("Settings");
            win.minSize = new Vector2(400, 200);
            win.Show();
        }

        XcodeSettings _settings;
        List<string> _customIgnoredFiles = new List<string>();

        Vector2 _scrollPosition = Vector2.zero;

        Styling _style;

        void OnEnable()
        {
            _settings = XcodeController.Instance().Settings;
            _customIgnoredFiles.Clear();
            _customIgnoredFiles.AddRange(IgnoredFiles.CustomList);

            if (_style == null)
            {
                _style = new Styling();
            }

            _style.Load();
        }

        void OnDisable()
        {
            _style.Unload();
        }

        void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawSettings();
            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
        }

        void DrawSettings()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("egoXproject Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Auto Run");
            EditorGUI.indentLevel--;
            EditorGUILayout.BeginVertical(_style.IndentedBox());
            EditorGUILayout.BeginHorizontal();
            _style.FixedWidthLabel("Enable merging during project build", "Turn this off if you need to override when egoXproject builds.", 300);
            EditorGUI.BeginChangeCheck();
            var enabled = EditorGUILayout.Toggle(_settings.AutoRunEnabled);

            if (EditorGUI.EndChangeCheck())
            {
                _settings.AutoRunEnabled = enabled;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
            DrawIgnoredFilesAndFolders();
            GUILayout.Space(10);
            DrawCustomXcode();
            GUILayout.Space(10);
        }

        void DrawIgnoredFilesAndFolders()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(new GUIContent("Ignored Files and Folders", "File and folders that will be ignored when adding folders to change files"));
            EditorGUI.indentLevel--;
            EditorGUILayout.BeginVertical(_style.IndentedBox());
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Default Ignored Files", "These files are always ignored when adding folders"), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField(StringUtils.ArrayToString(IgnoredFiles.DefaultList), EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Custom Ignored Files", "Add filenames and wildcards entries here to have them ignored when adding folders"), GUILayout.ExpandWidth(false));
            EditorGUI.indentLevel++;
            bool update = false;

            int remove = -1;

            for (int ii = 0; ii < _customIgnoredFiles.Count; ++ii)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                _customIgnoredFiles[ii] = EditorGUILayout.TextField(_customIgnoredFiles[ii]);

                if (EditorGUI.EndChangeCheck())
                {
                    update = true;
                }

                EditorGUILayout.Space();

                if (_style.MinusButton("Remove ignored file pattern"))
                {
                    bool removeCheck = true;

                    if (!string.IsNullOrEmpty(_customIgnoredFiles[ii].Trim()))
                    {
                        removeCheck = EditorUtility.DisplayDialog("Remove ignored file pattern?", "Are you sure you want to remove \"" + _customIgnoredFiles[ii] + "\" from the ignore list?", "Remove", "Cancel");
                    }

                    if (removeCheck)
                    {
                        remove = ii;
                    }
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
            }

            if (remove > -1)
                {
                    IgnoredFiles.Remove(_customIgnoredFiles[remove]);
                    _customIgnoredFiles.RemoveAt(remove);
                    _settings.SetDirty();
                    _settings.Save();
                }

            if (update)
            {
                IgnoredFiles.SetIngnoredFiles(_customIgnoredFiles.ToArray());
                _settings.SetDirty();
                _settings.Save();
                update = false;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (_style.PlusButton("Add new ignored file pattern"))
            {
                _customIgnoredFiles.Add("");
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }

        void DrawCustomXcode()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(new GUIContent("Xcode Location", "Change this if your Xcode install is not in the standard place"));
            EditorGUI.indentLevel--;
            EditorGUILayout.BeginVertical(_style.IndentedBox());

            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                EditorGUILayout.LabelField("You can only set a custom Xcode location on OS X");
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(XcodeFinder.XcodeLocation);

                if (GUILayout.Button("Select", GUILayout.Width(100)))
                {
                    var xcodePath = EditorUtility.OpenFilePanel("Choose Xcode Location", "/Applications", "app");

                    if (!string.IsNullOrEmpty(xcodePath))
                    {
                        if (!XcodeFinder.SetXcodeLocation(xcodePath))
                        {
                            EditorUtility.DisplayDialog("Invalid Xcode Location", xcodePath + " does not appear to be a valid Xcode install", "OK");
                        }
                    }
                }

                GUI.enabled = XcodeFinder.UsingCustomLocation;

                if (GUILayout.Button("Use default", GUILayout.Width(100)))
                {
                    XcodeFinder.ClearCustomXcodeLocation();
                }

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                if (!XcodeFinder.IsFound)
                {
                    EditorGUILayout.HelpBox("Xcode not found. Please set a valid location.", MessageType.Error);
                }
            }

            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }
    }
}
