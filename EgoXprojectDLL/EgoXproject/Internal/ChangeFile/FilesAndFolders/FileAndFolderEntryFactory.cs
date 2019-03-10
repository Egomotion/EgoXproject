// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;
using System.IO;
using UnityEngine;

namespace Egomotion.EgoXproject.Internal
{
    internal static class FileAndFolderEntryFactory
    {
        public static BaseFileEntry Create(string path, AddMethod addMethod)
        {
            var fileType = PBXFileTypeHelper.FileTypeFromFileName(path);

            if (!IsValidFileOrFolder(path))
            {
                return null;
            }

            if (PBXFileTypeHelper.IsSourceCodeFile(fileType))
            {
                return CreateSourceEntry(path, addMethod, "");
            }
            else if (PBXFileTypeHelper.IsFramework(fileType))
            {
                return CreateFrameworkEntry(path, addMethod, LinkType.Required, false);
            }
            else if (PBXFileTypeHelper.IsLibrary(fileType))
            {
                return CreateStaticLibraryEntry(path, addMethod, LinkType.Required);
            }
            else if (Directory.Exists(path) && !PBXFileTypeHelper.IsContainer(fileType))
            {
                return CreateFolderEntry(path, addMethod);
            }
            else
            {
                return CreateFileEntry(path, addMethod);
            }
        }

        public static SourceFileEntry CreateSourceEntry(string path, AddMethod addMethod, string attributes)
        {
            if (!IsValidFileOrFolder(path))
            {
                return null;
            }

            return new SourceFileEntry(path, addMethod, attributes);
        }

        public static FrameworkEntry CreateFrameworkEntry(string path, AddMethod addMethod, LinkType linkType, bool embedded)
        {
            if (!IsValidFileOrFolder(path))
            {
                return null;
            }

            return new FrameworkEntry(path, addMethod, linkType, embedded);
        }

        public static StaticLibraryEntry CreateStaticLibraryEntry(string path, AddMethod addMethod, LinkType linkType)
        {
            if (!IsValidFileOrFolder(path))
            {
                return null;
            }

            return new StaticLibraryEntry(path, addMethod, linkType);
        }

        public static FolderEntry CreateFolderEntry(string path, AddMethod addMethod)
        {
            if (!IsValidFileOrFolder(path))
            {
                return null;
            }

            var folderEntry =  new FolderEntry(path, addMethod);
            return folderEntry;
        }

        public static FileEntry CreateFileEntry(string path, AddMethod addMethod)
        {
            if (!IsValidFileOrFolder(path))
            {
                return null;
            }

            return new FileEntry(path, addMethod);
        }

        public static BaseFileEntry Create(PListDictionary dic)
        {
            var type = dic.StringValue(BaseFileEntry.TYPE_KEY);

            if (string.IsNullOrEmpty(type))
            {
                //TODO error? try and parse old version?
                Debug.LogError("EgoXproject: Corrupt entry, skipping: " + dic);
                return null;
            }

            switch (type)
            {
            case SourceFileEntry.TYPE:
                return CreateSourceEntry (dic);

            case FrameworkEntry.TYPE:
                return CreateFrameworkEntry (dic);

            case StaticLibraryEntry.TYPE:
                return CreateStaticLibraryEntry (dic);

            case FolderEntry.TYPE:
                return CreateFolderEntry (dic);

            case FileEntry.TYPE:
                return CreateFileEntry (dic);

            default:
                Debug.LogError ("EgoXproject: Unknown entry, skipping: " + dic);
                return null;
            }
        }

        //only used for upgrading from version 1 to 2
        public static BaseFileEntry CreateFromObsolete(PListDictionary dic)
        {
            string path = dic.StringValue("Path");
            var fileType = PBXFileTypeHelper.FileTypeFromFileName(path);

            if (PBXFileTypeHelper.IsSourceCodeFile(fileType))
            {
                var entry = CreateSourceEntry(dic);
                return entry;
            }
            else if (PBXFileTypeHelper.IsFramework(fileType))
            {
                return CreateFrameworkEntry(dic);
            }
            else if (PBXFileTypeHelper.IsLibrary(fileType))
            {
                return CreateStaticLibraryEntry(dic);
            }
            else if (PBXFileTypeHelper.IsContainer(fileType))
            {
                return CreateFileEntry(dic);
            }
            else if (dic.ContainsKey("Files") || dic.ContainsKey("Folders"))
            {
                return CreateFolderEntry(dic);
            }
            else
            {
                return CreateFileEntry(dic);
            }
        }

        public static SourceFileEntry CreateSourceEntry(PListDictionary dic)
        {
            try
            {
                return new SourceFileEntry(dic);
            }
            catch (System.Exception e)
            {
                Debug.LogError("EgoXproject: Could not load source file entry: " + e.Message);
                return null;
            }
        }

        public static FrameworkEntry CreateFrameworkEntry(PListDictionary dic)
        {
            try
            {
                return new FrameworkEntry(dic);
            }
            catch (System.Exception e)
            {
                Debug.LogError("EgoXproject: Could not load framework entry: " + e.Message);
                return null;
            }
        }

        public static StaticLibraryEntry CreateStaticLibraryEntry(PListDictionary dic)
        {
            try
            {
                return new StaticLibraryEntry(dic);
            }
            catch (System.Exception e)
            {
                Debug.LogError("EgoXproject: Could not load static library entry: " + e.Message);
                return null;
            }
        }

        public static FolderEntry CreateFolderEntry(PListDictionary dic)
        {
            try
            {
                return new FolderEntry(dic);
            }
            catch (System.Exception e)
            {
                Debug.LogError("EgoXproject: Could not load folder entry: " + e.Message);
                return null;
            }
        }

        public static FileEntry CreateFileEntry(PListDictionary dic)
        {
            try
            {
                return new FileEntry(dic);
            }
            catch (System.Exception e)
            {
                Debug.LogError("EgoXproject: Could not load file entry: " + e.Message);
                return null;
            }
        }

        public static bool IsValidFileOrFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (!Exists(path))
            {
                return false;
            }

            //skip ignored types
            if (IgnoredFiles.ShouldIgnore(path))
            {
                return false;
            }

            //Cannot ignore hidden files! Core Data models use them!
            return true;
        }

        public static bool Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }
    }
}
