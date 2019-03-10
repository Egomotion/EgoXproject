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
    internal abstract class PBXBaseObject
    {
        protected const string isaKey = "isa";

        /// <summary>
        /// Initializes a new instance of the <see cref="Egomotion.EgoXproject.Internal.PBXBaseObject"/> class.
        /// </summary>
        /// <param name="isa">Isa.</param> The type of object it is
        /// <param name="uid">Uid.</param> The UID in the project objects dictionary for this object
        /// <param name="dict">Dict.</param> The dictionary entry in the project objects dictionary for this object
        protected PBXBaseObject(PBXTypes isa, string uid, PBXProjDictionary dict)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException(nameof (uid), "Dictionary cannot be null");
            }

            if (dict == null)
            {
                throw new System.ArgumentNullException(nameof (dict), "Dictionary cannot be null");
            }

            var isaValue = dict.Element<PBXProjString>(isaKey);

            if (isaValue == null)
            {
                throw new System.ArgumentException("Dictionary must contain an isa key with a string value", nameof (dict));
            }

            if (isa.ToString() != isaValue.Value)
            {
                throw new System.ArgumentException("Dictionary must be a " + isa + " dictionary and not a " + isaValue.Value + " dictionary", nameof (dict));
            }

            Dict = dict;
            UID = uid;
            Isa = isa;
        }

        /// <summary>
        /// This is ref to the dictionary entry in the project dictionary object.
        /// </summary>
        /// <value>The dict.</value>
        public PBXProjDictionary Dict
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the unique id for the object
        /// </summary>
        /// <value>The unique ID</value>
        public string UID
        {
            get;
            private set;
        }

        public PBXTypes Isa
        {
            get;
            private set;
        }

        public abstract void Populate(Dictionary<string, PBXBaseObject> allObjects);

        #region helper funcs

        protected List<T> PopulateObjects<T>(string[] ids, Dictionary<string, PBXBaseObject> allObjects) where T : PBXBaseObject
        {
            List<T> results = new List<T>();

            if (ids == null || ids.Length <= 0)
            {
                return results;
            }

            foreach (var id in ids)
            {
                T obj = PopulateObject<T>(id, allObjects);

                if (obj != null)
                {
                    results.Add(obj);
                }
            }

            return results;
        }

        protected T PopulateObject<T>(string id, Dictionary<string, PBXBaseObject> allObjects) where T : PBXBaseObject
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            PBXBaseObject obj = null;

            if (allObjects.TryGetValue(id, out obj))
            {
                if (obj is T)
                {
                    return obj as T;
                }
                else
                {
                    Debug.LogWarning("EgoXproject: The project.pbxproj file has possibly been corrupted by another plugin. " + id + " should be a " + typeof(T) + " but is a " + obj.GetType());
                    throw new System.Exception("Referenced object is unexpected type: " + obj.GetType());
                }
            }
            else
            {
                Debug.LogWarning("EgoXproject: The project.pbxproj file has possibly been corrupted by another plugin. The referenced id cannot be found: " + id + ".");
                return null;
            }
        }

        protected PBXGroup FindParentGroup(Dictionary<string, PBXBaseObject> allObjects)
        {
            var groups = allObjects.Values.OfType<PBXGroup>();

            foreach (PBXGroup grp in groups)
            {
                if (grp.HasChildWithUID(UID))
                {
                    return grp;
                }
            }

            return null;
        }

        #endregion
    }
}
