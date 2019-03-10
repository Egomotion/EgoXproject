// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Egomotion.EgoXproject.UI.Internal;
using Egomotion.EgoXproject.Internal;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace Egomotion.EgoXproject.UI
{
    internal class BuildSettingSelectionPopover : EditorWindow
    {
        public delegate void OnSelectedItem(string name);

        public enum Mode
        {
            Add,
            Edit
        };

        //         group name   setting name, display name
        Dictionary<string, Dictionary<string, string>> _availableSettings = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, Dictionary<string, string>> _filteredSettings = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, bool> _groupFoldoutStates = new Dictionary<string, bool>();
        List<string> _existingSettings;

        Vector2 _position;
        OnSelectedItem _onSelectedItem;
        bool _showRawValues = false;

        string _title = "";
        string _searchString = "";

        bool _enableAddSettingButton = false;

        Styling _style;

        public static BuildSettingSelectionPopover Init(Rect position,
                Mode mode,
                string[] existingSettings,
                OnSelectedItem selectedItemCallback,
                Styling style,
                string initialString = "")
        {
            var win = EditorWindow.CreateInstance<BuildSettingSelectionPopover>();
            var size = new Vector2(position.width, position.height);
            position.width = position.height = 0;
            win.ShowAsDropDown(position, size);
            win.Configure(mode, existingSettings, selectedItemCallback, style, initialString);
            return win;
        }

        void Configure(Mode mode,
                       string[] existingSettings,
                       OnSelectedItem selectedItemCallback,
                       Styling style,
                       string initialString = "")
        {
            _style = style;
            _existingSettings = new List<string>(existingSettings);
            //exclude settings we have already added
            var settings = XcodeBuildSettings.Instance().BuildSettings.Where(o => !existingSettings.Contains(o.BuildSettingName)).ToList();

            foreach (var s in settings)
            {
                Dictionary<string, string> dic;

                if (!_availableSettings.TryGetValue(s.Group, out dic))
                {
                    dic = new Dictionary<string, string>();
                    _availableSettings[s.Group] = dic;
                }

                dic[s.BuildSettingName] = s.DisplayName;
                _groupFoldoutStates[s.Group] = true;
            }

            _searchString = initialString;
            _onSelectedItem = selectedItemCallback;

            if (mode == Mode.Add)
            {
                _title = "Add Build Setting";
            }
            else
            {
                _title = "Edit Build Setting";
            }

            UpdateFilteredList();
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

            GUILayout.Space(2);
            _showRawValues = EditorGUILayout.Toggle("Show raw values", _showRawValues);
            GUILayout.Space(2);
            DrawSearchBox();
            GUILayout.Space(6);

            if (_filteredSettings == null || _filteredSettings.Count <= 0)
            {
                EditorGUILayout.LabelField("No items");
                return;
            }

            _position = EditorGUILayout.BeginScrollView(_position);

            foreach (var groupKvp in _filteredSettings)
            {
                _groupFoldoutStates[groupKvp.Key] = EditorGUILayout.Foldout(_groupFoldoutStates[groupKvp.Key], groupKvp.Key, _style.Foldout());

                if (!_groupFoldoutStates[groupKvp.Key])
                {
                    continue;
                }

                foreach (var entryKvp in groupKvp.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    var entry = _showRawValues ? entryKvp.Key : entryKvp.Value;

                    if (GUILayout.Button(entry, EditorStyles.label, GUILayout.ExpandWidth(false)))
                    {
                        SetSelectedItem(entryKvp.Key);
                        Close();
                    }

                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(4);
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
                EditorGUI.FocusTextInControl("SearchBox");
            }

            if (EditorGUI.EndChangeCheck())
            {
                UpdateFilteredList();
            }

            if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18.0f)))
            {
                ResetSearch();
            }

            GUI.enabled = _enableAddSettingButton;

            if (_style.PlusButton("Add custom entry"))
            {
                string setting;

                if (!ContainsSetting(out setting))
                {
                    setting = _searchString;
                }

                SetSelectedItem(setting);
                Close();
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        bool ValidateEntry()
        {
            if (!XcodeBuildSettings.ValidateSettingString(_searchString))
            {
                return false;
            }

            //if is an existing value is invalid
            foreach (var existing in _existingSettings)
            {
                if (_searchString.Equals(existing, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        bool ContainsSetting(out string matchedSetting)
        {
            foreach (var groupKvp in _availableSettings)
            {
                foreach (var settingKvp in groupKvp.Value)
                {
                    if (settingKvp.Key.Equals(_searchString, StringComparison.OrdinalIgnoreCase))
                    {
                        matchedSetting = settingKvp.Key;
                        return true;
                    }
                }
            }

            matchedSetting = "";
            return false;
        }

        void UpdateFilteredList()
        {
            if (string.IsNullOrEmpty(_searchString))
            {
                ResetSearch();
                return;
            }

            var searchStr = _searchString.ToUpper();
            _filteredSettings.Clear();

            foreach (var groupKvp in _availableSettings)
            {
                var groupName = groupKvp.Key;
                var groupSettings = groupKvp.Value;
                //TODO do explcitly and compare each item using StringComparison.OrdinalIgnoreCase?
                Dictionary<string, string> dic = groupSettings.Where(kvp => kvp.Key.ToUpper().Contains(searchStr) || kvp.Value.ToUpper().Contains(searchStr)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                if (dic == null || dic.Count <= 0)
                {
                    continue;
                }

                _filteredSettings[groupName] = dic;
                _groupFoldoutStates[groupName] = true;
            }

            //TODO case insensitive search?
            _enableAddSettingButton = ValidateEntry();
        }

        void SetSelectedItem(string settingName)
        {
            if (_onSelectedItem != null)
            {
                _onSelectedItem(settingName);
            }
        }

        void ResetSearch()
        {
            _filteredSettings = new Dictionary<string, Dictionary<string, string>>(_availableSettings);
            _searchString = "";
            _enableAddSettingButton = false;
        }
    }
}
