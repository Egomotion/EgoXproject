// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

namespace Egomotion.EgoXproject.Internal
{
    internal class FilesAndFolderChanges : BaseChangeGroup
    {
        const string ENTRIES_KEY = "Entries";

        List<BaseFileEntry> _entries = new List<BaseFileEntry>();

        public FilesAndFolderChanges()
        {
        }

        public FilesAndFolderChanges(PListDictionary dic)
        {
            if (dic == null)
            {
                return;
            }

            var entries = dic.ArrayValue(ENTRIES_KEY);

            if (entries == null)
            {
                return;
            }

            for (int ii = 0; ii < entries.Count; ++ii)
            {
                var entryDic = entries.DictionaryValue(ii);

                if (entryDic == null)
                {
                    continue;
                }

                var entry = FileAndFolderEntryFactory.Create(entryDic);

                if (entry != null )
                {
                    _entries.Add(entry);
                }
            }
        }

        #region implemented abstract members of ChangeGroup

        public override IPListElement Serialize()
        {
            var dic = new PListDictionary();
            dic.Add(ENTRIES_KEY, Serialize(_entries));
            return dic;
        }

        public override bool HasChanges()
        {
            return _entries.Count > 0;
        }

        #endregion

        public void Merge(FilesAndFolderChanges other)
        {
            foreach (var entry in other._entries)
            {
                if (_entries.FindIndex(o => o.Path == entry.Path) < 0)
                {
                    _entries.Add(entry.Clone());
                }
            }
        }

        public override void Clear()
        {
            _entries.Clear();
        }

        public string[] EntryNames
        {
            get
            {
                return _entries.Select(o => o.FileName).ToArray();
            }
        }

        public BaseFileEntry[] Entries
        {
            get
            {
                return _entries.ToArray();
            }
        }

        public int EntryCount
        {
            get
            {
                return _entries.Count;
            }
        }

        void AddEntry(string path, AddMethod addMethod = AddMethod.Link)
        {
            path = ProjectUtil.MakePathRelativeToProject(path);

            if (_entries.FindIndex(o => o.Path == path) > -1)
            {
                return;
            }

            var entry = FileAndFolderEntryFactory.Create(path, addMethod);

            if (entry != null)
            {
                _entries.Add(entry);
            }
        }

        public void RemoveEntryAt(int index)
        {
            _entries.RemoveAt(index);
        }

        public void AddSourceFile(string path, AddMethod addMethod, string compilerFlags = "")
        {
            path = ProjectUtil.MakePathRelativeToProject(path);

            if (_entries.FindIndex(o => o.Path == path) < 0)
            {
                BaseFileEntry entry = null;
                var fileType = PBXFileTypeHelper.FileTypeFromFileName(path);

                if (PBXFileTypeHelper.IsSourceCodeFile(fileType))
                {
                    entry = FileAndFolderEntryFactory.CreateSourceEntry(path, addMethod, compilerFlags);
                }
                else
                {
                    Debug.LogWarning("EgoXproject: File is not a known source file type. Adding as a regular file: " + path);
                    entry = FileAndFolderEntryFactory.Create(path, addMethod);
                }

                if (entry == null)
                {
                    Debug.LogError("EgoXproject: Could not add file. Either it does not exist or is on Ignore List: " + path);
                }
                else
                {
                    _entries.Add(entry);
                }
            }
        }

        public void AddFrameworkOrLibrary(string path, AddMethod addMethod, LinkType linkType)
        {
            path = ProjectUtil.MakePathRelativeToProject(path);

            if (_entries.FindIndex(o => o.Path == path) < 0)
            {
                BaseFileEntry entry = null;
                var fileType = PBXFileTypeHelper.FileTypeFromFileName(path);

                if (PBXFileTypeHelper.IsFrameworkOrLibrary(fileType))
                {
                    entry = FileAndFolderEntryFactory.CreateFrameworkEntry(path, addMethod, linkType, false);
                }
                else
                {
                    Debug.LogWarning("EgoXproject: File is not a known Framework or library file type. Adding as a regular file: " + path);
                    entry = FileAndFolderEntryFactory.Create(path, addMethod);
                }

                if (entry == null)
                {
                    Debug.LogError("EgoXproject: Could not add file. Either it does not exist or is on Ignore List: " + path);
                }
                else
                {
                    _entries.Add(entry);
                }
            }
        }

        public void AddEmbeddedFramework(string path)
        {
            path = ProjectUtil.MakePathRelativeToProject(path);

            if (_entries.FindIndex(o => o.Path == path) < 0)
            {
                BaseFileEntry entry = null;
                var fileType = PBXFileTypeHelper.FileTypeFromFileName(path);

                if (PBXFileTypeHelper.IsFramework(fileType))
                {
                    entry = FileAndFolderEntryFactory.CreateFrameworkEntry(path, AddMethod.Copy, LinkType.Required, true);

                    if (entry == null)
                    {
                        Debug.LogError("EgoXproject: Could not add file. Either it does not exist or is on Ignore List: " + path);
                    }
                    else
                    {
                        _entries.Add(entry);
                    }
                }
                else
                {
                    Debug.LogWarning("EgoXproject: File is not a Framework type. Skipping: " + path);
                }
            }
        }

        public void AddFileOrFolder(string path, AddMethod addMethod = AddMethod.Link)
        {
            path = ProjectUtil.MakePathRelativeToProject(path);

            if (_entries.FindIndex(o => o.Path == path) < 0)
            {
                var entry = FileAndFolderEntryFactory.Create(path, addMethod);

                if (entry == null)
                {
                    Debug.LogError("EgoXproject: Could not add file. Either does not exist or is on Ignore List: " + path);
                }
                else
                {
                    _entries.Add(entry);
                }
            }
        }

        //Upgrader use only
        public void Upgrader_AddEntry(BaseFileEntry entry)
        {
            if (entry == null)
            {
                return;
            }

            if (_entries.FindIndex(o => o.Path == entry.Path) < 0)
            {
                _entries.Add(entry);
            }
        }
    }
}

