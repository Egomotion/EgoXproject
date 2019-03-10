// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Egomotion.EgoXproject.Internal;
using System.Linq;
using Egomotion.EgoXproject.UI.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal class FrameworksPopover : EditorWindow
    {
        public delegate void OnSelectedItem(string item);

        string[] _content;
        Vector2 _scrollPosition;
        OnSelectedItem _onSelectedItem;

        string _searchString;
        List<string> _filteredList = new List<string>();

        string _manualEntry = "";

        Styling _style;

        XcodeSDKFinder _xcodeFinder;

        BuildPlatform _platform;

        public static FrameworksPopover Init(Rect position, OnSelectedItem selectedItemCallback, Styling style, BuildPlatform platform)
        {
            var win = EditorWindow.CreateInstance<FrameworksPopover>();
            win.Configure(position, selectedItemCallback, style, platform);
            return win;
        }

        void Configure(Rect popoverPosition, OnSelectedItem selectedItemCallback, Styling style, BuildPlatform platform)
        {
            _xcodeFinder = new XcodeSDKFinder(platform);
            var minWidth = style.MaxLabelWidth(_xcodeFinder.FrameworkNames, popoverPosition.width) + 40;
            var width = popoverPosition.width;
            _platform = platform;

            if (minWidth > popoverPosition.width)
            {
                width = Mathf.Max(minWidth, popoverPosition.width);
                float delta = (minWidth - popoverPosition.width) * 0.5f;
                popoverPosition.x -= delta;
            }

            var size = new Vector2(width, popoverPosition.height);
            popoverPosition.width = popoverPosition.height = 0;
            _onSelectedItem = selectedItemCallback;
            _style = style;
            ShowAsDropDown(popoverPosition, size);
        }

        void OnGUI()
        {
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                EditorGUILayout.LabelField("Can only browse frameworks on OS X.");
                EditorGUILayout.LabelField("Please enter framework names manually.");
                ManualEntry();
                return;
            }

            if (!_xcodeFinder.IsFound)
            {
                EditorGUILayout.LabelField("Xcode not found. Please set the Xcode location");

                if (GUILayout.Button("Find Xcode"))
                {
                    var xcodePath = EditorUtility.OpenFilePanel("Choose Xcode Location", "/Applications", "app");

                    if (!string.IsNullOrEmpty(xcodePath))
                    {
                        if (XcodeFinder.SetXcodeLocation(xcodePath))
                        {
                            _xcodeFinder = new XcodeSDKFinder(_platform);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Invalid Xcode Location", xcodePath + " does not appear to be a valid Xcode install", "OK");
                        }
                    }
                }

                EditorGUILayout.HelpBox("You can change the location under settings", MessageType.Info);
                GUILayout.Space(20);
                EditorGUILayout.LabelField("Alternatively, add the framework names manually below.");
                ManualEntry();
                return;
            }

            if (_content == null)
            {
                _content = _xcodeFinder.FrameworkNames;
                _filteredList.Clear();
                _filteredList.AddRange(_content);
            }

            DrawSearchBox();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (string framework in _filteredList)
            {
                if (GUILayout.Button(framework, EditorStyles.label))
                {
                    AddFramework(framework);
                }
            }

            EditorGUILayout.EndScrollView();

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Close();
                }
            }
        }

        void DrawSearchBox()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("SearchBox");
            _searchString = EditorGUILayout.TextField("", _searchString, "SearchTextField");

            if (string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()))
            {
                GUI.FocusControl("SearchBox");
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (string.IsNullOrEmpty(_searchString))
                {
                    _filteredList.Clear();
                    _filteredList.AddRange(_content);
                }
                else
                {
                    var searchStr = _searchString.ToLower();
                    _filteredList = _content.Where(k => k.ToLower().Contains(searchStr)).ToList();
                }
            }

            if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
            {
                _searchString = "";
                _filteredList.Clear();
                _filteredList.AddRange(_content);
            }

            EditorGUILayout.EndHorizontal();
        }

        void ManualEntry()
        {
            EditorGUILayout.BeginHorizontal();
            _manualEntry = EditorGUILayout.TextField(_manualEntry);
            EditorGUILayout.Space();
            GUI.enabled = _manualEntry.Trim().Length > 0;

            if (_style.PlusButton("Add framework"))
            {
                AddFramework(_manualEntry);
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        void AddFramework(string frameworkName)
        {
            if (_onSelectedItem != null)
            {
                _onSelectedItem(frameworkName);
            }
        }
    }
}
