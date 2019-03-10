using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Egomotion.EgoXproject;
using Egomotion.EgoXproject.Internal;
using System.Linq;
using System.Text.RegularExpressions;
using Egomotion.EgoXproject.UI.Internal;

public class BuildSettingsEditor : EditorWindow
{
    readonly string FILE_TYPE_VALUE = "EgoXproject Build Settings";
    static readonly string BUILD_SETTINGS_KEY = "BuildSettings";
    readonly string DEFAULT_INDEX = "DefaultIndex";

    readonly string TYPE_KEY          = "Type";

    readonly string SETTING_KEY = "Setting";
    readonly string DISPLAY_NAME_KEY = "Name";
    readonly string VALUE_KEY = "Value";
    readonly string GROUP_KEY = "Group";
    readonly string INHERIT_KEY = "Inherit";
    readonly string PATH_KEY = "Path";

    readonly string[] _groups = new string[]
    {
        "-- Undefined --",
        "Architectures",
        "Assets",
        "Build Locations",
        "Build Options",
        "Deployment",
        "Headers",
        "Kernel Module",
        "Linking",
        "Packaging",
        "Search Paths",
        "Signing",
        "Testing",
        "Text-Based API",
        "Versioning",
        "Apple LLVM - Address Sanitizer",
        "Apple LLVM - Code Generation",
        "Apple LLVM - Custom Compiler Flags",
        "Apple LLVM - Language",
        "Apple LLVM - Language - C++",
        "Apple LLVM - Language - Modules",
        "Apple LLVM - Language - Objective C",
        "Apple LLVM - Preprocessing",
        "Apple LLVM - Warning Policies",
        "Apple LLVM - Warnings - All languages",
        "Apple LLVM - Warnings - C++",
        "Apple LLVM - Warnings - Objective C",
        "Apple LLVM - Warnings - Objective C and ARC",
        "Asset Catalog Compiler - Options",
        "Interface Builder Storyboard Compiler - Options",
        "Metal Compiler - Build Options",
        "Metal Compiler - Features",
        "OSACompile - Build Options",
        "Static Analyzer - Analysis Policy",
        "Static Analyzer - Generic Issues",
        "Static Analyzer - Issues - Objective-C",
        "Static Analyzer - Issues - Security",
    };

    //cached vales
    string _setting;
    string _name;
    bool _boolDefaultValue;
    string _stringDefaultValue;
    Dictionary<string, string> _enumValues = new Dictionary<string, string>();
    string _defaultEnum;
    int _groupIndex = 0;
    bool _isValidSettingName = false;
    bool _isInherit = false;
    bool _isPath = false;

    PList _plist;

    Vector2 _scrollPosition;
    string _lastPath = "uk.co.egomotion.egoXproject.BaseSettingsEditor.LastPath";
    string _openMessage = "Open Setting File";
    string _createMessage = "Create Setting File";
    string _fileName = "Settings";
    string _settingsDicKey;

    Dictionary<string, bool> _foldoutState = new Dictionary<string, bool>();

    enum SettingType {Bool, String, Array, StringList, Enum};
    SettingType _entryType = SettingType.Bool;

    enum EntryAction { None, MoveUp, MoveDown, Edit, Remove};

    Styling _style;

    [MenuItem("Window/EgoXproject Dev/Edit Build Settings Entries")]
    static void ShowWindow()
    {
        var win = GetWindow<BuildSettingsEditor>();
        win.Show();
        win.Configure("uk.co.egomotion.egoXproject.BuildSettingsEditor.LastPath",
                      "Open Build Settings PList",
                      "Create Build Settings PList",
                      "BuildSettings",
                      BUILD_SETTINGS_KEY);
    }


    void Configure(string lastPathKey,
                   string openMessage,
                   string createMessage,
                   string defaultFileNameNoExt,
                   string settingsDicKey)
    {
        if (!string.IsNullOrEmpty(lastPathKey))
        {
            _lastPath = lastPathKey;
        }

        if (!string.IsNullOrEmpty(openMessage))
        {
            _openMessage = openMessage;
        }

        if (!string.IsNullOrEmpty(createMessage))
        {
            _createMessage = createMessage;
        }

        if (!string.IsNullOrEmpty(defaultFileNameNoExt))
        {
            _fileName = defaultFileNameNoExt;
        }

        if (!string.IsNullOrEmpty(settingsDicKey))
        {
            _settingsDicKey = settingsDicKey;
        }
    }

