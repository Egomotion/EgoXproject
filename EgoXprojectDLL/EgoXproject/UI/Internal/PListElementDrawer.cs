// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------


using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using Egomotion.EgoXproject.UI.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal class PListElementDrawer : BasePListElementDrawer
    {
        public PListElementDrawer(Styling style)
        : base(style)
        {
        }

        void TypeSelector(IPListElement element)
        {
            string type = "";

            if (element is PListDictionary)
            {
                type = "Dictionary";
            }
            else if (element is PListArray)
            {
                type = "Array";
            }
            else if (element is PListBoolean)
            {
                type = "Boolean";
            }
            else if (element is PListInteger)
            {
                type = "Integer";
            }
            else if (element is PListReal)
            {
                type = "Real";
            }
            else if (element is PListString)
            {
                type = "String";
            }
            else if (element is PListDate)
            {
                type = "Date";
            }
            else if (element is PListData)
            {
                type = "Data";
            }

            Style.FixedWidthLabel(type, TYPE_WIDTH);
        }

        protected override void DrawDictionaryCommon(PListDictionary dic)
        {
            _indentLevel++;

            foreach (var kvp in dic)
            {
                Style.IndentedHorizontalLine(Styling.ROW_COLOR, _indentLevel * INDENT_AMOUNT);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(_indentLevel * INDENT_AMOUNT);
                bool open = DrawFoldout(kvp.Value);
                Style.FixedWidthLabel(kvp.Key, KEY_WIDTH);
                TypeSelector(kvp.Value);
                DrawElement(kvp.Value);
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

            _indentLevel--;
        }

        protected override void DrawDictionary(PListDictionary dic)
        {
            GUILayout.FlexibleSpace();
            Style.MinWidthLabel("(" + dic.Count + (dic.Count == 1 ? " item)" : " items)"), PADDING);
            EditorGUILayout.Space();
        }

        protected override void DrawArray(PListArray array)
        {
            GUILayout.FlexibleSpace();
            Style.MinWidthLabel("(" + array.Count + (array.Count == 1 ? " item)" : " items)"), PADDING);
            EditorGUILayout.Space();
        }

        void DrawArrayContent(PListArray array)
        {
            _indentLevel++;

            for (int ii = 0; ii < array.Count; ++ii)
            {
                var element = array[ii];
                Style.IndentedHorizontalLine(Styling.ROW_COLOR, _indentLevel * INDENT_AMOUNT);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(_indentLevel * INDENT_AMOUNT);
                bool open = DrawFoldout(element);
                string name = "Item " + ii;
                Style.FixedWidthLabel(name, KEY_WIDTH);
                TypeSelector(element);

                //draw the value
                if (element is PListDictionary)
                {
                    GUILayout.FlexibleSpace();
                    var d = element as PListDictionary;
                    Style.MinWidthLabel("(" + d.Count + (d.Count == 1 ? " item)" : " items)"), PADDING);
                    EditorGUILayout.Space();
                }
                else if (element is PListArray)
                {
                    GUILayout.FlexibleSpace();
                    var a = element as PListArray;
                    Style.MinWidthLabel("(" + a.Count + (a.Count == 1 ? " item)" : " items)"), PADDING);
                    EditorGUILayout.Space();
                }
                else
                {
                    DrawElement(element);
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

            _indentLevel--;
        }

        protected override void DrawBool(PListBoolean element)
        {
            Style.MinWidthLabel((element.Value ? "Yes" : "No"), PADDING);
        }

        protected override void DrawInt(PListInteger element)
        {
            Style.MinWidthLabel(element.ToString(), PADDING);
        }

        protected override void DrawReal(PListReal element)
        {
            Style.MinWidthLabel(element.ToString(), PADDING);
        }

        protected override void DrawString(PListString element)
        {
            Style.MinWidthLabel(element.ToString(), PADDING);
        }

        protected override void DrawDate(PListDate element)
        {
            Style.MinWidthLabel(element.ToString(), PADDING);
        }

        protected override void DrawData(PListData element)
        {
            Style.MinWidthLabel(element.ToString(), PADDING);
        }
    }
}

