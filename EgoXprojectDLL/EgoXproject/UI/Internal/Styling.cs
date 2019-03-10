//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.UI.Internal
{
    internal class Styling
    {
        public static readonly Color ROW_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        TextureResources _textures;
        Dictionary<string, GUIStyle> Styles
        {
            get;
            set;
        }

        public Styling()
        {
            _textures = new TextureResources();
            Styles = new Dictionary<string, GUIStyle>();
        }

        public void Load()
        {
            _textures.Load();
        }

        public void Unload()
        {
            _textures.Unload();
            Styles.Clear();
        }

        float InternalMinWidthLabel(string text, GUIStyle style, float padding = 0.0f)
        {
            var content = new GUIContent(text);
            float contentWidth = style.CalcSize(content).x + padding;
            EditorGUILayout.LabelField(content, style, GUILayout.Width(contentWidth), GUILayout.MinWidth(contentWidth), GUILayout.ExpandWidth(false));
            return contentWidth;
        }

        public float MinWidthLabel(string text, float padding = 0.0f)
        {
            return InternalMinWidthLabel(text, EditorStyles.label, padding);
        }

        public float MinWidthBoldLabel(string text, float padding = 0.0f)
        {
            return InternalMinWidthLabel(text, EditorStyles.boldLabel, padding);
        }

        public float MinWidthMiniBoldLabel(string text, float padding = 0.0f)
        {
            return InternalMinWidthLabel(text, EditorStyles.miniBoldLabel, padding);
        }

        public void FixedWidthLabel(string text, float width)
        {
            EditorGUILayout.LabelField(text, GUILayout.Width(width), GUILayout.ExpandWidth(false));
        }

        public void FixedWidthLabel(string text, string tooltip, float width)
        {
            EditorGUILayout.LabelField(new GUIContent(text, tooltip), GUILayout.Width(width), GUILayout.ExpandWidth(false));
        }

        public void HorizontalLine(Color color)
        {
            var original = GUI.color;
            GUI.color = color;
            EditorGUILayout.BeginHorizontal(RowEntry());
            GUI.color = original;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public void HorizontalLine()
        {
            HorizontalLine(ROW_COLOR);
        }

        public void IndentedHorizontalLine(Color color, float indent = 20.0f)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indent);
            HorizontalLine(color);
            EditorGUILayout.EndHorizontal();
        }

        public void IndentedHorizontalLine(float indent = 20.0f)
        {
            IndentedHorizontalLine(ROW_COLOR, indent);
        }

        public float MaxLabelWidth(string[] labels, float minWidth = 0.0f)
        {
            float finalWidth = minWidth;

            foreach (var label in labels)
            {
                float width = EditorStyles.label.CalcSize(new GUIContent(label)).x;
                finalWidth = Mathf.Max(width, finalWidth);
            }

            return finalWidth;
        }


        public float MaxLabelWidth(string[] labels, float minWidth, float maxWidth)
        {
            float finalWidth = MaxLabelWidth(labels, minWidth);
            return Mathf.Min(finalWidth, maxWidth);
        }

        public bool PlusButton(string tooltip)
        {
            var tex = _textures.Plus;

            if (tex)
            {
                return SquareIconButton(tex, tooltip);
            }
            else
            {
                return SquareButton("+", tooltip);
            }
        }

        public bool LargePlusButton(string tooltip)
        {
            var tex = _textures.Plus;
            float size = 40.0f;

            if (tex)
            {
                return SquareIconButton(tex, tooltip, size);
            }
            else
            {
                return SquareButton("+", tooltip, size);
            }
        }

        public bool MinusButton(string tooltip)
        {
            var tex = _textures.Minus;

            if (tex)
            {
                return SquareIconButton(tex, tooltip);
            }
            else
            {
                return SquareButton("-", tooltip);
            }
        }

        public bool EditButton(string tooltip)
        {
            var tex = _textures.Edit;

            if (tex)
            {
                float size = 20.0f;
                return GUILayout.Button(new GUIContent(tex, tooltip), IconButton(), GUILayout.Width(size), GUILayout.ExpandWidth(false), GUILayout.Height(size), GUILayout.ExpandHeight(false));
            }
            else
            {
                return GUILayout.Button(new GUIContent("edit", tooltip), GUILayout.Width(34), GUILayout.ExpandWidth(false));
            }
        }

        public bool HelpButton(string tooltip)
        {
            var tex = _textures.Help;

            if (tex)
            {
                return SquareIconButton(tex, tooltip);
            }
            else
            {
                return SquareButton("?", tooltip);
            }
        }

        public bool RefreshButton(string tooltip)
        {
            var tex = _textures.Refresh;

            if (tex)
            {
                float size = 20.0f;
                return GUILayout.Button(new GUIContent(tex, tooltip), IconButton(), GUILayout.Width(size), GUILayout.ExpandWidth(false), GUILayout.Height(size), GUILayout.ExpandHeight(false));
            }
            else
            {
                return GUILayout.Button(new GUIContent("Refresh", tooltip), GUILayout.MaxWidth(60));
            }
        }


        public bool SquareButton(string label, string tooltip, float size = 20.0f)
        {
            return GUILayout.Button(new GUIContent(label, tooltip), GUILayout.Width(size), GUILayout.ExpandWidth(false), GUILayout.Height(size), GUILayout.ExpandHeight(false));
        }

        public bool SquareIconButton(Texture2D tex, string tooltip, float size = 20.0f)
        {
            return GUILayout.Button(new GUIContent(tex, tooltip), GUILayout.Width(size), GUILayout.ExpandWidth(false), GUILayout.Height(size), GUILayout.ExpandHeight(false));
        }

        public void WarningIcon(string tooltip)
        {
            float size = 20.0f;
            GUILayout.Label(new GUIContent(_textures.Warning, tooltip), GUILayout.Width(size), GUILayout.ExpandWidth(false), GUILayout.Height(size), GUILayout.ExpandHeight(false));
        }

        public GUIStyle IconButton()
        {
            const string key = "egoIconButton";

            if (!Styles.ContainsKey(key))
            {
                var style = new GUIStyle(GUI.skin.button);
                style.name = key;
                style.padding = new RectOffset(4, 4, 4, 3);
                Styles.Add(key, style);
            }

            return Styles[key];
        }


        public GUIStyle RowEntry()
        {
            const string key = "egoRowEntry";

            if (!Styles.ContainsKey(key))
            {
                var style = new GUIStyle(GUI.skin.box);
                style.normal.background = _textures.TopBorder;
                style.name = key;
                Styles.Add(key, style);
            }

            return Styles[key];
        }

        public GUIStyle Foldout()
        {
            const string key = "egoFoldout";

            if (!Styles.ContainsKey(key))
            {
                var style = new GUIStyle(GUI.skin.GetStyle("foldout"));
                style.name = key;
                style.font = EditorStyles.boldFont;
                Styles.Add(key, style);
            }

            return Styles[key];
        }

        public GUIStyle TextArea()
        {
            const string key = "egoTextArea";

            if (!Styles.ContainsKey(key))
            {
                var style = new GUIStyle(GUI.skin.GetStyle("foldout"));
                style.name = key;
                var color = new Color(0.706f, 0.706f, 0.706f, 1.0f);
                style.active.textColor = color;
                style.alignment = TextAnchor.UpperLeft;
                style.border = new RectOffset(3, 3, 3, 3);
                style.clipping = TextClipping.Clip;
                style.contentOffset = Vector2.zero;
                style.fixedHeight = 0;
                style.fixedWidth = 0;
                style.focused.textColor = color;
                style.fontSize = 0;
                style.fontStyle = FontStyle.Normal;
                style.hover.textColor = color;
                style.imagePosition = ImagePosition.TextOnly;
                style.margin = new RectOffset(4, 4, 2, 2);
                var tex = _textures.TextAreaBorder;
                style.normal.background = tex;
                style.normal.textColor = color;
                style.onActive.textColor = color;
                style.onFocused.textColor = color;
                style.onHover.textColor = color;
                style.onNormal.textColor = color;
                style.overflow = new RectOffset(0, 0, 0, 0);
                style.padding = new RectOffset(3, 3, 1, 2);
                style.richText = false;
                style.stretchHeight = true;
                style.stretchWidth = true;
                style.wordWrap = true;
                Styles.Add(key, style);
            }

            return Styles[key];
        }

        public GUIStyle EmptyFoldout()
        {
            const string key = "egoEmptyFoldout";

            if (!Styles.ContainsKey(key))
            {
                var fstyle = new GUIStyle(EditorStyles.foldout);
                fstyle.fixedWidth = 0.0f;
                fstyle.stretchWidth = false;
                Styles.Add(key, fstyle);
            }

            return Styles[key];
        }

        public GUIStyle IndentedBox()
        {
            const string key = "egoIndentedBox";

            if (!Styles.ContainsKey(key))
            {
                var style = new GUIStyle(GUI.skin.box);
                style.margin.left = 20;
                style.margin.right = 20;
                Styles.Add(key, style);
            }

            return Styles[key];
        }

        public GUIStyle Box()
        {
            const string key = "egoBox";

            if (!Styles.ContainsKey(key))
            {
                var style = new GUIStyle(GUI.skin.box);
                style.margin.left = 2;
                style.margin.right = 2;
                Styles.Add(key, style);
            }

            return Styles[key];
        }
    }
}