    void OnEnable()
    {
        if (_style == null)
        {
            _style = new Styling();
        }

        _style.Load();
    }

    void OnDisable()
    {
        if (_style != null)
        {
            _style.Unload();
        }
    }

    void OnGUI()
    {
        if (_plist == null)
        {
            DrawLoadPlist();
        }
        else
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawEntries();
            EditorGUILayout.EndScrollView();
            var color = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            DrawEditPlist();
            GUI.backgroundColor = color;
        }
    }

    void DrawLoadPlist()
    {
        if (GUILayout.Button(_openMessage))
        {
            string path = PlayerPrefs.GetString(_lastPath, Application.dataPath);
            path = EditorUtility.OpenFilePanel(_openMessage, path, "plist");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            PlayerPrefs.SetString(_lastPath, Path.GetDirectoryName(path));
            _plist = new PList(path);

            if (!ValidatePlist())
            {
                EditorUtility.DisplayDialog("Error", "Invalid Plist File", "Oops");
                _plist = null;
            }

            CheckGroups();
        }

        if (GUILayout.Button(_createMessage))
        {
            var path = EditorUtility.SaveFilePanel(_createMessage, Application.dataPath, _fileName, "plist");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            CreateNewFile(path);
        }
    }

    void DrawEntries()
    {
        //populate default fold out state data if requried
        if (_foldoutState.Count != _groups.Length)
        {
            foreach (var group in _groups)
            {
                _foldoutState[group] = true;
            }
        }

        var settings = _plist.Root.ArrayValue(_settingsDicKey);

        if (settings == null)
        {
            EditorGUILayout.LabelField("Error loading settings");
            return;
        }

        int actionIndex = -1;
        EntryAction action = EntryAction.None;
        Color normalColor = GUI.color;
        Color altColor = Color.green;
        bool useAltColor = false;

        //draw the foldouts
        foreach (var group in _groups)
        {
            GUI.color = normalColor;
            useAltColor = false;
            _foldoutState[group] = EditorGUILayout.Foldout(_foldoutState[group], group, _style.Foldout());

            if (!_foldoutState[group])
            {
                continue;
            }

            EditorGUI.indentLevel++;

            for (int ii = 0; ii < settings.Count; ++ii)
            {
                var dic = settings.DictionaryValue(ii);

                if (dic.StringValue(GROUP_KEY) != group)
                {
                    continue;
                }

                var a = DrawEntry(dic, ii);

                if (a != EntryAction.None)
                {
                    action = a;
                    actionIndex = ii;
                }

                useAltColor = !useAltColor;
                GUI.color = useAltColor ? altColor : normalColor;
            }

            EditorGUI.indentLevel--;
        }

        GUI.color = normalColor;

        switch (action)
        {
        case EntryAction.Edit:
            PopulateEdit(settings.DictionaryValue(actionIndex));
            break;

        case EntryAction.MoveUp:
            if (actionIndex > 0)
            {
                var entry = settings[actionIndex];
                settings.RemoveAt(actionIndex);
                int newIndex = actionIndex - 1;
                settings.Insert(newIndex, entry);
                _plist.Save();
            }

            break;

        case EntryAction.MoveDown:
            if (actionIndex < settings.Count - 1)
            {
                var entry = settings[actionIndex];
                settings.RemoveAt(actionIndex);
                int newIndex = actionIndex + 1;
                settings.Insert(newIndex, entry);
                _plist.Save();
            }

            break;

        case EntryAction.Remove:
            settings.RemoveAt(actionIndex);
            _plist.Save();
            break;

        default:
            break;
        }
    }

    EntryAction DrawEntry(PListDictionary dic, int index)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(dic.StringValue(DISPLAY_NAME_KEY), GUILayout.MaxWidth(400));
        EditorGUILayout.LabelField(dic.StringValue(SETTING_KEY), GUILayout.MaxWidth(400));
        var type = (SettingType)System.Enum.Parse(typeof(SettingType), dic.StringValue(TYPE_KEY));
        EditorGUILayout.LabelField(type.ToString(), GUILayout.MaxWidth(200));

        switch (type)
        {
        case SettingType.Bool:
            DrawBoolEntry(dic);
            break;

        case SettingType.String:
            DrawStringEntry(dic);
            break;

        case SettingType.Enum:
            DrawEnumEntry(dic);
            break;

        case SettingType.Array:
        case SettingType.StringList:
            DrawArrayEntry(dic);
            break;

        default:
            EditorGUILayout.LabelField("UNKNOWN", GUILayout.MaxWidth(600));
            break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(index.ToString(), GUILayout.MaxWidth(40));
        EntryAction action = EntryAction.None;

        if (GUILayout.Button("^", GUILayout.MaxWidth(40)))
        {
            action = EntryAction.MoveUp;
        }

        if (GUILayout.Button("v", GUILayout.MaxWidth(40)))
        {
            action = EntryAction.MoveDown;
        }

        if (GUILayout.Button("e", GUILayout.MaxWidth(40)))
        {
            action = EntryAction.Edit;
        }

        if (GUILayout.Button("-", GUILayout.MaxWidth(40)))
        {
            if (EditorUtility.DisplayDialog("Remove Entry?", "Remove entry: " + dic.StringValue(DISPLAY_NAME_KEY) + " - " + dic.StringValue(SETTING_KEY), "Remove", "Cancel"))
            {
                action = EntryAction.Remove;
            }
        }

        EditorGUILayout.EndHorizontal();
        return action;
    }

    void DrawBoolEntry(PListDictionary dic)
    {
        EditorGUILayout.LabelField(dic.BoolValue(VALUE_KEY) ? "Yes" : "No", GUILayout.MaxWidth(600));
    }

    void DrawStringEntry(PListDictionary dic)
    {
        string val = dic.StringValue(VALUE_KEY);

        if (dic.BoolValue(PATH_KEY))
        {
            if (!string.IsNullOrEmpty(val))
            {
                val += " : ";
            }

            val += "Path";
        }

        EditorGUILayout.LabelField(val, GUILayout.MaxWidth(600));
    }

    void DrawArrayEntry(PListDictionary dic)
    {
        var array = dic.ArrayValue(VALUE_KEY);

        if (array != null)
        {
            EditorGUILayout.Popup(0, array.ToStringArray(), GUILayout.MaxWidth(600));
        }

        string val = "";

        if (dic.BoolValue(INHERIT_KEY))
        {
            val += "Inherit";
        }

        if (dic.BoolValue(PATH_KEY))
        {
            if (!string.IsNullOrEmpty(val))
            {
                val += " : ";
            }

            val += "Path";
        }

        EditorGUILayout.LabelField(val, GUILayout.MaxWidth(600));
    }

    void DrawEnumEntry(PListDictionary dic)
    {
        var enumDic = dic.DictionaryValue(VALUE_KEY);

        if (enumDic == null)
        {
            EditorGUILayout.LabelField("NO VALUES DEFINED", GUILayout.MaxWidth(600));
        }
        else
        {
            List<string> entries = new List<string>();

            foreach (var kvp in enumDic)
            {
                entries.Add(kvp.Key + " | " + kvp.Value);
            }

            int selectedIndex = dic.IntValue(DEFAULT_INDEX);
            EditorGUILayout.Popup(selectedIndex, entries.ToArray(), GUILayout.MaxWidth(600));
        }
    }


    void PopulateEdit(PListDictionary dic)
    {
        ResetEntry();
        _name = dic.StringValue(DISPLAY_NAME_KEY);
        _setting =  dic.StringValue(SETTING_KEY);
        _isValidSettingName = ValidateSettingString(_setting);
        _groupIndex = System.Array.IndexOf(_groups, dic.StringValue(GROUP_KEY));

        if (_groupIndex < 0)
        {
            _groupIndex = 0;
        }

        _entryType = (SettingType)System.Enum.Parse(typeof(SettingType), dic.StringValue(TYPE_KEY));

        switch (_entryType)
        {
        case SettingType.Bool:
            _boolDefaultValue = dic.BoolValue(VALUE_KEY);
            break;

        case SettingType.Enum:
            var valDic = dic.DictionaryValue(VALUE_KEY);

            if (valDic != null)
            {
                foreach (var key in valDic.Keys)
                {
                    _enumValues[key] = valDic.StringValue(key);
                }
            }

            var defaultIndex = dic.IntValue(DEFAULT_INDEX);
            _defaultEnum = _enumValues.Keys.ToList()[defaultIndex];
            break;

        case SettingType.String:
            _stringDefaultValue = dic.StringValue(VALUE_KEY);
            _isPath = dic.BoolValue(PATH_KEY);
            break;

        case SettingType.Array:
        case SettingType.StringList:
            _isInherit = dic.BoolValue(INHERIT_KEY);
            _isPath = dic.BoolValue(PATH_KEY);
            break;

        default:
            break;
        }
    }

    void DrawEditPlist()
    {
        _groupIndex = EditorGUILayout.Popup("Group", _groupIndex, _groups);
        _name = EditorGUILayout.TextField("Name", _name);
        EditorGUI.BeginChangeCheck();
        _setting = EditorGUILayout.TextField("Setting", _setting);

        if (EditorGUI.EndChangeCheck())
        {
            _isValidSettingName = ValidateSettingString(_setting);
        }

        _entryType = (SettingType)EditorGUILayout.EnumPopup("Type", _entryType);

        switch (_entryType)
        {
        case SettingType.Bool:
            DrawBoolEdit();
            break;

        case SettingType.Enum:
            DrawEnumEdit();
            break;

        case SettingType.String:
            DrawStringEdit();
            break;

        case SettingType.Array:
        case SettingType.StringList:
            DrawArrayEdit();
            break;

        default:
            throw new System.Exception("Invalid Entry Type");
        }

        EditorGUILayout.BeginHorizontal();
        GUI.enabled = CanAddEntry();

        if (GUILayout.Button((SettingExists(_setting) ? "*** REPLACE ***" : "+++ ADD +++")))
        {
            AddEntry();
        }

        GUI.enabled = true;

        if (GUILayout.Button("Reset"))
        {
            ResetEntry();
        }

        if (GUILayout.Button("Save"))
        {
            _plist.Save();
        }

        EditorGUILayout.EndHorizontal();
    }

    void DrawBoolEdit()
    {
        _boolDefaultValue = EditorGUILayout.Toggle("Default Value", _boolDefaultValue);
    }

    void DrawEnumEdit()
    {
        EditorGUI.indentLevel++;
        string oldKey = "";
        string newKey = "";
        bool keyChanged = false;
        string valueKey = "";
        string value = "";
        bool valueChanged = false;

        bool remove = false;

        string removeKey = "";
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Setting Name");
        EditorGUILayout.LabelField("Display Name");
        EditorGUILayout.EndHorizontal();

        foreach (string key in _enumValues.Keys)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string tempKey = EditorGUILayout.TextField(key);

            if (EditorGUI.EndChangeCheck())
            {
                keyChanged = true;
                oldKey = key;
                newKey = tempKey;
            }

            EditorGUI.BeginChangeCheck();
            var tempVal = EditorGUILayout.TextField(_enumValues[key]);

            if (EditorGUI.EndChangeCheck())
            {
                valueChanged = true;
                value = tempVal;
                valueKey = key;
            }

            EditorGUI.BeginChangeCheck();
            bool defaultEntry = EditorGUILayout.Toggle(_defaultEnum == key);

            if (EditorGUI.EndChangeCheck() && defaultEntry)
            {
                _defaultEnum = key;
            }

            if (GUILayout.Button("-"))
            {
                remove = true;

                removeKey = key;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (keyChanged)
        {
            if (!_enumValues.ContainsKey(newKey))
            {
                var v = _enumValues[oldKey];
                _enumValues.Remove(oldKey);
                _enumValues.Add(newKey, v);
            }
        }

        if (valueChanged)
        {
            _enumValues[valueKey] = value;
        }

        if (remove)
        {
            _enumValues.Remove(removeKey);

            if (_defaultEnum == removeKey)
            {
                _defaultEnum = "";
            }
        }

        if (GUILayout.Button("+ Value"))
        {
            _enumValues[""] = "";
        }

        EditorGUI.indentLevel--;
    }

    void DrawStringEdit()
    {
        _stringDefaultValue = EditorGUILayout.TextField(_stringDefaultValue);
        _isPath = EditorGUILayout.Toggle("Is Path", _isPath);
    }

    void DrawArrayEdit()
    {
        _isInherit = EditorGUILayout.Toggle("Add Inherited", _isInherit);
        _isPath = EditorGUILayout.Toggle("Is Path", _isPath);
    }

    bool CanAddEntry()
    {
        if (string.IsNullOrEmpty(_name) ||
                string.IsNullOrEmpty(_setting) ||
                _groupIndex == 0 ||
                !_isValidSettingName)
        {
            return false;
        }

        return true;
    }

    void AddEntry()
    {
        if (!CanAddEntry())
        {
            return;
        }

        var settings = _plist.Root.ArrayValue(BUILD_SETTINGS_KEY);
        int existingSettingIndex = IndexOfSetting(_setting);
        var dic = new PListDictionary();
        dic.Add(SETTING_KEY, _setting);
        dic.Add(DISPLAY_NAME_KEY, _name);
        dic.Add(GROUP_KEY, _groups[_groupIndex]);
        dic.Add(TYPE_KEY, _entryType.ToString());

        switch (_entryType)
        {
        case SettingType.Bool:
            AddBoolValue(dic);
            break;

        case SettingType.Enum:
            AddEnumValues(dic);
            break;

        case SettingType.String:
            AddStringValue(dic);
            break;

        case SettingType.Array:
        case SettingType.StringList:
            AddArrayValue(dic);
            break;

        default:
            throw new System.Exception("Invalid Entry Type");
        }

        if (existingSettingIndex < 0)
        {
            settings.Add(dic);
        }
        else
        {
            settings.RemoveAt(existingSettingIndex);
            settings.Insert(existingSettingIndex, dic);
        }

        _plist.Save();
        ResetEntry();
    }

    void ResetEntry()
    {
        _name = "";
        _setting = "";
        _boolDefaultValue = false;
        _enumValues.Clear();
        _defaultEnum = "";
        _stringDefaultValue = "";
        _isValidSettingName = false;
        _isInherit = false;
        _isPath = false;
    }

    void CreateNewFile(string path)
    {
        _plist = new PList();
        _plist.Root.Add(TYPE_KEY, FILE_TYPE_VALUE);
        _plist.Root[BUILD_SETTINGS_KEY] = new PListArray();
        _plist.Save(path, true);
    }

    bool ValidatePlist()
    {
        return _plist.Root.StringValue(TYPE_KEY) == FILE_TYPE_VALUE;
    }

    void CheckGroups()
    {
        var settings = _plist.Root.ArrayValue(_settingsDicKey);

        if (settings == null)
        {
            EditorGUILayout.LabelField("Error loading settings");
            return;
        }

        for (int ii = 0; ii < settings.Count; ++ii)
        {
            var dic = settings.DictionaryValue(ii);
            var group = dic.StringValue(GROUP_KEY);

            if (!_groups.Contains(group))
            {
                Debug.LogError("Invalid Group found in setting: " + dic.StringValue(SETTING_KEY) + " has group " + group);
                //TODO reassign to the unassigend group?
            }
        }
    }

    void AddEnumValues(PListDictionary dic)
    {
        var valDic = new PListDictionary();

        foreach (var kvp in _enumValues)
        {
            if (string.IsNullOrEmpty(kvp.Key) || string.IsNullOrEmpty(kvp.Value))
            {
                Debug.LogWarning("Warning adding empty key or value for enum");
                //                continue;
            }

            valDic.Add(kvp.Key, kvp.Value);
        }

        if (valDic.Count > 0)
        {
            dic.Add(VALUE_KEY, valDic);
        }

        int defaultIndex = _enumValues.Keys.ToList().IndexOf(_defaultEnum);

        if (defaultIndex >= 0)
        {
            dic.Add(DEFAULT_INDEX, defaultIndex);
        }
    }

    void AddBoolValue(PListDictionary dic)
    {
        dic.Add(VALUE_KEY, _boolDefaultValue);
    }

    void AddStringValue(PListDictionary dic)
    {
        if (!string.IsNullOrEmpty(_stringDefaultValue))
        {
            dic.Add(VALUE_KEY, _stringDefaultValue);
        }

        if (_isPath)
        {
            dic.Add(PATH_KEY, _isPath);
        }
    }

    void AddArrayValue(PListDictionary dic)
    {
        dic.Add(INHERIT_KEY, _isInherit);

        if (_isPath)
        {
            dic.Add(PATH_KEY, _isPath);
        }
    }

    int IndexOfSetting(string settingName)
    {
        var settings = _plist.Root.ArrayValue(BUILD_SETTINGS_KEY);

        for (int ii = 0; ii < settings.Count; ++ii)
        {
            var dic = settings[ii] as PListDictionary;

            if (dic == null)
            {
                continue;
            }

            if (dic.StringValue(SETTING_KEY) == settingName)
            {
                return ii;
            }
        }

        return -1;
    }

    bool SettingExists(string settingName)
    {
        return IndexOfSetting(settingName) > -1;
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
