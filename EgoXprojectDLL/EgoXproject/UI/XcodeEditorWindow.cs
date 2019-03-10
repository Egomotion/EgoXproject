//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Egomotion.EgoXproject.Internal;
using Egomotion.EgoXproject.UI.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal abstract class XcodeEditorWindow : EditorWindow
    {
        enum Tab
        {
            Changes = 0,
            Preview = 1,
            Configurations = 2
        };

        string[] _tabNames = { "Changes", "Preview", "Configurations" };

        Tab _activeTab = Tab.Changes;

        ConfigurationsTab _configurationsTab;
        PreviewTab _previewTab;
        ChangeFileTab _changesTab;

        Styling _style;

        [SerializeField]
        BuildPlatform _platform;

        public BuildPlatform Platform 
        { 
            get {
                return _platform;
            } 
            protected set {
                _platform = value;
            } 
        }

        GUIStyle _titleStyle;

        void OnEnable()
        {
            XcodeController.OnRefresh += HandleRefresh;
            _activeTab = Tab.Changes;

            if (_style == null)
            {
                _style = new Styling();
            }

            _style.Load();
        }

        void OnDisable()
        {
            if (_changesTab != null)
            {
                _changesTab.RepaintRequired -= Repaint;
            }

            if (_configurationsTab != null)
            {
                _configurationsTab.RepaintRequired -= Repaint;
            }

            XcodeController.OnRefresh -= HandleRefresh;
            SaveIfRequired();

            if (_style == null)
            {
                _style.Unload();
            }
        }

        void OnLostFocus()
        {
            SaveIfRequired();
        }

        void OnDestroy()
        {
            SaveIfRequired();
            XcodeBuildSettings.Destroy();
        }

        void SaveIfRequired()
        {
            if (XcodeController.Instance().IsDirty)
            {
                XcodeController.Instance().Save();
            }

            if (_changesTab != null && _changesTab.IsDirty)
            {
                _changesTab.Save();
            }
        }


        void OnGUI()
        {
            //Do Drawing
            DrawTitle();
            GUILayout.Space(6);
            DrawHeader();
            DrawTabs();
            //Do everything else
            Process();
        }

        void Process()
        {
            SaveIfRequired();
        }


        void DrawTitle()
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(EditorStyles.largeLabel);
                _titleStyle.fontSize = 16;
                _titleStyle.fontStyle = FontStyle.Bold;
                _titleStyle.alignment = TextAnchor.UpperCenter;
                _titleStyle.fixedHeight = 30;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(titleContent.text, _titleStyle);
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(75);
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            _activeTab = (Tab) GUILayout.Toolbar((int) _activeTab, _tabNames);

            if (EditorGUI.EndChangeCheck())
            {
                SaveIfRequired();

                if (_previewTab != null)
                {
                    _previewTab.Refresh();
                }
            }

            EditorGUILayout.Space();
            GUILayout.Space(20);
            DrawVersion();
            DrawHelp();
            EditorGUILayout.EndHorizontal();
        }

        void DrawTabs()
        {
            switch (_activeTab)
            {
            case Tab.Configurations:
                DrawConfigurations();
                break;

            case Tab.Preview:
                DrawPreview();
                break;

            case Tab.Changes:
            default:
                DrawChanges();
                break;
            }
        }

        void DrawChanges()
        {
            if (_changesTab == null)
            {
                _changesTab = new ChangeFileTab(this, _style);
                _changesTab.RepaintRequired += Repaint;
            }

            _changesTab.Draw();
        }

        void DrawPreview()
        {
            if (_previewTab == null)
            {
                _previewTab = new PreviewTab(this, _style);
            }

            _previewTab.Draw();
        }

        void DrawConfigurations()
        {
            if (_configurationsTab == null)
            {
                _configurationsTab = new ConfigurationsTab(this, _style);
                _configurationsTab.RepaintRequired += Repaint;
            }

            _configurationsTab.Draw();
        }

        void DrawVersion()
        {
            EditorGUILayout.LabelField(DllUtils.VersionString(), GUILayout.Width(35));
        }

        void DrawHelp()
        {
            if (_style.HelpButton("Help"))
            {
                Application.OpenURL("https://egomotion.co.uk/egoXproject");
            }
        }

        void HandleRefresh()
        {
            if (_changesTab != null)
            {
                _changesTab.Refresh();
            }

            if (_previewTab != null)
            {
                _previewTab.Refresh();
            }

            if (_configurationsTab != null)
            {
                _changesTab.Refresh();
            }
        }
    }
}
