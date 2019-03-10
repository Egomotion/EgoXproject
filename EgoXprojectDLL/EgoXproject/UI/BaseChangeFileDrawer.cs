// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using Egomotion.EgoXproject.UI.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal abstract class BaseChangeFileDrawer
    {
        protected enum Sections
        {
            InfoPlist,
            Frameworks,
            Scripts,
            FilesFolders,
            BuildSettings,
            Signing,
            Capabilities
        };

        Dictionary<Sections, bool> _foldoutStates = new Dictionary<Sections, bool>();

        XcodeController _controller;

        protected XcodeEditorWindow Parent
        {
            get;
            private set;
        }
        protected Styling Style
        {
            get;
            private set;
        }

        protected const float ENUM_POPUP_WIDTH = 100.0f;
        protected const float MIN_FOLDER_NAME_WIDTH = 100.0f;
        protected const float DEFAULT_FOLDER_NAME_WIDTH = 300.0f;
        protected const float FUDGE_FACTOR = 15.0f;
        protected const float FOLDER_INDENT = 10.0f;
        protected const float SQUARE_BUTTON_SPACE_WIDTH = 24.0f;
        protected const float CAPABILITY_FIRST_COLUMN_WIDTH = 100.0f;
        protected const float RIGHT_SPACE = 4.0f;
        protected const float INDENT_SPACE = 20.0f;
        protected const float VERTICAL_SPACE = 10.0f;

        protected float MaxFolderSectionWidth
        {
            get;
            private set;
        }


        protected BaseChangeFileDrawer (XcodeEditorWindow parent, Styling style)
        {
            if (parent == null)
            {
                throw new System.ArgumentNullException(nameof(parent), "Parent cannot be null");
            }

            Parent = parent;
            Style = style;

            foreach (Sections e in System.Enum.GetValues(typeof(Sections)))
            {
                _foldoutStates[e] = true;
            }

            MaxFolderSectionWidth = DEFAULT_FOLDER_NAME_WIDTH;
        }

        internal XcodeController Controller
        {
            get
            {
                if (_controller == null)
                {
                    _controller = XcodeController.Instance();
                }

                return _controller;
            }
        }

        internal XcodeChangeFile ChangeFile
        {
            get;
            set;
        }

        protected bool Foldout(Sections section)
        {
            return _foldoutStates[section];
        }

        protected void SetFoldout(Sections section, bool open)
        {
            _foldoutStates[section] = open;
        }

        protected Vector2 MainScrollViewPosition
        {
            get;
            set;
        }

        protected bool DrawFoldOut(Sections section)
        {
            string title = "";

            switch (section)
            {
            case Sections.BuildSettings:
                title = "Build Settings";
                break;

            case Sections.FilesFolders:
                title = "Files and Folders";
                break;

            case Sections.Frameworks:
                title = "System Frameworks & Libraries";
                break;

            case Sections.InfoPlist:
                title = "Info.plist";
                break;

            case Sections.Scripts:
                title = "Scripts";
                break;

            case Sections.Signing:
                title = "Signing";
                break;

            case Sections.Capabilities:
                title = "Capabilities";
                break;

            default:
                return false;
            }

            EditorGUI.BeginChangeCheck();
            bool open = EditorGUILayout.Foldout(Foldout(section), title, Style.Foldout());

            if (EditorGUI.EndChangeCheck())
            {
                SetFoldout(section, open);
            }

            return open;
        }

        public abstract void Draw();

        public abstract void Refresh();

        protected void UpdateMaxFolderWidth()
        {
            MaxFolderSectionWidth = DEFAULT_FOLDER_NAME_WIDTH;
            MaxFolderSectionWidth = Style.MaxLabelWidth(ChangeFile.FilesAndFolders.EntryNames, MIN_FOLDER_NAME_WIDTH);

            foreach (var entry in ChangeFile.FilesAndFolders.Entries)
            {
                if (entry is FolderEntry)
                {
                    float width = MaxFolderWidth(entry as FolderEntry, 1);
                    MaxFolderSectionWidth = Mathf.Max(MaxFolderSectionWidth, width);
                }
            }

            MaxFolderSectionWidth += FUDGE_FACTOR; //fudge factor
        }

        float MaxFolderWidth(FolderEntry entry, int level)
        {
            float currentLevelWidth = Style.MaxLabelWidth(entry.Names, MIN_FOLDER_NAME_WIDTH) + level * FOLDER_INDENT;
            int nextLevel = level + 1;

            foreach (var e in entry.Entries)
            {
                if (e is FolderEntry)
                {
                    float width = MaxFolderWidth(e as FolderEntry, nextLevel);
                    currentLevelWidth = Mathf.Max(currentLevelWidth, width);
                }
            }

            return currentLevelWidth;
        }

        protected void DrawLeft()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(INDENT_SPACE);
            EditorGUILayout.BeginVertical(Style.Box(), GUILayout.MinHeight(EditorGUIUtility.singleLineHeight));
        }

        protected void DrawRight()
        {
            EditorGUILayout.EndVertical();
            GUILayout.Space(RIGHT_SPACE);
            EditorGUILayout.EndHorizontal();
        }
    }
}

