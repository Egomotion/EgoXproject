// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using Egomotion.EgoXproject.Internal;
using Egomotion.EgoXproject.UI.Internal;
using System.IO;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.UI
{
    internal class PreviewTab : BaseChangeFileDrawer
    {
        Vector2 _scrollPosition = Vector2.zero;
        PListElementDrawer _plistDrawer;

        public PreviewTab (XcodeEditorWindow parent, Styling style)
        : base(parent, style)
        {
            _plistDrawer = new PListElementDrawer(style);
        }

        public override void Draw()
        {
            if (ChangeFile == null)
            {
                LoadMergedFile();
            }

            DrawHeader();
            GUILayout.Space(10.0f);
            Style.HorizontalLine();
            DrawMainView();
        }

        void LoadMergedFile()
        {
            ChangeFile = XcodeController.Instance().MergedChanges(Parent.Platform);
            UpdateMaxFolderWidth();
            SetFoldout(Sections.InfoPlist, ChangeFile.InfoPlistChanges.Count > 0);
            SetFoldout(Sections.Frameworks, ChangeFile.Frameworks.HasChanges());
            SetFoldout(Sections.FilesFolders, ChangeFile.FilesAndFolders.HasChanges());
            SetFoldout(Sections.BuildSettings, ChangeFile.BuildSettings.HasChanges());
            SetFoldout(Sections.Signing, ChangeFile.Signing.HasChanges());
            SetFoldout(Sections.Scripts, ChangeFile.Scripts.HasChanges());
            SetFoldout(Sections.Capabilities, ChangeFile.Capabilities.HasChanges());
        }

        public override void Refresh()
        {
            ChangeFile = null;
            _plistDrawer.Reset();
        }

        void DrawHeader()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("The combined changes from all changes files in the active configuration. These will be applied to the Xcode project.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);
            var activeConfigLabel = new GUIContent("Active Configuration");
            float width = EditorStyles.boldLabel.CalcSize(activeConfigLabel).x;
            var platformConfiguration = XcodeController.Instance().Configuration(Parent.Platform);
            var configs = platformConfiguration.Configurations;
            var activeConfig = platformConfiguration.ActiveConfiguration;
            int activeIndex = System.Array.IndexOf(configs, activeConfig);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(activeConfigLabel, EditorStyles.boldLabel, GUILayout.Width(width));

            if (activeIndex < 0)
            {
                EditorGUILayout.LabelField(activeConfig);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                activeIndex = EditorGUILayout.Popup(activeIndex, configs);

                if (EditorGUI.EndChangeCheck())
                {
                    platformConfiguration.ActiveConfiguration = configs[activeIndex];
                    LoadMergedFile();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
        }


        void DrawMainView()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawInfoPlistSection();
            GUILayout.Space(VERTICAL_SPACE);
            DrawFrameworksSection();
            GUILayout.Space(VERTICAL_SPACE);
            DrawFilesAndFoldersSection();
            GUILayout.Space(VERTICAL_SPACE);
            DrawBuildSettingsSection();
            GUILayout.Space(VERTICAL_SPACE);
            DrawSigningSection();
            GUILayout.Space(VERTICAL_SPACE);
            DrawCapabilitiesSection();
            GUILayout.Space(VERTICAL_SPACE);
            DrawScriptsSection();
            GUILayout.FlexibleSpace ();
            EditorGUILayout.EndScrollView();
        }

        void DrawInfoPlistSection()
        {
            if (!DrawFoldOut(Sections.InfoPlist))
            {
                return;
            }

            DrawLeft();
            var dict = ChangeFile.InfoPlistChanges;
            _plistDrawer.DrawPList(dict);
            DrawRight();
        }

        void DrawFrameworksSection()
        {
            if (!DrawFoldOut(Sections.Frameworks))
            {
                return;
            }

            DrawLeft();
            var frameworks = ChangeFile.Frameworks;
            float maxWidth = Style.MaxLabelWidth(frameworks.Names, MIN_FOLDER_NAME_WIDTH, DEFAULT_FOLDER_NAME_WIDTH) + FUDGE_FACTOR;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.miniBoldLabel, GUILayout.MaxWidth(maxWidth + SQUARE_BUTTON_SPACE_WIDTH));
            EditorGUILayout.LabelField("Status", EditorStyles.miniBoldLabel, GUILayout.MaxWidth(ENUM_POPUP_WIDTH));
            EditorGUILayout.EndHorizontal();
            Style.HorizontalLine();

            for (int ii = 0; ii < frameworks.Count; ++ii)
            {
                string f = frameworks.FileNameAt(ii);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(f, GUILayout.Width(maxWidth));
                GUILayout.Space(SQUARE_BUTTON_SPACE_WIDTH);
                EditorGUILayout.LabelField(frameworks.LinkTypeAt(ii).ToString(), GUILayout.Width(ENUM_POPUP_WIDTH));
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
            }

            DrawRight();
        }

        void DrawFilesAndFoldersSection()
        {
            if (!DrawFoldOut(Sections.FilesFolders))
            {
                return;
            }

            DrawLeft();
            var filesFolders = ChangeFile.FilesAndFolders;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.miniBoldLabel, GUILayout.MaxWidth(MaxFolderSectionWidth + SQUARE_BUTTON_SPACE_WIDTH));
            EditorGUILayout.LabelField("Add Method", EditorStyles.miniBoldLabel, GUILayout.MaxWidth(ENUM_POPUP_WIDTH));
            EditorGUILayout.LabelField("Options", EditorStyles.miniBoldLabel);
            EditorGUILayout.EndHorizontal();
            Style.HorizontalLine();
            //Draw the list of files and folders to add to the project
            DrawFileEntries(filesFolders.Entries, 0, MaxFolderSectionWidth, 0);
            DrawRight();
        }

        void DrawFileEntry(FileEntry entry, float labelWidth, float indentWidth, bool isChild = false)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentWidth);
            var label = new GUIContent(entry.FileName, entry.Path);
            EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));

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
                EditorGUILayout.LabelField(entry.Add.ToString(), GUILayout.Width(ENUM_POPUP_WIDTH));
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        void DrawSourceFileEntry(SourceFileEntry entry, float labelWidth, float indentWidth, bool isChild = false)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentWidth);
            var label = new GUIContent(entry.FileName, entry.Path);
            EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));

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
                EditorGUILayout.LabelField(entry.Add.ToString(), GUILayout.Width(ENUM_POPUP_WIDTH));
            }

            EditorGUILayout.LabelField(entry.CompilerFlags);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        void DrawFrameworkEntry(FrameworkEntry entry, float labelWidth, float indentWidth, bool isChild = false)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentWidth);
            var label = new GUIContent(entry.FileName, entry.Path);
            EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));

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

                var add = entry.Embedded ? AddMethod.Copy : entry.Add;

                EditorGUILayout.LabelField(add.ToString(), GUILayout.Width(ENUM_POPUP_WIDTH));
            }

            var link = entry.Embedded ? LinkType.Required : entry.Link;
            EditorGUILayout.LabelField(link.ToString(), GUILayout.Width(ENUM_POPUP_WIDTH));

            if (entry.Embedded)
            {
                EditorGUILayout.LabelField("Embedded", GUILayout.Width(ENUM_POPUP_WIDTH));
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        void DrawStaticLibraryEntry(StaticLibraryEntry entry, float labelWidth, float indentWidth, bool isChild = false)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentWidth);
            var label = new GUIContent(entry.FileName, entry.Path);
            EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));

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
                EditorGUILayout.LabelField(entry.Add.ToString(), GUILayout.Width(ENUM_POPUP_WIDTH));
            }

            EditorGUILayout.LabelField(entry.Link.ToString(), GUILayout.Width(ENUM_POPUP_WIDTH));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        void DrawFolderEntry(FolderEntry entry, int level, bool isChild = false)
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
                EditorGUILayout.LabelField(entry.Add.ToString(), GUILayout.Width(ENUM_POPUP_WIDTH));
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            int nextLevel = level + 1;
            float nextIndentWidth = nextLevel * FOLDER_INDENT;
            float nextLabelWidth = MaxFolderSectionWidth - nextIndentWidth;
            DrawFileEntries(entry.Entries, nextLevel, nextLabelWidth, nextIndentWidth);
        }

        void DrawFileEntries(BaseFileEntry[] entries, int level, float labelWidth, float indentWidth)
        {
            bool isChild = level > 0;

            for (int ii = 0; ii < entries.Length; ++ii)
            {
                var entry = entries[ii];

                if (entry is FolderEntry)
                {
                    DrawFolderEntry(entries[ii] as FolderEntry, level, isChild);
                }
                else if (entry is FrameworkEntry)
                {
                    DrawFrameworkEntry(entry as FrameworkEntry, labelWidth, indentWidth, isChild);
                }
                else if (entry is StaticLibraryEntry)
                {
                    DrawStaticLibraryEntry(entry as StaticLibraryEntry, labelWidth, indentWidth, isChild);
                }
                else if (entry is SourceFileEntry)
                {
                    DrawSourceFileEntry(entry as SourceFileEntry, labelWidth, indentWidth, isChild);
                }
                else if (entry is FileEntry)
                {
                    DrawFileEntry(entry as FileEntry, labelWidth, indentWidth, isChild);
                }
                else
                {
                    //TODO error
                }
            }
        }

        void DrawBuildSettingsSection()
        {
            if (!DrawFoldOut(Sections.BuildSettings))
            {
                return;
            }

            DrawLeft();
            var buildSettings = ChangeFile.BuildSettings;
            float maxWidth = Style.MaxLabelWidth(buildSettings.Names, MIN_FOLDER_NAME_WIDTH, DEFAULT_FOLDER_NAME_WIDTH) + FUDGE_FACTOR;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.miniBoldLabel, GUILayout.MaxWidth(maxWidth));
            EditorGUILayout.LabelField("Value", EditorStyles.miniBoldLabel);
            EditorGUILayout.EndHorizontal();
            Style.HorizontalLine();

            for (int ii = 0; ii < buildSettings.Count; ++ii)
            {
                BaseBuildSettingEntry entry = buildSettings.EntryAt(ii);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(entry.Name, GUILayout.MaxWidth(maxWidth));

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
                    DrawArray(entry as CollectionBuildSettingEntry);
                }

                EditorGUILayout.EndHorizontal();
            }

            DrawRight();
        }

        void DrawBool(BoolBuildSettingEntry entry)
        {
            BoolEnum b = entry.Value ? BoolEnum.Yes : BoolEnum.No;
            EditorGUILayout.LabelField(b.ToString());
        }

        void DrawEnum(EnumBuildSettingEntry entry)
        {
            EditorGUILayout.LabelField(entry.Value);
        }

        void DrawString(StringBuildSettingEntry entry)
        {
            EditorGUILayout.LabelField(entry.Value);
        }

        void DrawArray(CollectionBuildSettingEntry entry)
        {
            string label = "";

            switch (entry.Merge)
            {
            case MergeMethod.Append:
                label = "Will append values to existing values";
                break;

            case MergeMethod.Replace:
            default:
                label = "Will replace existing values";
                break;
            }

            EditorGUILayout.LabelField(label);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            GUILayout.Space(2);

            for (int ii = 0; ii < entry.Values.Count; ++ii)
            {
                EditorGUILayout.LabelField(entry.Values[ii]);
            }

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUILayout.BeginHorizontal();
        }

        void DrawScriptsSection()
        {
            if (!DrawFoldOut(Sections.Scripts))
            {
                return;
            }

            var scripts = ChangeFile.Scripts;
            GUIStyle scriptStyle = new GUIStyle(GUI.skin.textArea);
            scriptStyle.wordWrap = true;

            //Draw the list of files and folders to add to the project
            for (int ii = 0; ii < scripts.Count; ++ii)
            {
                DrawLeft();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name", GUILayout.Width(50));
                EditorGUILayout.LabelField(scripts.NameAt(ii));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Shell", GUILayout.Width(50));
                EditorGUILayout.LabelField(scripts.ShellAt(ii));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField(scripts.ScriptAt(ii), scriptStyle);
                DrawRight();
                GUILayout.Space(6);
            }
        }

        void DrawSigningSection()
        {
            if (!DrawFoldOut(Sections.Signing))
            {
                return;
            }

            var signing = ChangeFile.Signing;
            DrawLeft();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Automatically Manage Signing", GUILayout.Width(200));
            EditorGUILayout.LabelField(signing.AutomaticProvisioning ? "Enabled" : "Disabled");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Development Team ID", GUILayout.Width(200));
            EditorGUILayout.LabelField(signing.TeamId);
            EditorGUILayout.EndHorizontal();
            DrawRight();
        }

        #region Capabilities

        void DrawCapabilitiesSection()
        {
            if (!DrawFoldOut(Sections.Capabilities))
            {
                return;
            }

            var capabilities = ChangeFile.Capabilities;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(INDENT_SPACE);
            EditorGUILayout.BeginVertical();
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
                bool enabled = capabilities.IsCapabilityEnabled(capability);

                if (!enabled)
                {
                    continue;
                }

                EditorGUILayout.BeginVertical(Style.Box());
                string capabilityName = SystemCapabilityHelper.Name(capability);
                EditorGUILayout.LabelField(capabilityName, GUILayout.ExpandWidth(true));
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
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(RIGHT_SPACE);
            EditorGUILayout.EndHorizontal();
        }

        void DrawCapability(SystemCapability systemCapability, BaseCapability capability)
        {
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
        }

        //iCloud
        void DrawICloudCapability(ICloudCapability capability)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Services:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();

            if (capability.KeyValueStorage)
            {
                EditorGUILayout.LabelField("Key-value storage");
            }

            if (capability.iCloudDocuments)
            {
                EditorGUILayout.LabelField("iCloud Documents");
            }

            if (capability.CloudKit)
            {
                EditorGUILayout.LabelField("CloudKit");
            }

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            bool drawContainers = (capability.iCloudDocuments || capability.CloudKit) && capability.UseCustomContainers;

            if (drawContainers)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField("Containers:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();

                if (capability.UseCustomContainers)
                {
                    EditorGUILayout.LabelField("Use Custom Containers");
                }

                GUILayout.Space(5);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(106);
                EditorGUILayout.BeginVertical();

                if (capability.UseCustomContainers)
                {
                    DrawStringList(capability.CustomContainers);
                }

                GUILayout.Space(5);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
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

            if (capability.AllowSubsetOfPassTypes)
            {
                EditorGUILayout.LabelField("Allow subset of pass types", GUILayout.ExpandWidth(true));
            }

            EditorGUIUtility.labelWidth = labelWidth;
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(106);
            EditorGUILayout.BeginVertical();

            if (capability.AllowSubsetOfPassTypes)
            {
                DrawStringList(capability.PassTypeSubsets);
            }

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
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
            DrawStringList(capability.MerchantIds);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        //In-App Purchase
        void DrawInAppPurchaseCapability(InAppPurchaseCapability capability)
        {
            //No additional user options required
        }

        //Maps
        void DrawMapsCapability(MapsCapability capability)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Routing:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(100), GUILayout.ExpandWidth(false));

            if (capability.Airplane) { EditorGUILayout.LabelField("Airplane"); }

            if (capability.Bike) { EditorGUILayout.LabelField("Bike"); }

            if (capability.Bus) { EditorGUILayout.LabelField("Bus"); }

            if (capability.Car) { EditorGUILayout.LabelField("Car"); }

            if (capability.Ferry) { EditorGUILayout.LabelField("Ferry"); }

            if (capability.Pedestrian) { EditorGUILayout.LabelField("Pedestrian"); }

            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical(GUILayout.Width(100), GUILayout.ExpandWidth(false));

            if (capability.RideSharing) { EditorGUILayout.LabelField("Ride Sharing"); }

            if (capability.Streetcar) { EditorGUILayout.LabelField("Streetcar"); }

            if (capability.Subway) { EditorGUILayout.LabelField("Subway"); }

            if (capability.Taxi) { EditorGUILayout.LabelField("Taxi"); }

            if (capability.Train) { EditorGUILayout.LabelField("Train"); }

            if (capability.Other) { EditorGUILayout.LabelField("Other"); }

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        void DrawGameControllersCapability (GameControllersCapability capability)
        {
            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.BeginVertical (GUILayout.Width (CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth (false));
            EditorGUILayout.LabelField ("Game Controllers:", GUILayout.Width (CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth (false));
            EditorGUILayout.EndVertical ();
            EditorGUILayout.BeginVertical ();

            if (capability.GameControllers != null && capability.GameControllers.Length > 0)
            {
                foreach (var c in capability.GameControllers)
                {
                    EditorGUILayout.LabelField (c.ToString ());
                }
            }

            GUILayout.Space (5);
            EditorGUILayout.EndVertical ();
            EditorGUILayout.EndHorizontal ();
            GUILayout.Space (10);
        }

        //Personal VPN
        void DrawPersonalVPNCapability(PersonalVPNCapability capability)
        {
            //No additional user options required
        }

        //Network Extensions
        void DrawNetworkExtensionsCapability(NetworkExtensionsCapability capability)
        {
            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.BeginVertical (GUILayout.Width (CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth (false));
            EditorGUILayout.LabelField ("Capabilities:", GUILayout.Width (CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth (false));
            EditorGUILayout.EndVertical ();
            EditorGUILayout.BeginVertical ();

            if (capability.AppProxy) { EditorGUILayout.LabelField ("App Proxy"); }

            if (capability.ContentFilter) { EditorGUILayout.LabelField ("Content Filter"); }

            if (capability.PacketTunnel) { EditorGUILayout.LabelField ("Packet Tunnel"); }

            if (capability.DNSProxy) { EditorGUILayout.LabelField ("DNS Proxy"); }

            GUILayout.Space (5);
            EditorGUILayout.EndVertical ();
            EditorGUILayout.EndHorizontal ();
            GUILayout.Space (10);
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
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Modes:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();

            if (capability.AudioAirplayPIP) { EditorGUILayout.LabelField("Audio, Airplay, and Picture in Picture"); }

            if (capability.LocationUpdates) { EditorGUILayout.LabelField("Location updates"); }

            if (capability.VOIP) { EditorGUILayout.LabelField("Voice over IP"); }

            if (capability.NewsstandDownloads) { EditorGUILayout.LabelField("Newstand downloads"); }

            if (capability.ExternalAccComms) { EditorGUILayout.LabelField("External accessory communication"); }

            if (capability.UsesBTLEAcc) { EditorGUILayout.LabelField("Uses Bluetooth LE accessories"); }

            if (capability.ActsAsBTLEAcc) { EditorGUILayout.LabelField("Acts as a Bluetooth LE accessory"); }

            if (capability.BackgroundFetch) { EditorGUILayout.LabelField("Background fetch"); }

            if (capability.RemoteNotifications) { EditorGUILayout.LabelField("Remote notifications"); }

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
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
            DrawStringList(capability.KeychainGroups);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        //Associated Domains
        void DrawAssociatedDomainsCapability(AssociatedDomainsCapability capability)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Domains:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            DrawStringList(capability.AssociatedDomains);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        //App Groups
        void DrawAppGroupsCapability(AppGroupsCapability capability)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("App Groups:", GUILayout.Width(CAPABILITY_FIRST_COLUMN_WIDTH), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            DrawStringList(capability.AppGroups);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
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

        void DrawStringList(List<string> list)
        {
            if (list == null || list.Count <= 0)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(Style.Box());

            for (int ii = 0; ii < list.Count; ++ii)
            {
                EditorGUILayout.LabelField(list[ii]);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(40);
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}
