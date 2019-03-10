// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Egomotion.EgoXproject.UI.Internal;
using System.Linq;
using System;

namespace Egomotion.EgoXproject.UI
{
    internal class ListSelectionPopover : EditorWindow
    {
        public delegate void OnSelectedItem(string name);

        string[] _content;
        Vector2 _position;
        OnSelectedItem _onSelectedItem;

        string _searchString = "";
        string[] _filtered;

        bool _allowCustomEntry = false;

        string _title;

        Styling _style;

        public static ListSelectionPopover Init(Rect position, string title, string[] content, OnSelectedItem selectedItemCallback, Styling style, bool allowCustomEntry = false, string initialString = "")
        {
            var win = EditorWindow.CreateInstance<ListSelectionPopover>();
            win.Configure(position, title, content, selectedItemCallback, style, allowCustomEntry, initialString);
            return win;
        }

        void Configure(Rect popoverPosition, string popoverTitle, string[] content, OnSelectedItem selectedItemCallback, Styling style, bool allowCustomEntry, string initialString)
        {
            var size = new Vector2(popoverPosition.width, popoverPosition.height);
            popoverPosition.width = popoverPosition.height = 0;
            _content = content;
            _filtered = content;
            _searchString = initialString;
            _onSelectedItem = selectedItemCallback;
            _allowCustomEntry = allowCustomEntry;
            _title = popoverTitle;
            _style = style;
            UpdateFilteredList();
            ShowAsDropDown(popoverPosition, size);
        }

        void OnGUI()
        {
            if (!string.IsNullOrEmpty(_title))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                _style.MinWidthBoldLabel(_title);
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
                _style.HorizontalLine();
            }

            DrawSearchBox();

            if (_filtered == null || _filtered.Length <= 0)
            {
                return;
            }

            _position = EditorGUILayout.BeginScrollView(_position);

            for (int ii = 0; ii < _filtered.Length; ++ii)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();

                if (GUILayout.Button(_filtered[ii], EditorStyles.label, GUILayout.ExpandWidth(false)))
                {
                    SetSelectedItem(_filtered[ii]);
                    Close();
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(4);
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
                EditorGUI.FocusTextInControl("SearchBox");
            }

            if (EditorGUI.EndChangeCheck())
            {
                UpdateFilteredList();
            }

            if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
            {
                _searchString = "";
                _filtered = _content;
            }

            if (_allowCustomEntry)
            {
                GUI.enabled = !string.IsNullOrEmpty(_searchString.Trim());

                if (_style.PlusButton("Add custom entry"))
                {
                    string value = _searchString;
                    int index = Array.FindIndex(_filtered, f => f.Equals(_searchString, StringComparison.OrdinalIgnoreCase));

                    if (index > -1)
                    {
                        value = _filtered[index];
                    }

                    SetSelectedItem(value);
                    Close();
                }

                GUI.enabled = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        void UpdateFilteredList()
        {
            if (string.IsNullOrEmpty(_searchString))
            {
                _filtered = _content;
            }
            else
            {
                var searchStr = _searchString.ToLower();
                _filtered = _content.Where(k => k.ToLower().Contains(searchStr)).ToArray();
            }
        }

        void SetSelectedItem(string itemName)
        {
            if (_onSelectedItem != null)
            {
                _onSelectedItem(itemName);
            }
        }
    }
}
