using UnityEngine;
using UnityEditor;
using System.Collections;
using Egomotion.EgoXproject;
using Egomotion.EgoXproject.Internal;
using System.IO;

public class PlistSettingsEditor : BaseSettingsEditor
{
    static readonly string FILE_TYPE_VALUE = "EgoXproject Build Settings";
    static readonly string INFO_PLIST_SETTINGS_KEY = "InfoPlistSettings";

    protected static readonly string DICTIONARY_TYPE_VALUE = "Bool";
    protected static readonly string REAL_TYPE_VALUE = "Real";
    protected static readonly string INTEGER_TYPE_VALUE = "Int";
    protected static readonly string DATE_TYPE_VALUE = "Date";
    protected static readonly string DATA_TYPE_VALUE = "Data";

    [MenuItem("Window/EgoXproject Dev/Add Info.plist Setting Entry")]
    static void ShowWindow()
    {
        var win = GetWindow<PlistSettingsEditor>();
        win.Show();
        win.Configure("uk.co.egomotion.egoXproject.PListSettingsEditor.LastPath",
                      "Open Info.pList Settings PList",
                      "Create Info.plist Setting PList",
                      "InfoPlistSettings",
                      INFO_PLIST_SETTINGS_KEY);
    }

    #region implemented abstract members of BaseSettingsEditor

    protected override void DrawEditPlist ()
    {
    }

    internal override bool DrawEntry (Egomotion.EgoXproject.Internal.PListDictionary dic)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(dic.StringValue(SETTING_KEY));
        EditorGUILayout.LabelField(dic.StringValue(NAME_KEY));
        var type = dic.StringValue(TYPE_KEY);

        if (type == BOOL_TYPE_VALUE)
        {
            var value = dic.BoolValue(VALUE_KEY);
            EditorGUILayout.LabelField(value ? "Yes" : "No");
        }
        else if (type == STRING_TYPE_VALUE)
        {
            var value = dic.StringValue(VALUE_KEY);
            EditorGUILayout.LabelField(value);
        }
        else if (type == INTEGER_TYPE_VALUE)
        {
            var value = dic.IntValue(VALUE_KEY);
            EditorGUILayout.LabelField(value.ToString());
        }
        else if (type == REAL_TYPE_VALUE)
        {
            var value = dic.FloatValue(VALUE_KEY);
            EditorGUILayout.LabelField(value.ToString());
        }
        else if (type == ARRAY_TYPE_VALUE)
        {
            //TODO should also be able to draw a defaul array entries
            EditorGUILayout.LabelField("Array");
        }
        else if (type == DICTIONARY_TYPE_VALUE)
        {
            //TODO should also be able to draw a defaul array entries
            EditorGUILayout.LabelField("Dictionary");
        }
        else
        {
            EditorGUILayout.LabelField("UNKNOWN");
        }

        bool remove = false;;

        if (GUILayout.Button("-"))
        {
            remove = true;
        }

        EditorGUILayout.EndHorizontal();

        return remove;
    }

    protected override void CreateNewFile (string path)
    {
        Plist = new PList();
        Plist.Root.Add(TYPE_KEY, FILE_TYPE_VALUE);
        Plist.Root[INFO_PLIST_SETTINGS_KEY] = new PListArray();
        Plist.Save(path, true);
    }

    protected override bool ValidatePlist ()
    {
        return Plist.Root.StringValue(TYPE_KEY) == FILE_TYPE_VALUE;
    }

    #endregion
}
