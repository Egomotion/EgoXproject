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
    /// <summary>
    /// PBX group.
    /// If group references a folder in the project dir, then it only has a path entry
    /// If it is a virtual group, then it only has a name entry
    /// If it is an external folder it has both
    /// A virtual sub folder has a path if it is under a real folder. its path is .. to take you up to the correct path when concatinating. further virtual folders have no path
    /// A real folder under another real folder, will have its path modifed to be relative if they are not naturally under each other
    /// </summary>

    internal class PBXGroup : PBXBaseObject
    {
        const string CHILDREN_KEY = "children";
        const string NAME_KEY = "name";
        const string PATH_KEY = "path";
        const string SOURCE_TREE_KEY = "sourceTree";

        List<PBXBaseObject> _children = new List<PBXBaseObject>();

        public PBXGroup(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXGroup, uid, dict)
        {
            Init();
        }

        //used by XCVersionGroup and XCVariantGroup
        protected PBXGroup(PBXTypes isa, string uid, PBXProjDictionary dict)
        : base(isa, uid, dict)
        {
            Init();
        }

        void Init()
        {
            if (!Dict.ContainsKey(CHILDREN_KEY))
            {
                Dict[CHILDREN_KEY] = new PBXProjArray();
            }
        }

        protected static PBXProjDictionary AddPathAndName(PBXProjDictionary dic, string path)
        {
            //path
            //TODO check to see if we need to remove quotes
            string unquotedPath = path;//.FromLiteral();
            dic.Add(PATH_KEY, unquotedPath.ToLiteralIfRequired());

            //optional name
            if (path.StartsWith("../", System.StringComparison.InvariantCultureIgnoreCase) ||
                    path.StartsWith("./", System.StringComparison.InvariantCultureIgnoreCase))
            {
                var fileName = System.IO.Path.GetFileName(unquotedPath);
                dic.Add(NAME_KEY, fileName.ToLiteralIfRequired());
            }

            return dic;
        }

        //folder on disk
        //have path, and only have a name if path is external to project
        public static PBXGroup CreateWithPath(string uid, string path, PBXGroup parentGroup)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new System.ArgumentNullException(nameof (path), "path cannot be null or empty");
            }

            var dic = CommonCreate(uid, PBXTypes.PBXGroup);
            dic = AddPathAndName(dic, path);
            var grp = new PBXGroup(uid, dic);
            grp.ParentGroup = parentGroup;
            return grp;
        }

        //virtual groups that do not have a location on disk only have a name
        public static PBXGroup CreateWithName(string uid, string name, PBXGroup parentGroup)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentNullException(nameof (name), "name cannot be null or empty");
            }

            var dic = CommonCreate(uid, PBXTypes.PBXGroup);
            dic.Add(NAME_KEY, name.ToLiteralIfRequired());
            var grp = new PBXGroup(uid, dic);
            grp.ParentGroup = parentGroup;
            return grp;
        }

        protected static PBXProjDictionary CommonCreate(string uid, PBXTypes type)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException(nameof (uid), "uid cannot be null or empty");
            }

            PBXProjDictionary emptyDic = new PBXProjDictionary();
            //isa
            emptyDic.Add(isaKey, type.ToString());
            emptyDic.Add(CHILDREN_KEY, new PBXProjArray());
            emptyDic.Add(SOURCE_TREE_KEY, SourceTreeLocation.GROUP);
            return emptyDic;
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            _children = PopulateObjects<PBXBaseObject>(ChildrenIDs, allObjects);
            ParentGroup = FindParentGroup(allObjects);
        }

        #endregion


        PBXProjArray ChildrenUIDs
        {
            get
            {
                return Dict.ArrayValue(CHILDREN_KEY);
            }
        }

        public string[] ChildrenIDs
        {
            get
            {
                return ChildrenUIDs.ToStringArray();
            }
        }

        public PBXBaseObject[] Children
        {
            get
            {
                return _children.ToArray();
            }
        }

        public PBXFileReference[] ChildFileReferences
        {
            get
            {
                return _children.OfType<PBXFileReference>().ToArray();
            }
        }

        public PBXGroup[] ChildGroups
        {
            get
            {
                return _children.OfType<PBXGroup>().ToArray();
            }
        }

        public PBXVariantGroup[] ChildVariants
        {
            get
            {
                return _children.OfType<PBXVariantGroup>().ToArray();
            }
        }

        public bool HasChildWithUID(string uid)
        {
            return System.Array.IndexOf(ChildrenIDs, uid) > -1;
        }

        public int ChildCount
        {
            get
            {
                return _children.Count;
            }
        }


        public void AddChild(PBXBaseObject obj)
        {
            if (obj == null)
            {
                return;
            }

            if (ChildrenIDs.Contains(obj.UID))
            {
                return;
            }

            _children.Add(obj);
            ChildrenUIDs.Add(obj.UID);
        }

        public void RemoveChild(PBXBaseObject child)
        {
            if (child == null)
            {
                return;
            }

            RemoveChild(child.UID);
        }

        public void RemoveChild(string childUID)
        {
            var children = ChildrenUIDs;
            var res = children.Where(o => (o is PBXProjString) && (o as PBXProjString).Value == childUID);

            foreach (var r in res)
            {
                children.Remove(r);
            }

            var objs = _children.Where(o => o.UID == childUID);

            foreach (var obj in objs)
            {
                _children.Remove(obj);
            }
        }

        public void RemoveAllChildren()
        {
            ChildrenUIDs.Clear();
        }

        public string Name
        {
            get
            {
                return Dict.StringValue(NAME_KEY).FromLiteral();
            }

            private set
            {
                Dict[NAME_KEY] = new PBXProjString(value.ToLiteralIfRequired());
            }
        }

        public string Path
        {
            get
            {
                return Dict.StringValue(PATH_KEY).FromLiteral();
            }

            protected set
            {
                Dict[PATH_KEY] = new PBXProjString(value.ToLiteralIfRequired());
            }
        }

        public string SourceTree
        {
            get
            {
                return Dict.StringValue(SOURCE_TREE_KEY);
            }
            set
            {
                Dict[SOURCE_TREE_KEY] = new PBXProjString(value);
            }
        }

        public PBXGroup GroupContainingObject(PBXBaseObject obj)
        {
            if (_children.Contains(obj))
            {
                return this;
            }

            foreach (var g in ChildGroups)
            {
                var containing = g.GroupContainingObject(obj);

                if (containing != null)
                {
                    return containing;
                }
            }

            return null;
        }

        public virtual string FullPath
        {
            get
            {
                if (SourceTree == SourceTreeLocation.GROUP)
                {
                    if (ParentGroup != null)
                    {
                        return System.IO.Path.Combine(ParentGroup.FullPath, Path);
                    }
                    else
                    {
                        return Path;
                    }
                }
                else
                {
                    return Path;
                }
            }
        }

        public PBXGroup ParentGroup
        {
            get;
            protected set;
        }

        public PBXGroup GetGroupWithName(string name, bool searchChildren)
        {
            var groups = ChildGroups;
            PBXGroup grp = groups.Where(o => o.Name == name).FirstOrDefault();

            if (grp != null)
            {
                return grp;
            }

            if (searchChildren)
            {
                foreach (var g in groups)
                {
                    grp = GetGroupWithName(name, searchChildren);

                    if (grp != null)
                    {
                        return grp;
                    }
                }
            }

            return null;
        }

        //path should be relative to the project folder and be in the group already
        public string PathRelativeToGroup(string path)
        {
            string fullPath = FullPath;

            if (string.IsNullOrEmpty(fullPath))
            {
                return path;
            }

            if (path.StartsWith(fullPath, System.StringComparison.InvariantCultureIgnoreCase))
            {
                path = path.Remove(0, fullPath.Length);
            }

            if (path.StartsWith("/", System.StringComparison.InvariantCultureIgnoreCase))
            {
                path = path.Remove(0, 1);
            }

            return path;
        }
    }
}
