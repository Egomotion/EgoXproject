// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEngine;
using UnityEditor;
using Egomotion.EgoXproject.Internal;
using System.Linq;
using System.IO;
using Egomotion.EgoXproject.UI.Internal;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.UI
{
    internal class ChangeFileTab : BaseChangeFileDrawer
    {
        public event System.Action RepaintRequired;

        string _lastLocation = string.Empty;

        PListElementDrawerMutable _plistDrawer;

        bool _drawAddBuildSetting = false;
        bool _drawEditBuildSetting = false;
        string _buildSettingToEdit;
        bool _drawAddFramework = false;

        bool _showBuildSettingRawValues = false;

        string _lastOpenedChangeFilePath;

        public ChangeFileTab(XcodeEditorWindow parent, Styling style)
        : base(parent, style)
        {
            _lastLocation = Application.dataPath;
            _plistDrawer = new PListElementDrawerMutable(style);
        }

        public override void Draw()
        {
            if (string.IsNullOrEmpty(_lastLocation))
            {
                _lastLocation = Application.dataPath;
            }

            LoadChangeFile();

            if (Controller.ChangeFileCount(Parent.Platform) <= 0)
            {
                DrawStartHere();
            }
            else
            {
                DrawHeader();
                DrawMainView();
            }
        }

        void DrawStartHere()
        {
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            Style.MinWidthBoldLabel("Create a new change file", 5);
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (Style.LargePlusButton("Create new change file"))
            {
                CreateNewChangeFile();
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        void CreateNewChangeFile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create a new change file",
                          "Default-" + Parent.Platform.ToString(),
                          XcodeChangeFile.Extension.Remove(0, 1),   //stupid unity
                          "Enter a name for the new change file",
                          Controller.LastSaveDirectory);

            if (!string.IsNullOrEmpty(path))
            {
                //We assume this is correct as the user is prompted by the dialog about replacing the file
                ChangeFile = Controller.CreateChangeFile(Parent.Platform, path);
                _plistDrawer.Reset();
            }
        }

        void LoadChangeFile()
        {
            if (ChangeFile != null)
            {
                return;
            }

            //try loading the last opened file
            if (!string.IsNullOrEmpty (_lastOpenedChangeFilePath))
            {
                var changeFiles = Controller.ChangeFiles (Parent.Platform);
                int foundIndex = System.Array.IndexOf (changeFiles, _lastOpenedChangeFilePath);

                if (foundIndex > -1)
                {
                    if (LoadChangeFile (foundIndex))
                    {
                        return;
                    }
                }
            }

            //load the first change file if not loaded. keep trying until one loads
            while (ChangeFile == null && Controller.ChangeFileCount(Parent.Platform) > 0)
            {
                if (LoadChangeFile (0))
                {
                    return;
                }
            }
        }

        bool LoadChangeFile(int index)
        {
            var filePath = Controller.ChangeFiles (Parent.Platform) [index];
            ChangeFile = XcodeChangeFile.Load (filePath);

            if (ChangeFile == null)
            {
                Controller.RemoveChangeFileFromList (Parent.Platform, filePath);
                ChangeFile = null;
                return false;
            }
            else
            {
                _lastOpenedChangeFilePath = ChangeFile.SavePath;
                _plistDrawer.Reset ();
                UpdateMaxFolderWidth ();
                return true;
            }
        }

        void DrawHeader()
        {
            //TODO do this once and only refesh when assetwatcher updates the list or controller sets a flag
            var changeFiles = Controller.ChangeFiles(Parent.Platform);
            int currentIndex = System.Array.IndexOf(changeFiles, ChangeFile.SavePath);
            //To stop the fancy nested heirachy thing the popup does
            var names = new string[changeFiles.Length];

            for (int ii = 0; ii < changeFiles.Length; ++ii)
            {
                string s = changeFiles[ii];

                if (s.StartsWith("Assets", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    s = s.Remove(0, 7);
                }
                else if (s.StartsWith(Application.dataPath, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    s = s.Remove(0, Application.dataPath.Length + 1);
                }

                s = s.Replace("/", " | ");  //for windows!
                names[ii] = s;
            }

            GUILayout.Space(VERTICAL_SPACE);
            EditorGUILayout.BeginHorizontal();
            float textWidth = Style.MinWidthBoldLabel("Change File", 10.0f);
            EditorGUI.BeginChangeCheck();
            int index = EditorGUILayout.Popup(currentIndex, names);

            if (EditorGUI.EndChangeCheck())
            {
                if (ChangeFile.SavePath != changeFiles[index])
                {
                    Save();
                    ChangeFile = XcodeChangeFile.Load(changeFiles[index]);
                    //TODO handle load failure
                    _lastOpenedChangeFilePath = ChangeFile.SavePath;
                    _plistDrawer.Reset();
                    UpdateMaxFolderWidth();
                }
            }

            if (Style.PlusButton("Create a new change file"))
            {
                CreateNewChangeFile();
            }

            if (Style.MinusButton("Remove change file"))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete",
                                                "Are you sure you want to delete the " + Path.GetFileNameWithoutExtension(ChangeFile.SavePath) + " change file?",
                                                "Delete",
                                                "Cancel"))
                {
                    Controller.DeleteChangeFile(ChangeFile.SavePath);
                    ChangeFile = null;
                    _plistDrawer.Reset();
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path", EditorStyles.boldLabel, GUILayout.Width(textWidth));
            EditorGUILayout.LabelField(ChangeFile.SavePath);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);
            //TODO draw list of configurations this file is in?
            //TODO Indicate if this is in the active configuration?
            Style.HorizontalLine();
        }

        void DrawMainView()
        {
            MainScrollViewPosition = EditorGUILayout.BeginScrollView(MainScrollViewPosition);
            DrawInfoPlistSection();
            DrawFrameworksSection();
            DrawFilesAndFoldersSection();
            DrawBuildSettingsSection();
            DrawSigningSection();
            DrawCapabilitiesSection();
            DrawScriptsSection();
            GUILayout.Space(VERTICAL_SPACE);
            EditorGUILayout.EndScrollView();

            if (_drawAddBuildSetting)
            {
                _drawAddBuildSetting = false;
                Vector2 popUpSize = new Vector2(400, 300);
                Vector2 popUpPos = Parent.position.center - popUpSize * 0.5f;
                var r = new Rect(popUpPos.x, popUpPos.y, popUpSize.x, popUpSize.y);
                BuildSettingSelectionPopover.Init(r, BuildSettingSelectionPopover.Mode.Add, ChangeFile.BuildSettings.Names, AddBuildSetting, Style, "");
            }

            if (_drawEditBuildSetting)
            {
                _drawEditBuildSetting = false;
                Vector2 popUpSize = new Vector2(400, 300);
                Vector2 popUpPos = Parent.position.center - popUpSize * 0.5f;
                var r = new Rect(popUpPos.x, popUpPos.y, popUpSize.x, popUpSize.y);
                BuildSettingSelectionPopover.Init(r, BuildSettingSelectionPopover.Mode.Edit, ChangeFile.BuildSettings.Names, EditBuildSetting, Style, _buildSettingToEdit);
            }

            if (_drawAddFramework)
            {
                _drawAddFramework = false;
                Vector2 popUpSize = new Vector2(300, 400);
                Vector2 popUpPos = Parent.position.center - popUpSize * 0.5f;
                var r = new Rect(popUpPos.x, popUpPos.y, popUpSize.x, popUpSize.y);
                FrameworksPopover.Init(r, AddFramework, Style, Parent.Platform);
            }
        }

        void DrawInfoPlistSection()
        {
            if (!DrawFoldOut(Sections.InfoPlist))
            {
                return;
            }

            EditorGUI.indentLevel++;
            string description = "Add or replace entries in the Info.plist.\n" +
                                 "- Existing values will be replaced.\n" +
                                 "- Dictionaries will have entries added or replaced if they exist.\n" +
                                 "- Arrays will have entries appended unless they match exactly.";
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
            EditorGUI.indentLevel--;
            DrawLeft();
            var dict = ChangeFile.InfoPlistChanges;
            _plistDrawer.DrawPList(dict);
            DrawRight();

            if (_plistDrawer.IsDirty)
            {
                ChangeFile.IsDirty = true;
            }

            GUILayout.Space(VERTICAL_SPACE);
        }

        void DrawFrameworksSection()
        {
            if (!DrawFoldOut(Sections.Frameworks))
            {
                return;
            }

            EditorGUI.indentLevel++;
            string description = "Add system frameworks and libraries";
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
            EditorGUI.indentLevel--;
            var frameworks = ChangeFile.Frameworks;
            float maxWidth = Style.MaxLabelWidth(frameworks.Names, MIN_FOLDER_NAME_WIDTH, DEFAULT_FOLDER_NAME_WIDTH) + FUDGE_FACTOR;
            DrawLeft();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.miniBoldLabel, GUILayout.MaxWidth(maxWidth + SQUARE_BUTTON_SPACE_WIDTH));
            EditorGUILayout.LabelField("Status", EditorStyles.miniBoldLabel, GUILayout.MaxWidth(ENUM_POPUP_WIDTH));
            EditorGUILayout.EndHorizontal();
            Style.HorizontalLine();
            int toRemove = -1;

            for (int ii = 0; ii < frameworks.Count; ++ii)
            {
                if (DrawFrameworkEntry(ii, maxWidth))
                {
                    toRemove = ii;
                }
            }

            if (toRemove > -1)
            {
                frameworks.RemoveAt(toRemove);
                ChangeFile.IsDirty = true;
            }

            toRemove = -1;
            GUILayout.Space(6);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _drawAddFramework = false;

            if (GUILayout.Button(new GUIContent("+ Framework or Library", "Add system framework or library")))
            {
                _drawAddFramework = true;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawRight();
            GUILayout.Space(VERTICAL_SPACE);
        }

        bool DrawFrameworkEntry(int index, float maxWidth)
        {

            bool remove = false;

            var frameworks = ChangeFile.Frameworks;
            string f = frameworks.FileNameAt(index);
            EditorGUILayout.BeginHorizontal();
            Style.FixedWidthLabel(f, maxWidth);
            GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
            EditorGUI.BeginChangeCheck();
            LinkType status = (LinkType) EditorGUILayout.EnumPopup(frameworks.LinkTypeAt(index), GUILayout.Width(ENUM_POPUP_WIDTH));

            if (EditorGUI.EndChangeCheck())
            {
                frameworks.SetLinkTypeAt(index, status);
                ChangeFile.IsDirty = true;
            }

            EditorGUILayout.Space();

            if (Style.MinusButton("Remove " + f))
            {
                remove = EditorUtility.DisplayDialog("Remove Framework?", "Are you sure you want to remove " + f + "?", "Remove", "Cancel");
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);

            return remove;
        }

        void DrawFilesAndFoldersSection()
        {
            if (!DrawFoldOut(Sections.FilesFolders))
            {
                return;
            }

            EditorGUI.indentLevel++;
            string description = "Add files, frameworks, libraries, folders, bundles, asset catelogs, core data models to the Xcode project";
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
            EditorGUI.indentLevel--;
            var filesFolders = ChangeFile.FilesAndFolders;
            DrawLeft();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.miniBoldLabel, GUILayout.MaxWidth(MaxFolderSectionWidth + SQUARE_BUTTON_SPACE_WIDTH));
            EditorGUILayout.LabelField("Add Method", EditorStyles.miniBoldLabel, GUILayout.MaxWidth(ENUM_POPUP_WIDTH));
            EditorGUILayout.LabelField("Options", EditorStyles.miniBoldLabel);
            EditorGUILayout.EndHorizontal();
            Style.HorizontalLine();
            int toRemove = -1;
            //Draw the list of files and folders to add to the project

            bool remove = DrawFileEntries(filesFolders.Entries, 0, MaxFolderSectionWidth, 0, out toRemove);

            if (remove)
            {
                filesFolders.RemoveEntryAt(toRemove);
                ChangeFile.IsDirty = true;
                UpdateMaxFolderWidth();
            }

            GUILayout.Space(6);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            //draw the add file buttons
            if (GUILayout.Button(new GUIContent("+ File", "Add file")))
            {
                string path = EditorUtility.OpenFilePanel("Add File", _lastLocation, "");

                if (!string.IsNullOrEmpty(path))
                {
                    _lastLocation = Path.GetDirectoryName(path);
                    filesFolders.AddFileOrFolder(path);
                    ChangeFile.IsDirty = true;
                    UpdateMaxFolderWidth();
                }
            }

            //draw the add file buttons
            if (GUILayout.Button(new GUIContent("+ Folder", "Add folder")))
            {
                string path = EditorUtility.OpenFolderPanel("Add Folder", _lastLocation, "");

                if (!string.IsNullOrEmpty(path))
                {
                    _lastLocation = Path.GetDirectoryName(path);
                    filesFolders.AddFileOrFolder(path);
                    ChangeFile.IsDirty = true;
                    UpdateMaxFolderWidth();
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawRight();
            GUILayout.Space(VERTICAL_SPACE);
        }

        bool DrawFileEntry(FileEntry entry, float labelWidth, float indentWidth, bool isChild = false)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentWidth);
            Style.FixedWidthLabel(entry.FileName, entry.Path, labelWidth);

            if (FileAndFolderEntryFactory.Exists(entry.Path))
            {
                GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
            }
            else
            {
                Style.WarningIcon("File is missing");
            }

            if (isChild)
            {
                GUILayout.Space(ENUM_POPUP_WIDTH);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var method = (AddMethod) EditorGUILayout.EnumPopup(entry.Add, GUILayout.Width(ENUM_POPUP_WIDTH));

                if (EditorGUI.EndChangeCheck())
                {
                    entry.Add = method;
                    ChangeFile.IsDirty = true;
                }
            }

            EditorGUILayout.Space();
            GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);

            bool remove = false;

            if (Style.MinusButton("Remove " + entry.FileName))
            {
                remove = EditorUtility.DisplayDialog("Remove File?", "Are you sure you want to remove " + entry.FileName + "?", "Remove", "Cancel");
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);

            return remove;
        }

        bool DrawSourceFileEntry(SourceFileEntry entry, float labelWidth, float indentWidth, bool isChild = false)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentWidth);
            Style.FixedWidthLabel(entry.FileName, entry.Path, labelWidth);

            if (FileAndFolderEntryFactory.Exists(entry.Path))
            {
                GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
            }
            else
            {
                Style.WarningIcon("File is missing");
            }

            if (isChild)
            {
                GUILayout.Space(ENUM_POPUP_WIDTH);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var method = (AddMethod) EditorGUILayout.EnumPopup(entry.Add, GUILayout.Width(ENUM_POPUP_WIDTH));

                if (EditorGUI.EndChangeCheck())
                {
                    entry.Add = method;
                    ChangeFile.IsDirty = true;
                }
            }

            EditorGUILayout.LabelField("Compiler Flags", GUILayout.MaxWidth(ENUM_POPUP_WIDTH));
            EditorGUI.BeginChangeCheck();
            var compilerFlags = EditorGUILayout.TextField(entry.CompilerFlags);

            if (EditorGUI.EndChangeCheck())
            {
                entry.CompilerFlags = compilerFlags;
                ChangeFile.IsDirty = true;
            }

            GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);

            bool remove = false;

            if (Style.MinusButton("Remove " + entry.FileName))
            {
                remove = EditorUtility.DisplayDialog("Remove File?", "Are you sure you want to remove " + entry.FileName + "?", "Remove", "Cancel");
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);

            return remove;
        }

        bool DrawFrameworkEntry(FrameworkEntry entry, float labelWidth, float indentWidth, bool isChild = false)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentWidth);
            Style.FixedWidthLabel(entry.FileName, entry.Path, labelWidth);

            if (FileAndFolderEntryFactory.Exists(entry.Path))
            {
                GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
            }
            else
            {
                Style.WarningIcon("File is missing");
            }

            if (isChild)
            {
                GUILayout.Space(ENUM_POPUP_WIDTH);
            }
            else
            {
                if (entry.Embedded)
                {
                    Style.FixedWidthLabel(AddMethod.Copy.ToString(), "Embedded frameworks have to be copied", ENUM_POPUP_WIDTH);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    var method = (AddMethod) EditorGUILayout.EnumPopup(entry.Add, GUILayout.Width(ENUM_POPUP_WIDTH));

                    if (EditorGUI.EndChangeCheck())
                    {
                        entry.Add = method;
                        ChangeFile.IsDirty = true;
                    }
                }
            }

            EditorGUILayout.LabelField("Status", GUILayout.Width(ENUM_POPUP_WIDTH));

            if (entry.Embedded)
            {
                Style.FixedWidthLabel(LinkType.Required.ToString(), "Embedded frameworks are required", ENUM_POPUP_WIDTH);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                LinkType status = (LinkType) EditorGUILayout.EnumPopup(entry.Link, GUILayout.Width(ENUM_POPUP_WIDTH));

                if (EditorGUI.EndChangeCheck())
                {
                    entry.Link = status;
                    ChangeFile.IsDirty = true;
                }
            }

            EditorGUI.BeginChangeCheck();
            bool embedded = EditorGUILayout.ToggleLeft("Embedded", entry.Embedded);

            if (EditorGUI.EndChangeCheck())
            {
                entry.Embedded = embedded;
                ChangeFile.IsDirty = true;
            }

            EditorGUILayout.Space();
            GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);

            bool remove = false;

            if (Style.MinusButton("Remove " + entry.FileName))
            {
                remove = EditorUtility.DisplayDialog("Remove File?", "Are you sure you want to remove " + entry.FileName + "?", "Remove", "Cancel");
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);

            return remove;
        }

        bool DrawStaticLibraryEntry(StaticLibraryEntry entry, float labelWidth, float indentWidth, bool isChild = false)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentWidth);
            Style.FixedWidthLabel(entry.FileName, entry.Path, labelWidth);

            if (FileAndFolderEntryFactory.Exists(entry.Path))
            {
                GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
            }
            else
            {
                Style.WarningIcon("File is missing");
            }

            if (isChild)
            {
                GUILayout.Space(ENUM_POPUP_WIDTH);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var method = (AddMethod) EditorGUILayout.EnumPopup(entry.Add, GUILayout.Width(ENUM_POPUP_WIDTH));

                if (EditorGUI.EndChangeCheck())
                {
                    entry.Add = method;
                    ChangeFile.IsDirty = true;
                }
            }

            EditorGUILayout.LabelField("Status", GUILayout.Width(ENUM_POPUP_WIDTH));
            EditorGUI.BeginChangeCheck();
            LinkType status = (LinkType) EditorGUILayout.EnumPopup(entry.Link, GUILayout.Width(ENUM_POPUP_WIDTH));

            if (EditorGUI.EndChangeCheck())
            {
                entry.Link = status;
                ChangeFile.IsDirty = true;
            }

            EditorGUILayout.Space();
            GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);

            bool remove = false;

            if (Style.MinusButton("Remove " + entry.FileName))
            {
                remove = EditorUtility.DisplayDialog("Remove File?", "Are you sure you want to remove " + entry.FileName + "?", "Remove", "Cancel");
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);

            return remove;
        }

        bool DrawFolderEntry(FolderEntry entry, int level, bool isChild = false)
        {
            EditorGUILayout.BeginHorizontal();
            float indent = level * FOLDER_INDENT;
            GUILayout.Space(indent);
            float labelWidth = MaxFolderSectionWidth - indent;
            Style.FixedWidthLabel(entry.FileName, entry.Path, labelWidth);

            if (FileAndFolderEntryFactory.Exists(entry.Path))
            {
                GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
            }
            else
            {
                Style.WarningIcon("Folder is missing");
            }

            if (isChild)
            {
                GUILayout.Space(ENUM_POPUP_WIDTH);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var method = (AddMethod) EditorGUILayout.EnumPopup(entry.Add, GUILayout.Width(ENUM_POPUP_WIDTH));

                if (EditorGUI.EndChangeCheck())
                {
                    entry.Add = method;
                    ChangeFile.IsDirty = true;
                }
            }

            EditorGUILayout.Space();

            if (isChild)
            {
                GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
            }
            else
            {
                if (Style.RefreshButton("Update the folder's contents"))
                {
                    entry.RefreshFolder();
                    UpdateMaxFolderWidth();
                    ChangeFile.IsDirty = true;
                }
            }

            bool remove = false;

            if (Style.MinusButton("Remove " + entry.FileName))
            {
                remove = EditorUtility.DisplayDialog("Remove Folder?", "Are you sure you want to remove " + entry.FileName + "?", "Remove", "Cancel");
            }

            EditorGUILayout.EndHorizontal();
            int removeIndex = -1;
            int nextLevel = level + 1;
            float nextIndentWidth = nextLevel * FOLDER_INDENT;
            float nextLabelWidth = MaxFolderSectionWidth - nextIndentWidth;

            if (DrawFileEntries(entry.Entries, nextLevel, nextLabelWidth, nextIndentWidth, out removeIndex))
            {
                entry.RemoveAt(removeIndex);
                ChangeFile.IsDirty = true;
                UpdateMaxFolderWidth();
            }

            return remove;
        }

        bool DrawFileEntries(BaseFileEntry[] entries, int level, float labelWidth, float indentWidth, out int removeIndex)
        {
            bool isChild = level > 0;

            bool remove = false;

            removeIndex = -1;

            for (int ii = 0; ii < entries.Length; ++ii)
            {
                var entry = entries[ii];

                if (entry is FolderEntry)
                {
                    remove = DrawFolderEntry(entries[ii] as FolderEntry, level, isChild);
                }
                else if (entry is FrameworkEntry)
                {
                    remove = DrawFrameworkEntry(entry as FrameworkEntry, labelWidth, indentWidth, isChild);
                }
                else if (entry is StaticLibraryEntry)
                {
                    remove = DrawStaticLibraryEntry(entry as StaticLibraryEntry, labelWidth, indentWidth, isChild);
                }
                else if (entry is SourceFileEntry)
                {
                    remove = DrawSourceFileEntry(entry as SourceFileEntry, labelWidth, indentWidth, isChild);
                }
                else if (entry is FileEntry)
                {
                    remove = DrawFileEntry(entry as FileEntry, labelWidth, indentWidth, isChild);
                }
                else
                {
                    //TODO error
                }

                if (remove)
                {
                    removeIndex = ii;
                }
            }

            return removeIndex > -1;
        }

        void DrawBuildSettingsSection()
        {
            if (!DrawFoldOut(Sections.BuildSettings))
            {
                return;
            }

            _drawEditBuildSetting = false;
            EditorGUI.indentLevel++;
            string description = "Set and add build settings";
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
            EditorGUI.indentLevel--;
            DrawLeft();
            _showBuildSettingRawValues = EditorGUILayout.Toggle("Show raw values", _showBuildSettingRawValues);
            var buildSettings = ChangeFile.BuildSettings;
            string[] displayNames;

            if (_showBuildSettingRawValues)
            {
                displayNames = buildSettings.Names;
            }
            else
            {
                displayNames = new string[buildSettings.Count];
                var settingNames = buildSettings.Names;

                for (int ii = 0; ii < settingNames.Length; ++ii)
                {
                    displayNames[ii] = XcodeBuildSettings.Instance().DisplayName(settingNames[ii]);
                }
            }

            float maxWidth = Style.MaxLabelWidth(displayNames, MIN_FOLDER_NAME_WIDTH, DEFAULT_FOLDER_NAME_WIDTH) + FUDGE_FACTOR;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.miniBoldLabel, GUILayout.MaxWidth(maxWidth + SQUARE_BUTTON_SPACE_WIDTH));
            EditorGUILayout.LabelField("Value", EditorStyles.miniBoldLabel);
            EditorGUILayout.EndHorizontal();
            Style.HorizontalLine();
            int toRemove = -1;

            for (int ii = 0; ii < buildSettings.Count; ++ii)
            {
                if (DrawBuildSettingEntry(ii, maxWidth, displayNames[ii]))
                {
                    toRemove = ii;
                }
            }

            if (toRemove > -1)
            {
                ChangeFile.BuildSettings.RemoveAt(toRemove);
                ChangeFile.IsDirty = true;
            }

            GUILayout.Space(6);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _drawAddBuildSetting = false;

            if (GUILayout.Button(new GUIContent("+ Build Setting", "Add build setting")))
            {
                _drawAddBuildSetting = true;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawRight();
            GUILayout.Space(VERTICAL_SPACE);
        }

        bool DrawBuildSettingEntry(int index, float maxWidth, string displayName)
        {

            bool remove = false;

            var buildSettings = ChangeFile.BuildSettings;
            BaseBuildSettingEntry entry = buildSettings.EntryAt(index);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(displayName, GUILayout.MaxWidth(maxWidth));

            if (entry is BoolBuildSettingEntry)
            {
                DrawBool(entry as BoolBuildSettingEntry);
            }
            else if (entry is StringBuildSettingEntry)
            {
                DrawString(entry as StringBuildSettingEntry);
            }
            else if (entry is EnumBuildSettingEntry)
            {
                DrawEnum(entry as EnumBuildSettingEntry);
            }
            else if (entry is CollectionBuildSettingEntry)
            {
                DrawArrayPart1(entry as CollectionBuildSettingEntry);
            }

            if (entry is CustomStringBuildSettingEntry)
            {
                if (Style.EditButton("Edit the build setting name"))
                {
                    _drawEditBuildSetting = true;
                    _buildSettingToEdit = entry.Name;
                }
            }
            else
            {
                GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
            }

            if (Style.MinusButton("Remove build setting"))
            {
                remove = EditorUtility.DisplayDialog("Remove Build Setting?", "Are you sure you want to remove \"" + displayName + "\"?", "Remove", "Cancel");
            }

            if (entry is CollectionBuildSettingEntry)
            {
                DrawArrayPart2(entry as CollectionBuildSettingEntry);
            }

            EditorGUILayout.EndHorizontal();

            return remove;
        }

        void DrawBool(BoolBuildSettingEntry entry)
        {
            BoolEnum b = entry.Value ? BoolEnum.Yes : BoolEnum.No;
            EditorGUI.BeginChangeCheck();
            b = (BoolEnum) EditorGUILayout.EnumPopup(b);

            if (EditorGUI.EndChangeCheck())
            {
                entry.Value = (b == BoolEnum.Yes);
                ChangeFile.IsDirty = true;
            }
        }

        void DrawEnum(EnumBuildSettingEntry entry)
        {
            EditorGUI.BeginChangeCheck();
            int index = EditorGUILayout.Popup(entry.SelectedIndex, entry.AcceptedValues);

            if (EditorGUI.EndChangeCheck())
            {
                entry.SelectedIndex = index;
                ChangeFile.IsDirty = true;
            }
        }

        void DrawString(StringBuildSettingEntry entry)
        {
            EditorGUI.BeginChangeCheck();
            string value = EditorGUILayout.TextField(entry.Value);

            if (EditorGUI.EndChangeCheck())
            {
                entry.Value = value;
                ChangeFile.IsDirty = true;
            }
        }

        void DrawArrayPart1(CollectionBuildSettingEntry entry)
        {
            EditorGUILayout.LabelField("Add Method", GUILayout.Width(90.0f));
            EditorGUI.BeginChangeCheck();
            entry.Merge = (MergeMethod) EditorGUILayout.EnumPopup(entry.Merge);

            if (EditorGUI.EndChangeCheck())
            {
                ChangeFile.IsDirty = true;
            }
        }

        void DrawArrayPart2(CollectionBuildSettingEntry entry)
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(25);
            EditorGUILayout.BeginVertical();
            int toRemove = -1;

            for (int ii = 0; ii < entry.Values.Count; ++ii)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var value = EditorGUILayout.TextField(entry.Values[ii]);

                if (EditorGUI.EndChangeCheck())
                {
                    entry.Values[ii] = value;
                    ChangeFile.IsDirty = true;
                }

                if (Style.MinusButton("Remove value"))
                {

                    bool remove = true;

                    if (!string.IsNullOrEmpty(entry.Values[ii]))
                    {
                        remove = EditorUtility.DisplayDialog("Remove Value?", "Are you sure you want to remove this value?", "Remove", "Cancel");
                    }

                    if (remove)
                    {
                        toRemove = ii;
                    }
                }

                GUILayout.Space(24);
                EditorGUILayout.EndHorizontal();
            }

            if (toRemove > -1)
            {
                entry.Values.RemoveAt(toRemove);
                ChangeFile.IsDirty = true;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (Style.PlusButton("Add value"))
            {
                entry.Values.Add("");
                ChangeFile.IsDirty = true;
            }

            GUILayout.Space(24);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        void DrawScriptsSection()
        {
            if (!DrawFoldOut(Sections.Scripts))
            {
                return;
            }

            EditorGUI.indentLevel++;
            string description = "Add post build scripts";
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
            EditorGUI.indentLevel--;
            int toRemove = -1;
            var scripts = ChangeFile.Scripts;
            DrawLeft();

            //Draw the list of files and folders to add to the project
            for (int ii = 0; ii < scripts.Count; ++ii)
            {
                if (DrawScriptEntry(ii))
                {
                    toRemove = ii;
                }
            }

            if (toRemove > -1)
            {
                scripts.RemoveAt(toRemove);
                ChangeFile.IsDirty = true;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("+ Script", "Add Script")))
            {
                scripts.AddScript();
                ChangeFile.IsDirty = true;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawRight();
            GUILayout.Space(VERTICAL_SPACE);
        }

        bool DrawScriptEntry(int index)
        {

            bool remove = false;

            var scripts = ChangeFile.Scripts;
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Name", GUILayout.Width(50));
            var newName = EditorGUILayout.TextField(scripts.NameAt(index));

            if (EditorGUI.EndChangeCheck())
            {
                scripts.SetNameAt(index, newName);
                ChangeFile.IsDirty = true;
            }

            if (Style.MinusButton("Remove script"))
            {
                remove = EditorUtility.DisplayDialog("Confirm Script Removal", "Are you sure you want to delete this script?", "Delete", "Cancel");
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Shell", GUILayout.Width(50));
            var newShell = EditorGUILayout.TextField(scripts.ShellAt(index));

            if (EditorGUI.EndChangeCheck())
            {
                scripts.SetShellAt(index, newShell);
                ChangeFile.IsDirty = true;
            }

            GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Script", GUILayout.Width(50));
            EditorGUI.BeginChangeCheck();
            var ta = GUI.skin.textArea;
            ta.stretchHeight = true;
            ta.fixedHeight = 0;
            var newScript = EditorGUILayout.TextArea(scripts.ScriptAt(index), ta, GUILayout.MinHeight(ta.lineHeight * 4), GUILayout.ExpandHeight(true));

            if (EditorGUI.EndChangeCheck())
            {
                scripts.SetScriptAt(index, newScript);
                ChangeFile.IsDirty = true;
            }

            GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
            EditorGUILayout.EndHorizontal();
            //TODO input and output files
            //TODO run on install only option
            GUILayout.Space(14);

            return remove;
        }

        void DrawSigningSection()
        {
            if (!DrawFoldOut(Sections.Signing))
            {
                return;
            }

            var signing = ChangeFile.Signing;
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel--;
            DrawLeft();
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200;
            EditorGUI.BeginChangeCheck();
            var prov = EditorGUILayout.Toggle("Automatically Manage Signing", signing.AutomaticProvisioning);

            if (EditorGUI.EndChangeCheck())
            {
                signing.AutomaticProvisioning = prov;
                ChangeFile.IsDirty = true;
            }

            EditorGUI.BeginChangeCheck();
            var teamId = EditorGUILayout.TextField("Development Team ID", signing.TeamId, GUILayout.MaxWidth(400));

            if (EditorGUI.EndChangeCheck())
            {
                signing.TeamId = teamId;
                ChangeFile.IsDirty = true;
            }

            EditorGUIUtility.labelWidth = labelWidth;
            DrawRight();
            GUILayout.Space(VERTICAL_SPACE);
        }

        void DrawCapabilitiesSection()
        {
            if (!DrawFoldOut(Sections.Capabilities))
            {
                return;
            }

            EditorGUI.indentLevel++;
            EditorGUI.indentLevel--;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(INDENT_SPACE);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Make sure you add activated Capbilities to your App ID in the Apple Developer Center or they will not work");
            EditorGUI.BeginChangeCheck();
            SystemCapability [] platformCapabilities;

            if (Parent.Platform == BuildPlatform.tvOS)
            {
                platformCapabilities = SystemCapabilityHelper.TVOSCapabilities;
            }
            else
            {
                platformCapabilities = SystemCapabilityHelper.IOSCapabilities;
            }

            foreach (var capability in platformCapabilities)
            {
                DrawCapabilityEntry(capability);
            }

            if (EditorGUI.EndChangeCheck())
            {
                ChangeFile.IsDirty = true;
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(RIGHT_SPACE);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);
        }

        void DrawCapabilityEntry(SystemCapability capability)
        {
            EditorGUILayout.BeginVertical(Style.Box(), GUILayout.MinHeight(EditorGUIUtility.singleLineHeight + 8));
            var capabilities = ChangeFile.Capabilities;
            bool enabled = capabilities.IsCapabilityEnabled(capability);
            string capabilityName = SystemCapabilityHelper.Name(capability);
            EditorGUI.BeginChangeCheck();
            enabled = EditorGUILayout.ToggleLeft(capabilityName, enabled, GUILayout.ExpandWidth(true));

            if (EditorGUI.EndChangeCheck())
            {
                capabilities.EnableCapability(capability, enabled);
            }

            if (enabled)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(INDENT_SPACE);
                EditorGUILayout.BeginVertical();
                var baseCap = capabilities.Capability(capability);

                if (baseCap != null)
                {
                    DrawCapability(capability, baseCap);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        void DrawCapability(SystemCapability systemCapability, BaseCapability capability)
        {
            EditorGUI.BeginChangeCheck();

            switch (systemCapability)
            {
            case SystemCapability.iCloud:
                DrawICloudCapability(capability as ICloudCapability);
                break;

            case SystemCapability.PushNotifications:
                DrawPushNotificationsCapability(capability as PushNotificationsCapability);
                break;

            case SystemCapability.GameCenter:
                DrawGameCenterCapability(capability as GameCenterCapability);
                break;

            case SystemCapability.Wallet:
                DrawWalletCapability(capability as WalletCapability);
                break;

            case SystemCapability.Siri:
                DrawSiriCapability(capability as SiriCapability);
                break;

            case SystemCapability.ApplePay:
                DrawApplePayCapability(capability as ApplePayCapability);
                break;

            case SystemCapability.InAppPurchase:
                DrawInAppPurchaseCapability(capability as InAppPurchaseCapability);
                break;

            case SystemCapability.Maps:
                DrawMapsCapability(capability as MapsCapability);
                break;

            case SystemCapability.GameControllers:
                DrawGameControllersCapability (capability as GameControllersCapability);
                break;

            case SystemCapability.PersonalVPN:
                DrawPersonalVPNCapability(capability as PersonalVPNCapability);
                break;

            case SystemCapability.NetworkExtensions:
                DrawNetworkExtensionsCapability(capability as NetworkExtensionsCapability);
                break;

            case SystemCapability.HotspotConfiguration:
                DrawHotspotConfigurationCapability (capability as HotspotConfigurationCapability);
                break;

            case SystemCapability.Multipath:
                DrawMultipathCapability (capability as MultipathCapability);
                break;

            case SystemCapability.NFCTagReading:
                DrawNFCTagReadingCapability (capability as NFCTagReadingCapability);
                break;

            case SystemCapability.BackgroundModes:
                DrawBackgroundModesCapability(capability as BackgroundModesCapability);
                break;

            case SystemCapability.InterAppAudio:
                DrawInterAppAudioCapability(capability as InterAppAudioCapability);
                break;

            case SystemCapability.KeychainSharing:
                DrawKeychainSharingCapability(capability as KeychainSharingCapability);
                break;

            case SystemCapability.AssociatedDomains:
                DrawAssociatedDomainsCapability(capability as AssociatedDomainsCapability);
                break;

            case SystemCapability.AppGroups:
                DrawAppGroupsCapability(capability as AppGroupsCapability);
                break;

            case SystemCapability.DataProtection:
                DrawDataProtectionapability(capability as DataProtectionapability);
                break;

            case SystemCapability.HomeKit:
                DrawHomeKitCapability(capability as HomeKitCapability);
                break;

            case SystemCapability.HealthKit:
                DrawHealthKitCapability(capability as HealthKitCapability);
                break;

            case SystemCapability.WirelessAccessoryConfiguration:
                DrawWirelessAccessoryConfigurationCapability(capability as WirelessAccessoryConfigurationCapability);
                break;

            default:
                throw new System.ArgumentOutOfRangeException();
            }

            if (EditorGUI.EndChangeCheck())
            {
                ChangeFile.IsDirty = true;
            }
        }

        //iCloud
        void DrawICloudCapability(ICloudCapability capability)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Services:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            capability.KeyValueStorage = EditorGUILayout.ToggleLeft("Key-value storage", capability.KeyValueStorage);

            if (Parent.Platform == BuildPlatform.iOS)
            {
                capability.iCloudDocuments = EditorGUILayout.ToggleLeft ("iCloud Documents", capability.iCloudDocuments);
            }

            EditorGUI.BeginChangeCheck();
            capability.CloudKit = EditorGUILayout.ToggleLeft("CloudKit", capability.CloudKit);

            if (EditorGUI.EndChangeCheck() && capability.CloudKit)
            {
                ChangeFile.Capabilities.EnableCapability(SystemCapability.PushNotifications, true);
            }

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Containers:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            GUI.enabled = capability.iCloudDocuments || capability.CloudKit;
            capability.UseCustomContainers = EditorGUILayout.ToggleLeft("Use Custom Containers", capability.UseCustomContainers);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(106);
            EditorGUILayout.BeginVertical();

            if (capability.UseCustomContainers)
            {
                DrawEditableStringList(capability.CustomContainers,
                                       "Custom Container",
                                       "iCloud.$(CFBundleIdentifier)"
                                      );
            }

            GUI.enabled = true;
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(2);

            if (GUILayout.Button("CloudKit Dashboard", GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL("https://icloud.developer.apple.com/dashboard/");
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);
        }

        //Push Notifications
        void DrawPushNotificationsCapability(PushNotificationsCapability capability)
        {
            //No additional user options required
        }

        //Game Center
        void DrawGameCenterCapability(GameCenterCapability capability)
        {
            //No additional user options required
        }

        //Wallet
        void DrawWalletCapability(WalletCapability capability)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Pass Types:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200;
            EditorGUI.BeginChangeCheck();
            capability.AllowSubsetOfPassTypes = EditorGUILayout.ToggleLeft("Allow subset of pass types", capability.AllowSubsetOfPassTypes, GUILayout.ExpandWidth(true));

            if (EditorGUI.EndChangeCheck())
            {
                ChangeFile.IsDirty = true;
            }

            EditorGUIUtility.labelWidth = labelWidth;
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUI.enabled = capability.AllowSubsetOfPassTypes;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(106);
            EditorGUILayout.BeginVertical();

            if (capability.AllowSubsetOfPassTypes)
            {
                DrawEditableStringList(capability.PassTypeSubsets,
                                       "Pass Type Subsets",
                                       "");
            }

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);
        }

        //Siri
        void DrawSiriCapability(SiriCapability capability)
        {
            //No additional user options required
        }

        //Apple Pay
        void DrawApplePayCapability(ApplePayCapability capability)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Merchant IDs:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            DrawEditableStringList(capability.MerchantIds,
                                   "Merchant Id",
                                   "merchant.");
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);
        }

        //In-App Purchase
        void DrawInAppPurchaseCapability(InAppPurchaseCapability capability)
        {
            //No additional user options required
        }

        //Maps
        void DrawMapsCapability(MapsCapability capability)
        {
            if (Parent.Platform == BuildPlatform.tvOS)
            {
                //no configuration on tvOS
                return;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Routing:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(100), GUILayout.ExpandWidth(false));
            capability.Airplane = EditorGUILayout.ToggleLeft("Airplane", capability.Airplane);
            capability.Bike = EditorGUILayout.ToggleLeft("Bike", capability.Bike);
            capability.Bus = EditorGUILayout.ToggleLeft("Bus", capability.Bus);
            capability.Car = EditorGUILayout.ToggleLeft("Car", capability.Car);
            capability.Ferry = EditorGUILayout.ToggleLeft("Ferry", capability.Ferry);
            capability.Pedestrian = EditorGUILayout.ToggleLeft("Pedestrian", capability.Pedestrian);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical(GUILayout.Width(100), GUILayout.ExpandWidth(false));
            capability.RideSharing = EditorGUILayout.ToggleLeft("Ride Sharing", capability.RideSharing);
            capability.Streetcar = EditorGUILayout.ToggleLeft("Streetcar", capability.Streetcar);
            capability.Subway = EditorGUILayout.ToggleLeft("Subway", capability.Subway);
            capability.Taxi = EditorGUILayout.ToggleLeft("Taxi", capability.Taxi);
            capability.Train = EditorGUILayout.ToggleLeft("Train", capability.Train);
            capability.Other = EditorGUILayout.ToggleLeft("Other", capability.Other);
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);

            if (EditorGUI.EndChangeCheck())
            {
                ChangeFile.IsDirty = true;
            }
        }

        //GameControllers
        void DrawGameControllersCapability (GameControllersCapability capability)
        {
            EditorGUI.BeginChangeCheck ();
            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.BeginVertical (GUILayout.Width (CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth (false));
            EditorGUILayout.LabelField ("Game Controllers:", GUILayout.Width (CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth (false));
            EditorGUILayout.EndVertical ();
            EditorGUILayout.BeginVertical ();
            var enabled = new List<GameControllersCapability.GameControllerType> ();
            var disabled = new List<GameControllersCapability.GameControllerType> ();

            if (capability.GameControllers != null && capability.GameControllers.Length > 0)
            {
                foreach (var c in capability.GameControllers)
                {
                    enabled.Add (c);
                }
            }

            var all = System.Enum.GetValues (typeof (GameControllersCapability.GameControllerType));

            foreach (GameControllersCapability.GameControllerType c in all)
            {
                if (!enabled.Contains (c))
                {
                    disabled.Add (c);
                }
            }

            var toRemove = GameControllersCapability.GameControllerType.ExtendedGamepad;
            var toAdd = GameControllersCapability.GameControllerType.ExtendedGamepad;
            var indexToMoveUp = -1;
            bool removeRquired = false, addRequired = false;
            var s = new Styling ();

            for (int ii = 0; ii < enabled.Count; ++ii)
            {
                var c = enabled [ii];
                EditorGUILayout.BeginHorizontal ();
                bool selected = EditorGUILayout.ToggleLeft (c.ToString (), true, GUILayout.ExpandWidth (false), GUILayout.Width (200));

                if (!selected)
                {
                    removeRquired = true;
                    toRemove = c;
                }

                if (ii == 0)
                {
                    GUILayout.Space (20);
                }
                else if (s.SquareButton ("^", "Move up"))
                {
                    indexToMoveUp = ii;
                }

                EditorGUILayout.Space ();
                EditorGUILayout.EndHorizontal ();
            }

            foreach (var c in disabled)
            {
                bool selected = EditorGUILayout.ToggleLeft (c.ToString (), false);

                if (selected)
                {
                    addRequired = true;
                    toAdd = c;
                }
            }

            if (removeRquired)
            {
                enabled.Remove (toRemove);
                disabled.Add (toRemove);
                ChangeFile.IsDirty = true;
            }

            if (addRequired)
            {
                enabled.Add (toAdd);
                disabled.Remove (toAdd);
                ChangeFile.IsDirty = true;
            }

            if (indexToMoveUp > 0)
            {
                var c = enabled [indexToMoveUp];
                enabled.RemoveAt (indexToMoveUp);
                enabled.Insert (indexToMoveUp - 1, c);
                ChangeFile.IsDirty = true;
            }

            GUILayout.Space (5);
            EditorGUILayout.LabelField ("With multiple controllers selected, use the arrow button to rearrange");
            EditorGUILayout.LabelField ("Note: Setting the controllers here will overwrite the Info.plist entry with these settings");
            EditorGUILayout.EndVertical ();
            EditorGUILayout.EndHorizontal ();
            GUILayout.Space (VERTICAL_SPACE);
            capability.GameControllers = enabled.ToArray ();
        }

        //Personal VPN
        void DrawPersonalVPNCapability(PersonalVPNCapability capability)
        {
            //No additional user options required
        }

        //Network Extensions
        void DrawNetworkExtensionsCapability(NetworkExtensionsCapability capability)
        {
            EditorGUI.BeginChangeCheck ();
            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.BeginVertical (GUILayout.Width (CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth (false));
            EditorGUILayout.LabelField ("Capabilities:", GUILayout.Width (CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth (false));
            EditorGUILayout.EndVertical ();
            EditorGUILayout.BeginVertical ();
            capability.AppProxy = EditorGUILayout.ToggleLeft ("App Proxy", capability.AppProxy);
            capability.ContentFilter = EditorGUILayout.ToggleLeft ("Content Filter", capability.ContentFilter);
            capability.PacketTunnel = EditorGUILayout.ToggleLeft ("Packet Tunnel", capability.PacketTunnel);
            capability.DNSProxy = EditorGUILayout.ToggleLeft ("DNS Proxy", capability.DNSProxy);
            GUILayout.Space (5);
            EditorGUILayout.EndVertical ();
            EditorGUILayout.EndHorizontal ();
            GUILayout.Space (VERTICAL_SPACE);

            if (EditorGUI.EndChangeCheck ())
            {
                ChangeFile.IsDirty = true;
            }
        }

        void DrawHotspotConfigurationCapability (HotspotConfigurationCapability capability)
        {
            //No additional user options required
        }

        void DrawMultipathCapability (MultipathCapability capability)
        {
            //No additional user options required
        }

        void DrawNFCTagReadingCapability (NFCTagReadingCapability capability)
        {
            //No additional user options required
        }

        //Background Modes
        void DrawBackgroundModesCapability(BackgroundModesCapability capability)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Modes:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            capability.AudioAirplayPIP = EditorGUILayout.ToggleLeft("Audio, Airplay, and Picture in Picture", capability.AudioAirplayPIP);

            if (Parent.Platform == BuildPlatform.iOS)
            {
                capability.LocationUpdates = EditorGUILayout.ToggleLeft ("Location updates", capability.LocationUpdates);
                capability.NewsstandDownloads = EditorGUILayout.ToggleLeft ("Newstand downloads", capability.NewsstandDownloads);
            }

            capability.ExternalAccComms = EditorGUILayout.ToggleLeft("External accessory communication", capability.ExternalAccComms);

            if (Parent.Platform == BuildPlatform.iOS)
            {
                capability.UsesBTLEAcc = EditorGUILayout.ToggleLeft ("Uses Bluetooth LE accessories", capability.UsesBTLEAcc);
                capability.ActsAsBTLEAcc = EditorGUILayout.ToggleLeft ("Acts as a Bluetooth LE accessory", capability.ActsAsBTLEAcc);
            }

            capability.BackgroundFetch = EditorGUILayout.ToggleLeft("Background fetch", capability.BackgroundFetch);
            capability.RemoteNotifications = EditorGUILayout.ToggleLeft("Remote notifications", capability.RemoteNotifications);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);

            if (EditorGUI.EndChangeCheck())
            {
                ChangeFile.IsDirty = true;
            }
        }

        //Inter-App Audio
        void DrawInterAppAudioCapability(InterAppAudioCapability capability)
        {
            //No additional user options required
        }

        //Keychain Sharing
        void DrawKeychainSharingCapability(KeychainSharingCapability capability)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Keychain Groups:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            DrawEditableStringList(capability.KeychainGroups,
                                   "Keychain Group",
                                   "");
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);
        }

        //Associated Domains
        void DrawAssociatedDomainsCapability(AssociatedDomainsCapability capability)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Domains:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            DrawEditableStringList(capability.AssociatedDomains,
                                   "Associated Domain",
                                   "webcredentials:example.com");
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);
        }

        //App Groups
        void DrawAppGroupsCapability(AppGroupsCapability capability)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("App Groups:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            DrawEditableStringList(capability.AppGroups,
                                   "App Group",
                                   "group.");
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(VERTICAL_SPACE);
        }

        //Data Protection
        void DrawDataProtectionapability(DataProtectionapability capability)
        {
            //No additional user options required
        }

        //HomeKit
        void DrawHomeKitCapability(HomeKitCapability capability)
        {
            //No additional user options required
        }

        //HealthKit
        void DrawHealthKitCapability(HealthKitCapability capability)
        {
            //No additional user options required
        }

        //Wireless Accessory Configuration
        void DrawWirelessAccessoryConfigurationCapability(WirelessAccessoryConfigurationCapability capability)
        {
            //No additional user options required
        }

        void DrawEditableStringList(List<string> list, string commonName = "Item", string defaultAddName = "")
        {
            EditorGUILayout.BeginHorizontal(Style.Box());
            EditorGUILayout.BeginVertical();
            int toRemove = -1;

            for (int ii = 0; ii < list.Count; ++ii)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(list[ii], GUILayout.ExpandWidth (true));

                if (Style.EditButton("Edit " + commonName))
                {
                    Vector2 position = Parent.position.center;
                    TextEditPopover.Init(position,
                                         400,
                                         "Edit " + commonName,
                                         "You will also need to add it to your App ID on the Apple Developer website.",
                                         list[ii],
                                         (string oldText, string newText) =>
                    {
                        if (RenameStringInList(list, oldText, newText))
                        {
                            ChangeFile.IsDirty = true;
                        }
                    },
                    (string textToValidate) =>
                    {
                        return ValidateStringInList(list, textToValidate);
                    },
                    "Update",
                    Style);
                }

                if (Style.MinusButton("Remove " + commonName))
                {

                    bool remove = true;

                    if (!string.IsNullOrEmpty(list[ii]))
                    {
                        remove = EditorUtility.DisplayDialog("Remove Value?", "Are you sure you want to remove this value?", "Remove", "Cancel");
                    }

                    if (remove)
                    {
                        toRemove = ii;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            if (toRemove > -1)
            {
                list.RemoveAt(toRemove);
                ChangeFile.IsDirty = true;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (Style.PlusButton("Add " + commonName))
            {
                Vector2 position = Parent.position.center;
                TextEditPopover.Init(position,
                                     400,
                                     "Add " + commonName,
                                     "You will also need to add it to your App ID on the Apple Developer website.",
                                     defaultAddName,
                                     (string oldText, string newText) =>
                {
                    if (AddStringToList(list, newText))
                    {
                        ChangeFile.IsDirty = true;
                    }
                },
                (string textToValidate) =>
                {
                    return ValidateStringInList(list, textToValidate);
                },
                "Add",
                Style);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        bool AddStringToList(List<string> list, string newText)
        {
            if (!ValidateStringInList(list, newText))
            {
                return false;
            }

            list.Add(newText);
            return true;
        }

        bool RenameStringInList(List<string> list, string oldText, string newText)
        {
            if (!ValidateStringInList(list, newText))
            {
                return false;
            }

            if (string.IsNullOrEmpty(oldText))
            {
                return false;
            }

            //old may exist
            var index = list.IndexOf(oldText);

            if (index > -1)
            {
                list[index] = newText;
                return true;
            }
            else
            {
                list.Add(newText);
                return true;
            }
        }

        bool ValidateStringInList(List<string> list, string textToValidate)
        {
            //must not be empty
            if (string.IsNullOrEmpty(textToValidate))
            {
                return false;
            }

            //no dupes
            if (list.Contains(textToValidate))
            {
                return false;
            }

            return true;
        }

        public bool IsDirty
        {
            get
            {
                if (ChangeFile != null)
                {
                    return ChangeFile.IsDirty;
                }

                return false;
            }
        }

        public void Save()
        {
            if (ChangeFile != null)
            {
                ChangeFile.Save();
                _plistDrawer.IsDirty = false;
            }
        }

        #region frameworks callbacks

        void AddFramework(string name)
        {
            if (ChangeFile == null)
            {
                return;
            }

            ChangeFile.Frameworks.Add(name);
            ChangeFile.IsDirty = true;

            if (RepaintRequired != null)
            {
                RepaintRequired();
            }
        }

        #endregion

        void AddBuildSetting(string name)
        {
            ChangeFile.BuildSettings.Add(name);
            ChangeFile.IsDirty = true;

            if (RepaintRequired != null)
            {
                RepaintRequired();
            }
        }

        void EditBuildSetting(string name)
        {
            ChangeFile.BuildSettings.EditCustomBuildSetting(_buildSettingToEdit, name);
            ChangeFile.IsDirty = true;

            if (RepaintRequired != null)
            {
                RepaintRequired();
            }
        }

        public override void Refresh()
        {
            ChangeFile = null;
            LoadChangeFile();
        }
    }
}
