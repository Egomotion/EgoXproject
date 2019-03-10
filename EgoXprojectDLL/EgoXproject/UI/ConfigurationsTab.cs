// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using Egomotion.EgoXproject.UI.Internal;
using System.Linq;

namespace Egomotion.EgoXproject.UI
{
    internal class ConfigurationsTab
    {
        public event System.Action RepaintRequired;

        bool _enableControls = true;

        Vector2 _scrollPosition = Vector2.zero;

        PlatformConfiguration _platformConfiguration;

        bool _drawChangeFilesDropDown = false;
        string _configuration = "";

        bool _drawRenameDialog = false;
        string _configToRename = "";

        bool _drawAddDialog = false;

        string _configToRemove = "";

        XcodeEditorWindow _parent;

        Styling _style;

        public ConfigurationsTab (XcodeEditorWindow parent, Styling style)
        {
            if (parent == null)
            {
                throw new System.ArgumentNullException(nameof(parent), "parent cannot be null");
            }

            _parent = parent;
            _style = style;
            _platformConfiguration = XcodeController.Instance().Configuration(_parent.Platform);
        }

        public void Draw()
        {
            _drawChangeFilesDropDown = false;
            _drawRenameDialog = false;
            _drawAddDialog = false;
            _configToRemove = "";
            GUILayout.Space(10);
            GUI.enabled = _enableControls;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawConfiguration();
            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();

            if (_drawChangeFilesDropDown)
            {
                _drawChangeFilesDropDown = false;
                var dropSize = new Vector2(400, 300);
                var dropPos = _parent.position.center - dropSize * 0.5f;
                var r = new Rect(dropPos.x, dropPos.y, dropSize.x, dropSize.y);
                var filtered = XcodeController.Instance().ChangeFiles(_parent.Platform).Except(_platformConfiguration.ChangeFilesInConfiguration(_configuration)).ToArray();
                ListSelectionPopover.Init(r, "Select a change file", filtered, AddChangeFile, _style);
            }

            if (_drawRenameDialog)
            {
                _drawRenameDialog = false;
                Vector2 position = _parent.position.center;
                TextEditPopover.Init(position,
                                     300,
                                     "Rename Configuration",
                                     "",
                                     _configToRename,
                                     HandleOnRename,
                                     _platformConfiguration.IsValidConfigurationName,
                                     "Rename",
                                     _style);
            }

            if (_drawAddDialog)
            {
                _drawAddDialog = false;
                Vector2 position = _parent.position.center;
                TextEditPopover.Init(position,
                                     300,
                                     "Add Configuration",
                                     "",
                                     "",
                                     HandleOnAdd,
                                     _platformConfiguration.IsValidConfigurationName,
                                     "Add",
                                     _style);
            }

            if (!string.IsNullOrEmpty(_configToRemove))
            {
                if (EditorUtility.DisplayDialog("Remove Configuration?", "Are you sure you want to remove the \"" + _configToRemove + "\" configuration?", "Remove", "Cancel"))
                {
                    _platformConfiguration.RemoveConfiguration(_configToRemove);
                }
            }

            GUI.enabled = true;
        }

        public void Process()
        {
        }

        void AddChangeFile(string changeFile)
        {
            _platformConfiguration.AddChangeFileToConfiguration(changeFile, _configuration);

            if (RepaintRequired != null)
            {
                RepaintRequired();
            }
        }

        void DrawConfiguration()
        {
            EditorGUILayout.LabelField("Configurations", EditorStyles.boldLabel);
            DrawConfigs();
        }

        void DrawConfigs()
        {
            EditorGUI.indentLevel++;
            var configs = _platformConfiguration.Configurations;

            foreach (var config in configs)
            {
                if (config == PlatformConfiguration.DEFAULT_CONFIG_NAME)
                {
                    DrawDefaultConfig();
                }
                else
                {
                    DrawConfig(config);
                }

                GUILayout.Space(6);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (_style.PlusButton("Add configuration"))
            {
                _drawAddDialog = true;
            }

            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void DrawConfig(string configName)
        {
            bool selected = (configName == _platformConfiguration.ActiveConfiguration);
            GUI.backgroundColor = (selected ? Color.yellow : Color.white);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            bool enable = EditorGUILayout.ToggleLeft("  " + configName, selected, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));

            if (EditorGUI.EndChangeCheck() && !selected && enable)
            {
                _platformConfiguration.ActiveConfiguration = configName;
            }

            EditorGUILayout.Space();

            if (_style.EditButton("Change name"))
            {
                _drawRenameDialog = true;
                _configToRename = configName;
            }

            if (_style.MinusButton("Remove configuration"))
            {
                _configToRemove = configName;
            }

            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            EditorGUILayout.BeginVertical(_style.IndentedBox());
            var changeFiles = _platformConfiguration.ChangeFilesInConfiguration(configName);
            float labelWidth = 200.0f;

            foreach (var changeFile in changeFiles)
            {
                float width = EditorStyles.label.CalcSize(new GUIContent(changeFile)).x;
                labelWidth = Mathf.Max(labelWidth, width);
            }

            string toRemove = "";

            foreach (var changeFile in changeFiles)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(changeFile, GUILayout.Width(labelWidth + 20.0f));
                EditorGUILayout.Space();

                if (_style.MinusButton("Remove change file from configuration"))
                {
                    if (EditorUtility.DisplayDialog("Remove change file from configuration?", "Are your sure you want to remove the \"" + System.IO.Path.GetFileName(changeFile) + "\" change file from the \"" + configName + "\" configuration?", "Remove", "Cancel"))
                    {
                        toRemove = changeFile;
                    }
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(4);
            }

            if (!string.IsNullOrEmpty(toRemove))
            {
                _platformConfiguration.RemoveChangeFileFromConfiguration(toRemove, configName);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUI.enabled = XcodeController.Instance().ChangeFileCount(_parent.Platform) > 0;

            if (_style.PlusButton("Add change file to configuration"))
            {
                _drawChangeFilesDropDown = true;
                _configuration = configName;
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
        }

        void DrawDefaultConfig()
        {
            bool selected = (PlatformConfiguration.DEFAULT_CONFIG_NAME == _platformConfiguration.ActiveConfiguration);
            GUI.backgroundColor = (selected ? Color.yellow : Color.white);
            EditorGUI.BeginChangeCheck();
            bool enable = EditorGUILayout.ToggleLeft("  " + PlatformConfiguration.DEFAULT_CONFIG_NAME, selected, EditorStyles.boldLabel);

            if (EditorGUI.EndChangeCheck() && !selected && enable)
            {
                _platformConfiguration.ActiveConfiguration = PlatformConfiguration.DEFAULT_CONFIG_NAME;
            }

            EditorGUILayout.BeginVertical(_style.IndentedBox());
            var changeFiles = XcodeController.Instance().ChangeFiles(_parent.Platform);
            float labelWidth = 200.0f;

            foreach (var changeFile in changeFiles)
            {
                float width = EditorStyles.label.CalcSize(new GUIContent(changeFile)).x;
                labelWidth = Mathf.Max(labelWidth, width);
            }

            foreach (var changeFile in changeFiles)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(changeFile, GUILayout.Width(labelWidth + 20.0f));
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
        }

        void HandleOnRename(string original, string name)
        {
            _platformConfiguration.RenameConfiguration(original, name);
        }

        void HandleOnAdd(string original, string name)
        {
            _platformConfiguration.AddConfiguration(name);
        }
    }

}
