//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Egomotion.EgoXproject.Internal
{
    internal class XCBuildConfiguration : PBXBaseObject
    {
        const string BUILD_SETTINGS_KEY = "buildSettings";
        const string NAME_KEY = "name";

        const string INHERITED = "$(inherited)";

        const string FRAMEWORK_SEARCH_PATHS = "FRAMEWORK_SEARCH_PATHS";
        const string LIBRARY_SEARCH_PATHS = "LIBRARY_SEARCH_PATHS";
        const string LD_RUNPATH_SEARCH_PATHS = "LD_RUNPATH_SEARCH_PATHS";

        const string ANY_SDK_CONDITIONAL = "[sdk=*]";
        const string IOS_SDK_CONDITIONAL = "[sdk=iphoneos*]";
        const string IOS_SIMULATOR_SDK_CONDITIONAL = "[sdk=iphonesimulator*]";
        const string TVOS_SDK_CONDITIONAL = "[sdk=appletvos*]";
        const string TVOS_SIMULATOR_SDK_CONDITIONAL = "[sdk=appletvsimulator*]";
        const string WATCHOS_SDK_CONDITIONAL = "[sdk=watchos*]";
        const string WATCHOS_SIMULATOR_SDK_CONDITIONAL = "[sdk=watchsimulator*]";
        const string MACOS_SDK_CONDITIONAL = "[sdk=macosx*]";

        const string IOS_CONDITIONAL = "iphoneos";
        const string IOS_SIMULATOR_CONDITIONAL = "iphonesimulator";
        const string TVOS_CONDITIONAL = "appletvos";
        const string TVOS_SIMULATOR_CONDITIONAL = "appletvsimulator";
        const string WATCHOS_CONDITIONAL = "watchos";
        const string WATCHOS_SIMULATOR_CONDITIONAL = "watchsimulator";
        const string MACOS_CONDITIONAL = "macosx";


        class Conditional
        {
            public enum ConditionalType
            {
                sdk,
                arch
            };

            public ConditionalType type;
            public string value;
        }

        class ConditionalSetting
        {
            public string name;
            public List<Conditional> conditionals;
        }

        Dictionary<string, List<ConditionalSetting>> _conditionalSettings;

        public XCBuildConfiguration(string uid, PBXProjDictionary dict)
        : base(PBXTypes.XCBuildConfiguration, uid, dict)
        {
            ExtractConditionals(dict);
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
        }

        #endregion

        void ExtractConditionals(PBXProjDictionary dict)
        {
            _conditionalSettings = new Dictionary<string, List<ConditionalSetting>>();

            //find and log conditionals
            //conditional will generally look like:
            // "SETTING_NAME[sdk=iphoneos*]"
            // but can look like
            // "SETTING_NAME[sdk=iphoneos*][arch=armv7]
            // * may also be a version number
            foreach (var kvp in dict.DictionaryValue(BUILD_SETTINGS_KEY))
            {
                var setting = kvp.Key;

                if (setting.Contains("["))
                {
                    var originalSetting = setting;
                    setting = setting.FromLiteral();
                    var leftBracketIndex = setting.IndexOf("[", System.StringComparison.InvariantCultureIgnoreCase);
                    var bareSetting = setting.Substring(0, leftBracketIndex);

                    if (!_conditionalSettings.ContainsKey(bareSetting))
                    {
                        _conditionalSettings[bareSetting] = new List<ConditionalSetting>();
                    }

                    int startIndex = leftBracketIndex;
                    int endIndex = -1;
                    ConditionalSetting cs = new ConditionalSetting();
                    cs.name = originalSetting;
                    cs.conditionals = new List<Conditional>();
                    Conditional conditional = null;

                    do
                    {
                        conditional = ExtractConditional(setting, startIndex, out endIndex);

                        if (conditional == null)
                        {
                            break;
                        }
                        else
                        {
                            cs.conditionals.Add(conditional);
                            startIndex = endIndex;
                        }
                    }
                    while (conditional != null);

                    _conditionalSettings[bareSetting].Add(cs);
                }
            }
        }

        Conditional ExtractConditional(string setting, int startIndex, out int endIndex)
        {
            endIndex = -1;

            if (startIndex < 0 || startIndex >= setting.Length)
            {
                return null;
            }

            var leftBracketIndex = setting.IndexOf("[", startIndex, System.StringComparison.InvariantCultureIgnoreCase);
            var rightBracketIndex = setting.IndexOf("]", leftBracketIndex, System.StringComparison.InvariantCultureIgnoreCase);
            var equalSignIndex = setting.IndexOf("=", leftBracketIndex, System.StringComparison.InvariantCultureIgnoreCase);

            if (leftBracketIndex < 0 || rightBracketIndex < 0 || equalSignIndex < 0)
            {
                return null;
            }

            if (equalSignIndex > rightBracketIndex)
            {
                Debug.LogError("EgoXproject: Malformed build setting entry: " + setting);
                return null;
            }

            var typeIndex = leftBracketIndex + 1;
            var typeLength = equalSignIndex - typeIndex;
            var typeString = setting.Substring(typeIndex, typeLength);
            Conditional.ConditionalType conditionalType;

            try
            {
                conditionalType = (Conditional.ConditionalType) System.Enum.Parse(typeof(Conditional.ConditionalType), typeString);
            }
            catch (System.Exception)
            {
                Debug.LogError("Unsupported conditional type on build setting: " + setting);
                return null;
            }

            var valueIndex = equalSignIndex + 1;
            var valueLength = rightBracketIndex - valueIndex;
            var value = setting.Substring(valueIndex, valueLength);
            var conditional = new Conditional();
            conditional.type = conditionalType;
            conditional.value = value;
            endIndex = rightBracketIndex + 1;
            return conditional;
        }

        public string Name
        {
            get
            {
                return Dict.StringValue(NAME_KEY);
            }
        }

        public void AddLibrarySearchPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            AddCollectionBuildSetting(LIBRARY_SEARCH_PATHS, new string[] { path }, MergeMethod.Append);
        }

        public void AddFrameworkSearchPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            AddCollectionBuildSetting(FRAMEWORK_SEARCH_PATHS, new string[] { path }, MergeMethod.Append);
        }

        public void AddRunpathSearchPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            AddCollectionBuildSetting(LD_RUNPATH_SEARCH_PATHS, new string[] { path }, MergeMethod.Append);
        }

        PBXProjDictionary BuildSettings
        {
            get
            {
                return Dict.DictionaryValue(BUILD_SETTINGS_KEY);
            }
        }

        public void AddStringBuildSetting(string settingName, string value)
        {
            if (string.IsNullOrEmpty(settingName) || string.IsNullOrEmpty(value))
            {
                return;
            }

            BaseBuildSetting baseSetting = null;
            XcodeBuildSettings.Instance().BuildSetting(settingName, out baseSetting);

            //is custom or is a string setting
            if (baseSetting == null || baseSetting is StringBuildSetting)
            {
                string settingNameToUse = SettingNameToUse(settingName);
                BuildSettings[settingNameToUse] = new PBXProjString(value.ToLiteralIfRequired());
            }
            else
            {
                Debug.LogError("EgoXproject: " + settingName + " is not a string build setting");
            }
        }

        public void AddCustomStringBuildSetting (string settingName, string value)
        {
            if (string.IsNullOrEmpty (settingName) || string.IsNullOrEmpty (value))
            {
                return;
            }

            BuildSettings [settingName] = new PBXProjString (value.ToLiteralIfRequired ());
        }


        public void AddEnumBuildSetting(string settingName, string value)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                return;
            }

            BaseBuildSetting baseSetting = null;
            XcodeBuildSettings.Instance().BuildSetting(settingName, out baseSetting);

            //we don't know about it (custom) add as string, or it is a known enum, or it is known but not an enum
            if (baseSetting == null)
            {
                AddCustomStringBuildSetting(settingName, value);
            }
            else if (baseSetting is EnumBuildSetting)
            {
                var enumSetting = baseSetting as EnumBuildSetting;

                if (!enumSetting.IsValidValue(value))
                {
                    string values = enumSetting.EnumValues[0];

                    for (int ii = 1; ii < enumSetting.EnumValues.Length; ii++)
                    {
                        values += ", " + enumSetting.EnumValues[ii];
                    }

                    Debug.LogError("EgoXproject: " + settingName + " value " + value + " is not a valid option. It can only be one of the following: " + values);
                    return;
                }

                //TODO check for enum conditional
                BuildSettings[settingName] = new PBXProjString(value.ToLiteralIfRequired());
            }
            else
            {
                Debug.LogError("EgoXproject: " + settingName + " is not an enumerated string build setting");
            }
        }

        public void AddBoolBuildSetting(string settingName, bool value)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                return;
            }

            BaseBuildSetting baseSetting = null;
            XcodeBuildSettings.Instance().BuildSetting(settingName, out baseSetting);

            //we don't know about it (custom) or it is not a bool
            if (baseSetting == null || baseSetting is BoolBuildSetting)
            {
                //TODO check for bool conditional
                BuildSettings[settingName] = new PBXProjBoolean(value);
            }
            else
            {
                Debug.LogError("EgoXproject: " + settingName + " is not a boolean build setting");
            }
        }

        public void AddCollectionBuildSetting(string settingName, string[] values, MergeMethod mergeMethod)
        {
            if (string.IsNullOrEmpty(settingName) || values == null || values.Length <= 0)
            {
                return;
            }

            BaseBuildSetting baseSetting = null;
            XcodeBuildSettings.Instance().BuildSetting(settingName, out baseSetting);

            //its a custom array setting
            if (baseSetting == null)
            {
                baseSetting = new StringListBuildSetting(settingName, settingName, "User-Defined", false, false);
            }

            if (baseSetting is ArrayBuildSetting)
            {
                AddArrayBuildSetting(baseSetting as ArrayBuildSetting, values, mergeMethod);
            }
            else if (baseSetting is StringListBuildSetting)
            {
                AddStringListBuildSetting(baseSetting as StringListBuildSetting, values, mergeMethod);
            }
            else
            {
                Debug.LogError("EgoXproject: " + settingName + " is not a string list build setting");
            }
        }

        void AddArrayBuildSetting(ArrayBuildSetting arraySetting, string[] values, MergeMethod mergeMethod)
        {
            var settings = BuildSettings;
            PBXProjArray array = null;

            //TODO check conditionals, as will have to run for each conditional variant

            //handle appending
            if (mergeMethod == MergeMethod.Append)
            {
                array = settings.Element<PBXProjArray>(arraySetting.BuildSettingName);

                //single entry may have been parsed as a string
                if (array == null)
                {
                    var str = settings.Element<PBXProjString>(arraySetting.BuildSettingName);

                    if (str != null)
                    {
                        array = new PBXProjArray();
                        array.Add(str.Value);
                        settings[arraySetting.BuildSettingName] = array;
                    }
                }
            }

            //handles null case and replace case
            if (array == null)
            {
                array = new PBXProjArray();
                settings[arraySetting.BuildSettingName] = array;
            }

            // handle settings that need inherit string being added
            if (arraySetting.AddInherited)
            {
                string inherited = INHERITED.EncloseInQuotes();

                if (array.Where(o => (o as PBXProjString).Value.Equals(inherited)).Count() <= 0)
                {
                    array.Add(inherited);
                }
            }

            foreach (var v in values)
            {
                string quoted;

                if (v.Contains(" ") && !v.StartsWith("\"", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    quoted = "\"" + v + "\"";
                }
                else
                {
                    quoted = v;
                }

                quoted = quoted.ToLiteralIfRequired();

                //only add if not present
                if (array.Where(o => (o as PBXProjString).Value.Equals(quoted)).Count() <= 0)
                {
                    array.Add(quoted);
                }
            }
        }

        void AddStringListBuildSetting(StringListBuildSetting stringListSetting, string[] values, MergeMethod mergeMethod)
        {
            var settings = BuildSettings;
            PBXProjString stringList = null;

            //TODO check conditionals, as will have to run for each conditional variant

            //handle appending
            if (mergeMethod == MergeMethod.Append)
            {
                stringList = settings.Element<PBXProjString>(stringListSetting.BuildSettingName);
            }

            //handles null case and replace case
            if (stringList == null)
            {
                stringList = new PBXProjString();
                settings[stringListSetting.BuildSettingName] = stringList;
            }

            //break down string list into list so it can be checked
            List<string> existingValues = StringUtils.StringListToList(stringList.Value);

            // handle settings that need inherit string being added
            if (stringListSetting.AddInherited)
            {
                if (existingValues.Where(o => o.Equals(INHERITED)).Count() <= 0)
                {
                    existingValues.Insert(0, INHERITED);
                }
            }

            foreach (var v in values)
            {
                if (existingValues.Where(o => o.Equals(v)).Count() <= 0)
                {
                    existingValues.Add(v);
                }
            }

            stringList.Value = StringUtils.ListToStringList(existingValues);
        }

        string SettingNameToUse(string settingName)
        {
            //just use the setting name if it exists
            if (BuildSettings.ContainsKey(settingName))
            {
                return settingName;
            }
            else if (_conditionalSettings.ContainsKey(settingName))
            {
                //if conditional exists, use the iphone conditional setting.
                //TODO use the conditional more intelligently
                return "\"" + settingName + IOS_SDK_CONDITIONAL + "\"";
            }
            else if (Parent != null && Parent.HasConditional(settingName))
            {
                //TODO do something better if parent has conditional
                return "\"" + settingName + IOS_SDK_CONDITIONAL + "\"";
            }
            else
            {
                return settingName;
            }
        }

        bool HasConditional(string settingName)
        {
            return _conditionalSettings.ContainsKey(settingName);
        }

        public string StringForKey(string key)
        {
            //INFOPLIST_FILE
            var str = BuildSettings.Element<PBXProjString>(key);

            if (str != null)
            {
                return str.Value;
            }

            return "";
        }

        public XCBuildConfiguration Parent
        {
            get;
            set;
        }
    }
}
