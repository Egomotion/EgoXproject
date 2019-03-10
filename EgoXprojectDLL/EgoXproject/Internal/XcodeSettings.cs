// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    /// <summary>
    /// Xcode settings.
    /// Stores the settings for egoXproject
    /// </summary>
    internal class XcodeSettings
    {
        const string SETTINGS_FILENAME = "egoxproject.settings";

        const string TYPE_KEY = "Type";
        const string TYPE_VALUE = "EgoXproject Settings";

        const string VERSION_KEY = "Version";
        const string AUTORUN_KEY = "Enabled";
        const string IGNORE_KEY = "Ignore";

        const int VERSION = 3;
        int[] _supportedVersions = { 1, 2 };

        bool _autoRun = true;

        string _savePath;

        public XcodeSettings(string defaultSaveLocation)
        {
            _savePath = Path.Combine(defaultSaveLocation, SETTINGS_FILENAME);
            Load();
        }

        public bool AutoRunEnabled
        {
            get
            {
                return _autoRun;
            }
            set
            {
                if (value != _autoRun)
                {
                    IsDirty = true;
                }

                _autoRun = value;
            }
        }

        public bool IsDirty
        {
            get;
            private set;
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public void Save()
        {
            var plist = new PList();
            plist.Root.Add(TYPE_KEY, TYPE_VALUE);
            plist.Root.Add(VERSION_KEY, VERSION);
            plist.Root.Add(AUTORUN_KEY, _autoRun);
            plist.Root.Add(IGNORE_KEY, new PListArray(IgnoredFiles.CustomList));
            bool assetImport = !File.Exists(_savePath);

            if (plist.Save(_savePath, true))
            {
                IsDirty = false;

                if (assetImport)
                {
                    AssetDatabase.ImportAsset(ProjectUtil.MakePathRelativeToProject(_savePath));
                }
            }
        }

        void Load()
        {
            if (!File.Exists(_savePath))
            {
                _savePath = FindSettingsFileLocation();
            }

            if (File.Exists(_savePath))
            {
                PList p = new PList();

                if (p.Load(_savePath))
                {
                    if (Validate(p))
                    {
                        ParseSettingsFile(p);
                        return;
                    }
                }

                //fall through to fail mode
                var backupPath = _savePath + ".corrupted-" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                Debug.LogWarning("EgoXproject: Corrupted settings found. Recreating.");
                Debug.LogWarning("EgoXproject: Corrupted file backed up to " + backupPath);
                File.Move(_savePath, backupPath);
            }

            //fill in empty values
            CreateDefaultSettings();
        }

        string FindSettingsFileLocation()
        {
            if (string.IsNullOrEmpty(_savePath))
            {
                _savePath = Application.dataPath;
            }

            var files = Directory.GetFiles(Application.dataPath, SETTINGS_FILENAME, SearchOption.AllDirectories);

            //no settings file. use default loaction
            if (files == null || files.Length <= 0)
            {
                return _savePath;
            }

            if (files.Length > 1)
            {
                Debug.LogWarning("EgoXproject: Multiple Settings files found!. Using " + files[0]);
                Debug.LogWarning("EgoXproject: Ignoring the following files. Please remove the incorrect files.");

                for (int ii = 1; ii < files.Length; ++ii)
                {
                    Debug.LogWarning(files[ii]);
                }
            }

            return files[0];
        }

        void CreateDefaultSettings()
        {
            _autoRun = true;
            IsDirty = false;
        }

        bool Validate(PList settingsPlist)
        {
            var typeValue = settingsPlist.Root.StringValue(TYPE_KEY);

            if (string.IsNullOrEmpty(typeValue) || typeValue != TYPE_VALUE)
            {
                return false;
            }

            var version = settingsPlist.Root.IntValue(VERSION_KEY);

            if (version != VERSION && !_supportedVersions.Contains(version))
            {
                return false;
            }

            if (settingsPlist.Root.Element<PListBoolean>(AUTORUN_KEY) == null)
            {
                return false;
            }

            if (settingsPlist.Root.Element<PListArray>(IGNORE_KEY) == null)
            {
                //return false;
            }

            //TODO verify the contents of the dictionary?
            return true;
        }

        void ParseSettingsFile(PList plist)
        {
            _autoRun = plist.Root.BoolValue(AUTORUN_KEY);
            var ignoredFiles = plist.Root.ArrayValue(IGNORE_KEY);

            if (ignoredFiles != null)
            {
                var files = ignoredFiles.ToStringArray();
                IgnoredFiles.SetIngnoredFiles(files);
            }

            IsDirty = false;
            var version = plist.Root.Element<PListInteger>(VERSION_KEY);

            if (version.IntValue != VERSION)
            {
                version.IntValue = VERSION;
                IsDirty = true;
            }
        }
    }
}
