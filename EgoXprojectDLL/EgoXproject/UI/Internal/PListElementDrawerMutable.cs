//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using Egomotion.EgoXproject.UI.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal class PListElementDrawerMutable : BasePListElementDrawer
    {
        public PListElementDrawerMutable(Styling style)
        : base(style)
        {
        }

        public bool IsDirty
        {
            get;
            set;
        }

        IPListElement TypeSelector(IPListElement element)
        {
            ElementType type = ElementType.String;

            if (element is PListDictionary)
            {
                type = ElementType.Dictionary;
            }
            else if (element is PListArray)
            {
                type = ElementType.Array;
            }
            else if (element is PListBoolean)
            {
                type = ElementType.Bool;
            }
            else if (element is PListInteger)
            {
                type = ElementType.Int;
            }
            else if (element is PListReal)
            {
                type = ElementType.Real;
            }
            else if (element is PListString)
            {
                type = ElementType.String;
            }
            else if (element is PListDate)
            {
                type = ElementType.Date;
            }
            else if (element is PListData)
            {
                type = ElementType.Data;
            }

            EditorGUI.BeginChangeCheck();
            type = (ElementType) EditorGUILayout.EnumPopup(type, GUILayout.MinWidth(TYPE_WIDTH), GUILayout.MaxWidth(TYPE_WIDTH));

            if (EditorGUI.EndChangeCheck())
            {
                switch (type)
                {
                case ElementType.Array:
                    return new PListArray ();

                case ElementType.Dictionary:
                    return new PListDictionary ();

                case ElementType.Bool:
                    return new PListBoolean ();

                case ElementType.Data:
                    return new PListData ();

                case ElementType.Date:
                    return new PListDate ();

                case ElementType.Int:
                    return new PListInteger ();

                case ElementType.Real:
                    return new PListReal ();

                case ElementType.String:
                    return new PListString ();

                default:
                    break;
                }
            }

            return element;
        }

        protected override void DrawDictionaryCommon(PListDictionary dic)
        {
            _indentLevel++;
            string entryToRemove = "";
            string keyToUpdate = "";
            string newKeyValue = "";
            string valueToUpdateKey = "";
            IPListElement valueToUpdate = null;
            Color original = GUI.color;

            foreach (var kvp in dic)
            {
                Style.IndentedHorizontalLine(Styling.ROW_COLOR, _indentLevel * INDENT_AMOUNT);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(_indentLevel * INDENT_AMOUNT);
                bool open = DrawFoldout(kvp.Value);

                try
                {
                    //draw the key
                    EditorGUI.BeginChangeCheck();
                    string key = EditorGUILayout.TextField(kvp.Key, GUILayout.MinWidth(KEY_WIDTH), GUILayout.MaxWidth(KEY_WIDTH));

                    if (EditorGUI.EndChangeCheck())
                    {
                        keyToUpdate = kvp.Key;
                        newKeyValue = key;
                        IsDirty = true;
                    }

                    //draw type selector
                    IPListElement value = TypeSelector(kvp.Value);

                    if (value != kvp.Value)
                    {
                        valueToUpdateKey = key;
                        valueToUpdate = value;
                        IsDirty = true;
                    }
                }
                catch
                {
                    Debug.LogError("EgoXproject: Failed to update key: " + kvp.Key);
                }

                //draw the value
                DrawElement(kvp.Value);

                if (RemoveElement(kvp.Key))
                {
                    entryToRemove = kvp.Key;
                    IsDirty = true;
                }

                EditorGUILayout.EndHorizontal();

                if (!open)
                {
                    continue;
                }

                //if element is a dictionary or an array, draw its entries
                if (kvp.Value is PListDictionary)
                {
                    DrawDictionaryCommon(kvp.Value as PListDictionary);
                }
                else if (kvp.Value is PListArray)
                {
                    DrawArrayContent(kvp.Value as PListArray);
                }
            }

            GUI.color = original;

            //perform dictionary maintenance
            if (!string.IsNullOrEmpty(keyToUpdate))
            {
                UpdateDictionaryKey(dic, keyToUpdate, newKeyValue);
            }

            if (!string.IsNullOrEmpty(valueToUpdateKey))
            {
                RemoveFoldoutEntry(dic [valueToUpdateKey]);
                dic [valueToUpdateKey] = valueToUpdate;
            }

            if (!string.IsNullOrEmpty(entryToRemove))
            {
                RemoveFoldoutEntry(dic [entryToRemove]);
                dic.Remove(entryToRemove);
            }

            if (AddElement())
            {
                string newKeyName = "New Key";
                int count = 1;

                while (dic.ContainsKey(newKeyName))
                {
                    newKeyName = "New Key " + count;
                    count++;
                }

                dic.Add(newKeyName, new PListString());
                IsDirty = true;
            }

            _indentLevel--;
        }

        protected void UpdateDictionaryKey(PListDictionary dict, string oldKey, string newKey)
        {
            if (oldKey == newKey)
            {
                return;
            }

            if (string.IsNullOrEmpty(newKey))
            {
                return;
            }

            if (dict.ContainsKey(newKey))
            {
                return;
            }

            var element = dict[oldKey];
            dict.Remove(oldKey);
            dict.Add(newKey, element);
        }

        protected override void DrawDictionary(PListDictionary dic)
        {
            Style.MinWidthLabel("(" + dic.Count + (dic.Count == 1 ? " item)" : " items)"), 20);
            EditorGUILayout.Space();
        }

        protected override void DrawArray(PListArray array)
        {
            Style.MinWidthLabel("(" + array.Count + (array.Count == 1 ? " item)" : " items)"), 20);
            EditorGUILayout.Space();
        }

        void DrawArrayContent(PListArray array)
        {
            int entryToRemove = -1;
            _indentLevel++;
            Color original = GUI.color;

            for (int ii = 0; ii < array.Count; ++ii)
            {
                var element = array[ii];
                Style.IndentedHorizontalLine(Styling.ROW_COLOR, _indentLevel * INDENT_AMOUNT);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(_indentLevel * INDENT_AMOUNT);
                bool open = DrawFoldout(element);
                //item number
                string name = "Item " + ii;
                EditorGUILayout.LabelField(name, GUILayout.MinWidth(KEY_WIDTH), GUILayout.MaxWidth(KEY_WIDTH));
                //type
                IPListElement value = TypeSelector(element);

                if (value != element)
                {
                    RemoveFoldoutEntry(element);
                    array[ii] = value;
                    element = array[ii];
                    IsDirty = true;
                }

                //draw the value
                if (element is PListDictionary)
                {
                    var d = element as PListDictionary;
                    Style.MinWidthLabel("(" + d.Count + (d.Count == 1 ? " item)" : " items)"), 20);
                    EditorGUILayout.Space();
                }
                else if (element is PListArray)
                {
                    var a = element as PListArray;
                    Style.MinWidthLabel("(" + a.Count + (a.Count == 1 ? " item)" : " items)"), 20);
                    EditorGUILayout.Space();
                }
                else
                {
                    DrawElement(element);
                }

                if (RemoveElement(name))
                {
                    entryToRemove = ii;
                    IsDirty = true;
                }

                EditorGUILayout.EndHorizontal();

                if (!open)
                {
                    continue;
                }

                //if element is a dictionary or an array, draw its entries
                if (element is PListDictionary)
                {
                    DrawDictionaryCommon(element as PListDictionary);
                }
                else if (element is PListArray)
                {
                    DrawArrayContent(element as PListArray);
                }
            }

            GUI.color = original;

            if (AddElement())
            {
                array.Add(new PListString());
                IsDirty = true;
            }

            if (entryToRemove >= 0)
            {
                RemoveFoldoutEntry(array[entryToRemove]);
                array.RemoveAt(entryToRemove);
            }

            _indentLevel--;
        }

        protected override void DrawBool(PListBoolean element)
        {
            BoolEnum b = element.Value ? BoolEnum.Yes : BoolEnum.No;
            EditorGUI.BeginChangeCheck();
            b = (BoolEnum) EditorGUILayout.EnumPopup(b, GUILayout.MinWidth(TYPE_WIDTH), GUILayout.MaxWidth(TYPE_WIDTH));

            if (EditorGUI.EndChangeCheck())
            {
                element.Value = b == BoolEnum.Yes;
                IsDirty = true;
            }

            GUILayout.FlexibleSpace();
        }

        protected override void DrawInt(PListInteger element)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.IntField(element.IntValue);

            if (EditorGUI.EndChangeCheck())
            {
                element.IntValue = value;
                IsDirty = true;
            }
        }

        protected override void DrawReal(PListReal element)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.FloatField(element.FloatValue);

            if (EditorGUI.EndChangeCheck())
            {
                element.FloatValue = value;
                IsDirty = true;
            }
        }

        protected override void DrawString(PListString element)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.TextField(element.Value);

            if (EditorGUI.EndChangeCheck())
            {
                element.Value = value;
                IsDirty = true;
            }
        }

        protected override void DrawDate(PListDate element)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.TextField(element.StringValue);

            if (EditorGUI.EndChangeCheck())
            {
                element.StringValue = value;
                IsDirty = true;
            }
        }

        protected override void DrawData(PListData element)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.TextField(element.Value);

            if (EditorGUI.EndChangeCheck())
            {
                element.Value = value;
                IsDirty = true;
            }
        }

        protected bool RemoveElement(string key)
        {
            if (Style.MinusButton("Remove " + key + " entry"))
            {
                return EditorUtility.DisplayDialog("Confirm Delete", "Are you sure you want to remove the " + key + " entry?", "Remove", "Cancel");
            }

            return false;
        }

        protected bool AddElement()
        {
            _indentLevel++;
            Style.IndentedHorizontalLine(Styling.ROW_COLOR, (_indentLevel - 1)*INDENT_AMOUNT);
            EditorGUILayout.BeginHorizontal();
            bool b = false;
            GUILayout.FlexibleSpace();

            if (Style.PlusButton("Add a new entry"))
            {
                b = true;
            }

            EditorGUILayout.EndHorizontal();
            _indentLevel--;
            return b;
        }
    }
}
