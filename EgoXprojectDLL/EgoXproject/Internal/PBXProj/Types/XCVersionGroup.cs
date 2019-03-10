//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class XCVersionGroup : PBXGroup
    {
        const string CURRENT_VERSION_KEY = "currentVersion";
        const string VERSION_GROUP_TYPE_KEY = "versionGroupType";

        public XCVersionGroup(string uid, PBXProjDictionary dict)
        : base(PBXTypes.XCVersionGroup, uid, dict)
        {
        }

        //TODO can XCVersion Groups have a name?
        public static XCVersionGroup Create(string uid, string path, PBXGroup parentGroup)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException(nameof (uid), "uid cannot be null or empty");
            }

            var dic = CommonCreate(uid, PBXTypes.XCVersionGroup);
            dic = AddPathAndName(dic, path);
            var xcVersionGroup = new XCVersionGroup(uid, dic);
            xcVersionGroup.VersionGroupType = PBXFileTypeHelper.FileTypeFromFileName(path).GetXcodeDataValue();
            xcVersionGroup.ParentGroup = parentGroup;
            return xcVersionGroup;
        }

        public string CurrentVersionID
        {
            get
            {
                string version = Dict.StringValue(CURRENT_VERSION_KEY);

                if (string.IsNullOrEmpty(version))
                {
                    var childIds = ChildrenIDs;

                    if (childIds.Length > 0)
                    {
                        version = childIds[0];
                        Dict[CURRENT_VERSION_KEY] = new PBXProjString(version);
                    }
                }

                return version;
            }
            set
            {
                if (ChildrenIDs.Contains(value))
                {
                    Dict[CURRENT_VERSION_KEY] = new PBXProjString(value);
                }
            }
        }

        //TODO do we need to link this up?
        //      PBXFileReference CurrentVersion { get; private set; }

        public string VersionGroupType
        {
            get
            {
                return Dict.StringValue(VERSION_GROUP_TYPE_KEY);
            }
            set
            {
                Dict[VERSION_GROUP_TYPE_KEY] = new PBXProjString(value);
            }
        }
    }
}
