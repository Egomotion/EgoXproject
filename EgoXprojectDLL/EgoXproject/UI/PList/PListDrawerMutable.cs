//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Egomotion.EgoXproject.Internal;
using Egomotion.EgoXproject.UI.Internal;

//TODO check saving of this class as now uses IsDirty.
namespace Egomotion.EgoXproject.UI
{
    internal class PListDrawerMutable : PListElementDrawerMutable
    {
        PList _plist;
        Vector2 _scrollPos;

        public PListDrawerMutable(Styling style)
        : base(style)
        {
        }

        public void Draw()
        {
            if (_plist == null)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            Style.MinWidthBoldLabel("File");
            EditorGUILayout.LabelField(_plist.SavePath);
            EditorGUILayout.EndHorizontal();
            Style.HorizontalLine();
            DrawPList();

            if (IsDirty)
            {
                Save();
            }
        }

        public PList Data
        {
            get
            {
                return _plist;
            }
            set
            {
                _plist = value;
            }
        }

        protected void Save()
        {
            if (_plist != null && _plist.HasPath)
            {
                _plist.Save();
                IsDirty = false;
            }
        }

        void DrawPList()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            DrawRootDictionary(_plist.Root);
            EditorGUILayout.EndScrollView();
        }

        void DrawRootDictionary(PListDictionary dict)
        {
            EditorGUILayout.BeginVertical();
            DrawPList(dict);
            EditorGUILayout.EndVertical();
        }
    }
}
