// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using Egomotion.EgoXproject.UI.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal abstract class BasePListElementDrawer
    {
        protected enum ElementType
        {
            Dictionary,
            Array,
            Bool,
            Int,
            Real,
            String,
            Date,
            Data
        };

        protected const float KEY_WIDTH = 200.0f;
        protected const float TYPE_WIDTH = 100.0f;
        protected const float INDENT_AMOUNT = 20.0f;
        protected const float PADDING = 20.0f;

        protected int _indentLevel = 0;

        Dictionary<int, bool> _foldouts = new Dictionary<int, bool>();

        protected BasePListElementDrawer (Styling styling)
        {
            Style = styling;
        }

        protected Styling Style
        {
            get;
            private set;
        }

        public void DrawPList(PListDictionary dic)
        {
            DrawHeader(dic.Count);
            DrawDictionaryCommon(dic);
            GUILayout.Space(4);
        }

        public void Reset()
        {
            ClearFoldoutEntrys();
        }

        void DrawHeader(int count)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(INDENT_AMOUNT);
            EditorGUILayout.LabelField("Key", EditorStyles.miniBoldLabel, GUILayout.Width(KEY_WIDTH));
            EditorGUILayout.LabelField("Type", EditorStyles.miniBoldLabel, GUILayout.Width(TYPE_WIDTH));
            EditorGUILayout.LabelField("Value", EditorStyles.miniBoldLabel);
            EditorGUILayout.Space();
            var countStr = count + (count == 1 ? " item" : " items");
            Style.MinWidthMiniBoldLabel(countStr, 20.0f);
            EditorGUILayout.EndHorizontal();
        }

        protected void DrawElement(IPListElement element)
        {
            if (element is PListDictionary)
            {
                DrawDictionary(element as PListDictionary);
            }
            else if (element is PListArray)
            {
                DrawArray(element as PListArray);
            }
            else if (element is PListBoolean)
            {
                DrawBool(element as PListBoolean);
            }
            else if (element is PListInteger)
            {
                DrawInt(element as PListInteger);
            }
            else if (element is PListReal)
            {
                DrawReal(element as PListReal);
            }
            else if (element is PListString)
            {
                DrawString(element as PListString);
            }
            else if (element is PListDate)
            {
                DrawDate(element as PListDate);
            }
            else if (element is PListData)
            {
                DrawData(element as PListData);
            }
        }

        protected bool DrawFoldout(IPListElement element)
        {
            bool open = true;

            if (element is PListDictionary || element is PListArray)
            {
                GUILayout.Space(-INDENT_AMOUNT + 2);

                if (!_foldouts.TryGetValue(element.GetHashCode(), out open))
                {
                    open = true;
                }

                _foldouts[element.GetHashCode()] = EditorGUILayout.Foldout(open, "", Style.EmptyFoldout());
                GUILayout.Space(-36);
            }

            return open;
        }

        protected void RemoveFoldoutEntry(IPListElement element)
        {
            _foldouts.Remove(element.GetHashCode());
        }

        protected void ClearFoldoutEntrys()
        {
            _foldouts.Clear();
        }

        protected abstract void DrawDictionary(PListDictionary dic);

        protected abstract void DrawDictionaryCommon(PListDictionary dic);

        protected abstract void DrawArray(PListArray array);

        protected abstract void DrawBool(PListBoolean element);

        protected abstract void DrawInt(PListInteger element);

        protected abstract void DrawReal(PListReal element);

        protected abstract void DrawString(PListString element);

        protected abstract void DrawDate(PListDate element);

        protected abstract void DrawData(PListData element);
    }
}

