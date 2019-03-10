using UnityEngine;
using UnityEditor;
using System.Collections;
using Egomotion.EgoXproject;
using Egomotion.EgoXproject.Internal;
using System.IO;

public abstract class BaseSettingsEditor : EditorWindow
{
    protected static readonly string TYPE_KEY = "Type";
    protected static readonly string BOOL_TYPE_VALUE = "Bool";
    protected static readonly string STRING_TYPE_VALUE = "String";
    protected static readonly string ARRAY_TYPE_VALUE = "Array";

    protected static readonly string SETTING_KEY = "Setting";
    protected static readonly string NAME_KEY = "Name";
    protected static readonly string VALUE_KEY = "Value";

    internal PList Plist
    {
        get;
        set;
    }
    Vector2 _scrollPosition;
    string _lastPath = "uk.co.egomotion.egoXproject.BaseSettingsEditor.LastPath";
    string _openMessage = "Open Setting File";
    string _createMessage = "Create Setting File";
    string _fileName = "Settings";
    string _settingsDicKey;

    protected void Configure(string lastPathKey,
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

    void OnGUI()
    {
        if (Plist == null)
        {
            DrawLoadPlist();
        }
        else
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawEntries();
            EditorGUILayout.EndScrollView();
            DrawEditPlist();
        }
    }

    void DrawLoadPlist()
    {
        if (GUILayout.Button("Open"))
        {
            string path = PlayerPrefs.GetString(_lastPath, Application.dataPath);
            path = EditorUtility.OpenFilePanel(_openMessage, path, "plist");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            PlayerPrefs.SetString(_lastPath, Path.GetDirectoryName(path));
            Plist = new PList(path);

            if (!ValidatePlist())
            {
                EditorUtility.DisplayDialog("Error", "Invalid Plist File", "Oops");
                Plist = null;
            }
        }

        if (GUILayout.Button("Create"))
        {
            var path = EditorUtility.SaveFilePanel(_createMessage, Application.dataPath, _fileName, "plist");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            CreateNewFile(path);
        }
    }

    protected abstract void DrawEditPlist();

    protected void DrawEntries()
    {
        var settings = Plist.Root.ArrayValue(_settingsDicKey);
        int indexToRemove = -1;

        for (int ii = 0; ii < settings.Count; ++ii)
        {
            var dic = settings.DictionaryValue(ii);

            if (DrawEntry(dic))
            {
                indexToRemove = ii;
            }
        }

        if (indexToRemove > -1)
        {
            settings.RemoveAt(indexToRemove);
        }
    }

    internal abstract bool DrawEntry(PListDictionary dic);
    protected abstract void CreateNewFile(string path);
    protected abstract bool ValidatePlist();

}
