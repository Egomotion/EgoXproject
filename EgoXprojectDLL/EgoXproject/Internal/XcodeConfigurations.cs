// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace Egomotion.EgoXproject.Internal
{
    internal class XcodeConfigurations
    {
        const string CONFIG_FILENAME = "egoxproject.configurations";
        const string TYPE_KEY = "Type";
        const string TYPE_VALUE = "EgoXproject Configurations";

        const string VERSION_KEY = "Version";

        const int VERSION = 1;
        int[] _supportedVersions = { };

        string _savePath;

        PlatformConfiguration _iosConfigs  = new PlatformConfiguration();
        PlatformConfiguration _tvosConfigs = new PlatformConfiguration();

        public XcodeConfigurations(string defaultSaveLocation)
        {
            _savePath = Path.Combine(defaultSaveLocation, CONFIG_FILENAME);
            Load();
        }

        public PlatformConfiguration Configuration(BuildPlatform platform)
        {
            switch (platform)
            {
            case BuildPlatform.iOS:
                return _iosConfigs;

            case BuildPlatform.tvOS:
                return _tvosConfigs;

            default:
                throw new System.ArgumentException("EgoXproject: Not a valid build platform for configuration selection");
            }
        }

        public bool IsDirty
        {
            get
            {
                return _iosConfigs.IsDirty || _tvosConfigs.IsDirty;
            }
            set
            {
                _iosConfigs.IsDirty = value;
                _tvosConfigs.IsDirty = value;
            }
        }

        public void Save()
        {
            var plist = new PList();
            plist.Root.Add(TYPE_KEY, TYPE_VALUE);
            plist.Root.Add(VERSION_KEY, VERSION);
            plist.Root.Add(BuildPlatform.iOS.ToString(), _iosConfigs.Serialize());
            plist.Root.Add(BuildPlatform.tvOS.ToString(), _tvosConfigs.Serialize());
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
                _savePath = FindFileLocation();
            }

            if (File.Exists(_savePath))
            {
                PList p = new PList();

                if (p.Load(_savePath))
                {
                    if (Validate(p))
                    {
                        ParseFile(p);
                        return;
                    }
                }

                //fall through to fail mode
                var backupPath = _savePath + ".corrupted-" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                Debug.LogWarning("EgoXproject: Corrupted configuration file found. Recreating.");
                Debug.LogWarning("EgoXproject: Corrupted file backed up to " + backupPath);
                AssetDatabase.MoveAsset(_savePath, backupPath);
            }
        }

        string FindFileLocation()
        {
            if (string.IsNullOrEmpty(_savePath))
            {
                _savePath = Application.dataPath;
            }

            var files = Directory.GetFiles(Application.dataPath, CONFIG_FILENAME, SearchOption.AllDirectories);

            //no settings file. use default location
            if (files == null || files.Length <= 0)
            {
                return _savePath;
            }

            if (files.Length > 1)
            {
                Debug.LogWarning("EgoXproject: Multiple Configuration files found!. Using " + files[0]);
                Debug.LogWarning("EgoXproject: Ignoring the following files. Please remove the incorrect files.");

                for (int ii = 1; ii < files.Length; ++ii)
                {
                    Debug.LogWarning(files[ii]);
                }
            }

            return files[0];
        }

        bool Validate(PList plist)
        {
            var typeValue = plist.Root.StringValue(TYPE_KEY);

            if (string.IsNullOrEmpty(typeValue) || typeValue != TYPE_VALUE)
            {
                return false;
            }

            var version = plist.Root.IntValue(VERSION_KEY);

            if (version != VERSION && !_supportedVersions.Contains(version))
            {
                return false;
            }

            return true;
        }

        void ParseFile(PList plist)
        {
            _iosConfigs = new PlatformConfiguration(plist.Root.DictionaryValue(BuildPlatform.iOS.ToString()));
            _tvosConfigs = new PlatformConfiguration(plist.Root.DictionaryValue(BuildPlatform.tvOS.ToString()));
            IsDirty = false;
        }
    }
}
