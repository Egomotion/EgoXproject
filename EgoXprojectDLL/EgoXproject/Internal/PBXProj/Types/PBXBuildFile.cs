//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    //eg 7F36C11213C5C673007FBDD9 = {isa = PBXBuildFile; fileRef = 7F36C10F13C5C673007FBDD9; };
    //eg 7F36C11313C5C673007FBDD9 = {isa = PBXBuildFile; fileRef = 7F36C11013C5C673007FBDD9; settings = {ATTRIBUTES = (Weak, ); }; };
    // settings is a dic and can contain a COMPILER_FLAGS string entry and a ATTRIBUTES array
    internal class PBXBuildFile : PBXBaseObject
    {
        const string FILE_REF_KEY = "fileRef";
        const string SETTINGS_KEY = "settings";
        const string COMPILER_FLAGS_KEY = "COMPILER_FLAGS";
        const string ATTRIBUTES_KEY = "ATTRIBUTES";
        public const string WEAK_KEY = "Weak";
        public const string CODE_SIGN_ON_COPY_KEY = "CodeSignOnCopy";
        public const string REMOVE_HEADERS_ON_COPY_KEY = "RemoveHeadersOnCopy";

        public PBXBuildFile(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXBuildFile, uid, dict)
        {
        }

        public static PBXBuildFile Create(string uid, PBXBaseObject fileRef)    //should be a file ref, or a variant group or a version group
        {
            if (fileRef == null)
            {
                throw new System.ArgumentNullException(nameof(fileRef), "fileRef cannot be null");
            }

            return CreateCommon(uid, fileRef.UID);
        }

        static PBXBuildFile CreateCommon(string uid, string refUID)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException(nameof (uid), "uid cannot be null or empty");
            }

            if (string.IsNullOrEmpty(refUID))
            {
                throw new System.ArgumentNullException(nameof (refUID), "refUID cannot be null");
            }

            PBXProjDictionary emptyDic = new PBXProjDictionary();
            emptyDic.Add(isaKey, PBXTypes.PBXBuildFile.ToString());
            emptyDic.Add(FILE_REF_KEY, refUID);
            return new PBXBuildFile(uid, emptyDic);
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            ReferencedObject = PopulateObject<PBXBaseObject>(FileRefID, allObjects);
        }

        #endregion

        public string FileRefID
        {
            get
            {
                return Dict.StringValue(FILE_REF_KEY);
            }
        }

        public PBXBaseObject ReferencedObject
        {
            get;
            private set;
        }

        public PBXFileReference FileReference
        {
            get
            {
                return ReferencedObject as PBXFileReference;
            }
        }

        public PBXVariantGroup VariantGroup
        {
            get
            {
                return ReferencedObject as PBXVariantGroup;
            }
        }

        public XCVersionGroup VersionGroup
        {
            get
            {
                return ReferencedObject as XCVersionGroup;
            }
        }

        T GetSettingsEntry<T>(string key) where T : class, IPBXProjExpression
        {
            var settings = Dict.DictionaryValue(SETTINGS_KEY);

            if (settings != null)
            {
                return settings.Element<T>(key);
            }

            return null;
        }

        void SetSettingsEntry(string key, IPBXProjExpression value)
        {
            var settings = Dict.DictionaryValue(SETTINGS_KEY);

            if (settings == null)
            {
                settings = new PBXProjDictionary();
                Dict.Add(SETTINGS_KEY, settings);
            }

            settings[key] = value;
        }

        void RemoveSettingsEntry(string key)
        {
            var settings = Dict.DictionaryValue(SETTINGS_KEY);

            if (settings == null)
            {
                return;
            }

            settings.Remove(key);

            if (settings.Count <= 0)
            {
                Dict.Remove(SETTINGS_KEY);
            }
        }

        public void AddAttribute(string attribute)
        {
            var attribs = GetSettingsEntry<PBXProjArray>(ATTRIBUTES_KEY);

            if (attribs == null)
            {
                attribs = new PBXProjArray();
                SetSettingsEntry(ATTRIBUTES_KEY, attribs);
            }

            if (!attribs.ToStringArray().Contains(attribute))
            {
                attribs.Add(attribute);
            }
        }

        public void RemoveAttribute(string attribute)
        {
            var attribs = GetSettingsEntry<PBXProjArray>(ATTRIBUTES_KEY);

            if (attribs == null)
            {
                return;
            }

            for (int ii = 0; ii < attribs.Count; ii++)
            {
                if (attribs.StringValue(ii) == attribute)
                {
                    attribs.RemoveAt(ii);
                    break;
                }
            }

            if (attribs.Count <= 0)
            {
                RemoveSettingsEntry(ATTRIBUTES_KEY);
            }
        }

        public bool HasAttribute(string attribute)
        {
            var attribs = GetSettingsEntry<PBXProjArray>(ATTRIBUTES_KEY);

            if (attribs == null)
            {
                return false;
            }
            else
            {
                return attribs.ToStringArray().Contains(attribute);
            }
        }

        public string CompilerFlags
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    SetSettingsEntry(COMPILER_FLAGS_KEY, new PBXProjString(value.ToLiteralIfRequired()));
                }
                else
                {
                    RemoveSettingsEntry(COMPILER_FLAGS_KEY);
                }
            }
        }
    }
}
