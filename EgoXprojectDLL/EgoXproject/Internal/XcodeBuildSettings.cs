// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Egomotion.EgoXproject.Internal
{
    internal class XcodeBuildSettings
    {
        const string TYPE_KEY = "Type";
        const string SETTING_KEY = "Setting";
        const string GROUP_KEY = "Group";
        const string DISPLAY_NAME_KEY = "Name";
        const string VALUE_KEY = "Value";
        const string DEFAULT_INDEX = "DefaultIndex";
        const string INHERIT_KEY = "Inherit";
        const string PATH_KEY = "Path";

        const string BUILD_SETTINGS_PLIST = "Egomotion.EgoXproject.Resources.BuildSettings.plist";
        const string FILE_TYPE_VALUE = "EgoXproject Build Settings";
        const string BUILD_SETTINGS_KEY = "BuildSettings";

        enum SettingType
        {
            Bool,
            String,
            Array,
            StringList,
            Enum
        };

        Dictionary<string, BaseBuildSetting> _settings = new Dictionary<string, BaseBuildSetting>();

        static XcodeBuildSettings _instance = null;

        public static XcodeBuildSettings Instance()
        {
            if (_instance == null)
            {
                _instance = new XcodeBuildSettings();
            }

            return _instance;
        }

        public static void Destroy()
        {
            _instance = null;
        }

        protected XcodeBuildSettings()
        {
            LoadResourceBuildSettings();
        }

        void ExtractCommon(PListDictionary dic, out string settingName, out string displayName, out string group)
        {
            settingName = dic.StringValue(SETTING_KEY);
            displayName = dic.StringValue(DISPLAY_NAME_KEY);
            group = dic.StringValue(GROUP_KEY);
        }

        void AddBool(PListDictionary dic)
        {
            string settingName, displayName, group;
            ExtractCommon(dic, out settingName, out displayName, out group);
            var value = dic.BoolValue(VALUE_KEY);
            _settings[settingName] = new BoolBuildSetting(settingName, displayName, group, value);
        }

        void AddArray(PListDictionary dic)
        {
            string settingName, displayName, group;
            ExtractCommon(dic, out settingName, out displayName, out group);
            bool inherited = dic.BoolValue(INHERIT_KEY);
            bool path = dic.BoolValue(PATH_KEY);
            _settings[settingName] = new ArrayBuildSetting(settingName, displayName, group, path, inherited);
        }

        void AddStringList(PListDictionary dic)
        {
            string settingName, displayName, group;
            ExtractCommon(dic, out settingName, out displayName, out group);
            bool path = dic.BoolValue(PATH_KEY);
            bool inherited = dic.BoolValue(INHERIT_KEY);
            _settings[settingName] = new StringListBuildSetting(settingName, displayName, group, path, inherited);
        }

        void AddString(PListDictionary dic)
        {
            string settingName, displayName, group;
            ExtractCommon(dic, out settingName, out displayName, out group);
            var value = dic.StringValue(VALUE_KEY);
            bool path = dic.BoolValue(PATH_KEY);
            _settings[settingName] = new StringBuildSetting(settingName, displayName, group, path, value);
        }

        void AddEnum(PListDictionary dic)
        {
            string settingName, displayName, group;
            ExtractCommon(dic, out settingName, out displayName, out group);
            var valueDic = dic.DictionaryValue(VALUE_KEY);

            if (valueDic != null)
            {
                List<string> enumValues = new List<string>();
                List<string> enumNames = new List<string>();
                int defaultIndex = dic.IntValue(DEFAULT_INDEX);

                foreach (var key in valueDic.Keys)
                {
                    enumValues.Add(key);
                    enumNames.Add(valueDic.StringValue(key));
                }

                _settings[settingName] = new EnumBuildSetting(settingName, displayName, group, enumValues.ToArray(), enumNames.ToArray(), defaultIndex);
            }
        }

        public ReadOnlyCollection<BaseBuildSetting> BuildSettings
        {
            get
            {
                return _settings.Values.ToList().AsReadOnly();
            }
        }

        public bool BuildSetting(string settingName, out BaseBuildSetting buildSetting)
        {
            return _settings.TryGetValue(settingName, out buildSetting);
        }

        public T BuildSetting<T>(string settingName) where T : BaseBuildSetting
        {
            BaseBuildSetting buildSetting;

            if (_settings.TryGetValue(settingName, out buildSetting))
            {
                return buildSetting as T;
            }

            return null;
        }

        public string DisplayName(string settingName)
        {
            BaseBuildSetting setting;

            if (BuildSetting(settingName, out setting))
            {
                return setting.DisplayName;
            }

            return settingName;
        }

        void LoadResourceBuildSettings()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream stream = myAssembly.GetManifestResourceStream(BUILD_SETTINGS_PLIST);

            if (stream == null)
            {
                return;
            }

            PList plist = new PList();
            string content = "";

            using (StreamReader reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }

            if (!plist.LoadFromString(content))
            {
                return;
            }

            //check is right type
            if (!ValidatePlist(plist))
            {
                return;
            }

            //load the contents from the dic
            var settings = plist.Root.ArrayValue(BUILD_SETTINGS_KEY);

            if (settings == null)
            {
                return;
            }

            //populate the settings
            for (int ii = 0; ii < settings.Count; ++ii)
            {
                var dic = settings.DictionaryValue(ii);
                SettingType type;

                try
                {
                    type = (SettingType) System.Enum.Parse(typeof(SettingType), dic.StringValue(TYPE_KEY));
                }
                catch
                {
                    Debug.LogError("EgoXproject: Unknown setting type in build settings database.");
                    continue;
                }

                switch (type)
                {
                case SettingType.Bool:
                    AddBool(dic);
                    break;

                case SettingType.Enum:
                    AddEnum(dic);
                    break;

                case SettingType.String:
                    AddString(dic);
                    break;

                case SettingType.Array:
                    AddArray(dic);
                    break;

                case SettingType.StringList:
                    AddStringList(dic);
                    break;

                default:
                    Debug.LogError("EgoXproject: Developer has forgot to implement code for a new type in the build settings database.");
                    break;
                }
            }
        }

        bool ValidatePlist(PList plist)
        {
            return plist.Root.StringValue(TYPE_KEY) == FILE_TYPE_VALUE;
        }

        public static bool ValidateSettingString(string settingName)
        {
            //null or empty is not valid
            if (string.IsNullOrEmpty(settingName))
            {
                UnityEngine.Debug.LogError("String is empty");
                return false;
            }

            //A valid entry can start with a letter or underscore, and remaining string can be letter, number or underscore
            var remainingString = settingName.Substring(1);
            //invalid patterns - ^ makes it find everhthing that is not these
            string firstCharPattern = @"^[^A-Za-z_]";
            string remainingCharPattern = @"[^A-Za-z0-9_]";

            if (Regex.IsMatch(settingName, firstCharPattern))
            {
                return false;
            }

            if (Regex.IsMatch(remainingString, remainingCharPattern))
            {
                return false;
            }

            return true;
        }
    }
}
