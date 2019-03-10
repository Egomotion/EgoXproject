// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//
using System;
using UnityEngine;
using UnityEditor;
using Egomotion.EgoXproject.UI.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal class TextEditPopover : EditorWindow
    {
        public delegate void OnSetText(string originalText, string newText);
        public delegate bool ValidationFunction(string text);

        OnSetText _onSetText;
        ValidationFunction _validateFunc;
        string _originalText;
        string _text;
        string _title;
        string _description;
        string _okButton;

        Styling _style;

        public static void Init(Vector2 centerPosition,
                                float width,
                                string title,
                                string description,
                                string text,
                                OnSetText setTextCallback,
                                ValidationFunction validateFunc,
                                string okButtonText,
                                Styling style)
        {
            var descContent = new GUIContent(description);
            var height = GUI.skin.label.CalcHeight(descContent, width);

            if (height > EditorGUIUtility.singleLineHeight)
            {
                height -= EditorGUIUtility.singleLineHeight;
            }

            var win = EditorWindow.CreateInstance<TextEditPopover>();
            var size = new Vector2(width, 90 + height);
            var rect = new Rect(centerPosition.x - size.x * 0.5f, centerPosition.y - size.y * 0.5f, 0, 0);
            win.Configure(title, description, text, setTextCallback, validateFunc, okButtonText, style);
            win.ShowAsDropDown(rect, size);
        }

        void Configure(string popoverTitle,
                       string description,
                       string text,
                       OnSetText setTextCallback,
                       ValidationFunction validateFunc,
                       string okButtonText,
                       Styling style)
        {
            _title = popoverTitle;
            _description = description;
            _originalText = _text = text;
            _onSetText = setTextCallback;
            _validateFunc = validateFunc;
            _okButton = okButtonText;
            _style = style;
        }

        void OnGUI()
        {
            if (!string.IsNullOrEmpty(_title))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                _style.MinWidthBoldLabel(_title);
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                _style.HorizontalLine();
            }

            if (!string.IsNullOrEmpty(_description))
            {
                EditorGUILayout.LabelField(_description, EditorStyles.wordWrappedLabel  );
            }

            EditorGUILayout.BeginHorizontal();
            GUI.SetNextControlName("TextBox");
            _text = EditorGUILayout.TextField(_text);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            bool valid = Validate();
            GUI.enabled = valid;

            if (GUILayout.Button(_okButton))
            {
                SetText();
                Close();
            }

            GUI.enabled = true;

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.KeyDown)
            {
                if (valid && Event.current.keyCode == KeyCode.Return)
                {
                    SetText();
                    Close();
                }
                else if (Event.current.keyCode == KeyCode.Escape)
                {
                    Close();
                }
            }
        }

        void SetText()
        {
            if (_onSetText != null)
            {
                _onSetText(_originalText, _text);
            }
        }

        bool Validate()
        {
            if (_validateFunc != null)
            {
                return _validateFunc(_text);
            }

            return true;
        }
    }
}
