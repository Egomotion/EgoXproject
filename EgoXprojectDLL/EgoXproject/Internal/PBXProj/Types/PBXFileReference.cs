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
    //Eg: 08B24F75137BFDFA00FBA309 = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = iAd.framework; path = System/Library/Frameworks/iAd.framework; sourceTree = SDKROOT; };
    //    1D6058910D05DD3D006BFB54 = {isa = PBXFileReference; explicitFileType = wrapper.application; includeInIndex = 0; path = ProductName.app; sourceTree = BUILT_PRODUCTS_DIR; };
    //    56C56C9717D6015100616839 = {isa = PBXFileReference; lastKnownFileType = folder.assetcatalog; name = Images.xcassets; path = "Unity-iPhone/Images.xcassets"; sourceTree = "<group>"; };
    //    56DBF99C15E3CDC9007A4A8D = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.cpp.objcpp; path = iPhone_Sensors.mm; sourceTree = "<group>"; };

    internal class PBXFileReference : PBXBaseObject
    {
        const string LAST_KNOWN_FILE_TYPE_KEY = "lastKnownFileType";
        const string NAME_KEY = "name";
        const string PATH_KEY = "path";
        const string SOURCE_TREE_KEY = "sourceTree";
        const string EXPLICIT_FILE_TYPE_KEY = "explicitFileType";
        const string FILE_ENCODING_KEY = "fileEncoding";
        const string INCLUDE_IN_INDEX_KEY = "includeInIndex";

        public PBXFileReference(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXFileReference, uid, dict)
        {
        }

        public static PBXFileReference CreateRelativeToSdkRoot(string uid, string path, PBXGroup group)
        {
            var fileRef = CommonCreate(uid, path, SourceTreeLocation.SDK_ROOT, group);
            return fileRef;
        }

        public static PBXFileReference CreateRelativeToSourceRoot(string uid, string path, PBXGroup group)
        {
            var fileRef = CommonCreate(uid, path, SourceTreeLocation.SOURCE_ROOT, group);
            return fileRef;
        }

        public static PBXFileReference CreateRelativeToGroup(string uid, string fileName, PBXGroup group)
        {
            var fileRef = CommonCreate(uid, fileName, SourceTreeLocation.GROUP, group);
            return fileRef;
        }

        public static PBXFileReference CreateWithAbsolutePath(string uid, string path, PBXGroup group)
        {
            var fileRef = CommonCreate(uid, path, SourceTreeLocation.ABSOLUTE, group);
            return fileRef;
        }

        public static PBXFileReference CreateBuildTarget(string uid, string fileName, PBXGroup group)
        {
            var fileRef = CommonCreate(uid, fileName, SourceTreeLocation.BUILT_PRODUCTS_DIR, group);
            return fileRef;
        }

        public static PBXFileReference CreateRelativeToDeveloperDirectory(string uid, string path, PBXGroup group)
        {
            var fileRef = CommonCreate(uid, path, SourceTreeLocation.DEVELOPER_DIR, group);
            return fileRef;
        }

        static void NullCheck(string uid, string path, PBXGroup group)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new System.ArgumentNullException(nameof (uid), "uid cannot be null or empty");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new System.ArgumentNullException(nameof (path), "path cannot be null or empty");
            }

            if (group == null)
            {
                throw new System.ArgumentNullException(nameof (group), "group cannot be null");
            }
        }

        static PBXFileReference CommonCreate(string uid, string path, string sourceTree, PBXGroup group)
        {
            NullCheck(uid, path, group);
            PBXProjDictionary emptyDic = new PBXProjDictionary();
            //isa
            emptyDic.Add(isaKey, PBXTypes.PBXFileReference.ToString());
            //path
            //TODO check that path will never have quotes or be a literal when entered
            //string unquotedPath = path.FromLiteral();
            string unquotedPath = path;
            emptyDic.Add(PATH_KEY, unquotedPath.ToLiteralIfRequired());
            //file type
            var fileType = PBXFileTypeHelper.FileTypeFromFileName(unquotedPath);

            if (sourceTree == SourceTreeLocation.BUILT_PRODUCTS_DIR)
            {
                emptyDic.Add(EXPLICIT_FILE_TYPE_KEY, fileType.GetXcodeDataValue());
                emptyDic.Add(INCLUDE_IN_INDEX_KEY, 0);
            }
            else
            {
                emptyDic.Add(LAST_KNOWN_FILE_TYPE_KEY, fileType.GetXcodeDataValue());
            }

            //source tree
            emptyDic.Add(SOURCE_TREE_KEY, sourceTree);
            //filename
            var fileName = System.IO.Path.GetFileName(unquotedPath);

            if (fileName != unquotedPath)
            {
                emptyDic.Add(NAME_KEY, fileName.ToLiteralIfRequired());
            }

            //TODO ignore file encoding for now
            var fileRef = new PBXFileReference(uid, emptyDic);
            fileRef.ParentGroup = group;
            return fileRef;
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            ParentGroup = FindParentGroup(allObjects);
        }

        #endregion


        public string Name
        {
            get
            {
                return Dict.StringValue(NAME_KEY).FromLiteral();
            }
        }

        public string Path
        {
            get
            {
                return Dict.StringValue(PATH_KEY).FromLiteral();
            }
        }

        public bool IsTarget
        {
            get
            {
                return Dict.ContainsKey(EXPLICIT_FILE_TYPE_KEY);
            }
        }

        public string LastKnownFileType
        {
            get
            {
                return (Dict.StringValue(LAST_KNOWN_FILE_TYPE_KEY));
            }
        }

        public string SourceTree
        {
            get
            {
                return Dict.StringValue(SOURCE_TREE_KEY);
            }
        }

        public string ExplicitFileType
        {
            get
            {
                return (Dict.StringValue(EXPLICIT_FILE_TYPE_KEY));
            }
        }

        public string FileEncoding
        {
            get
            {
                return Dict.StringValue(FILE_ENCODING_KEY);
            }
        }

        public string IncludeInIndex
        {
            get
            {
                return Dict.StringValue(INCLUDE_IN_INDEX_KEY);
            }
        }

        public string FullPath
        {
            get
            {
                if (SourceTree == SourceTreeLocation.GROUP)  
                //TODO || SourceTreeLocation.BUILT_PRODUCTS_DIR ?
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
            private set;
        }
    }
}
