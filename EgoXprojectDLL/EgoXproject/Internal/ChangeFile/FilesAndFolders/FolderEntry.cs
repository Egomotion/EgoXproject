// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Egomotion.EgoXproject.Internal
{
    internal class FolderEntry : BaseFileEntry
    {
        public const string TYPE = "Folder";
        const string ENTRIES_KEY = "Entries";

        List<BaseFileEntry> _entries = new List<BaseFileEntry>();

        public FolderEntry(string path, AddMethod addMethod)
        : base(path, addMethod)
        {
            Populate();
        }

        public FolderEntry(PListDictionary dic)
        : base(dic)
        {
            var entryDics = dic.ArrayValue(ENTRIES_KEY);

            if (entryDics != null)
            {
                foreach (var entryDic in entryDics)
                {
                    var entry = FileAndFolderEntryFactory.Create(entryDic as PListDictionary);
                    AddEntry(entry);
                }
            }
            else
            {
                //Try and load old keys
                var files = dic.ArrayValue("Files");

                if (files != null)
                {
                    for (int ii = 0; ii < files.Count; ii++)
                    {
                        var entry = FileAndFolderEntryFactory.CreateFromObsolete(files.DictionaryValue(ii));

                        if (entry != null)
                        {
                            AddEntry(entry);
                        }
                    }
                }

                var folders = dic.ArrayValue("Folders");

                if (folders != null)
                {
                    for (int ii = 0; ii < folders.Count; ii++)
                    {
                        var entry = FileAndFolderEntryFactory.CreateFromObsolete(folders.DictionaryValue(ii));

                        if (entry != null)
                        {
                            AddEntry(entry);
                        }
                    }
                }
            }
        }

        public FolderEntry(FolderEntry other)
        : base(other)
        {
            foreach (var entry in other._entries)
            {
                _entries.Add(entry.Clone());
            }
        }

        #region implemented abstract members of BaseChangeEntry

        public override PListDictionary Serialize()
        {
            var dic = base.Serialize();

            if (_entries.Count > 0)
            {
                var entriesArray = new PListArray();

                foreach (var entry in _entries)
                {
                    entriesArray.Add(entry.Serialize());
                }

                dic.Add(ENTRIES_KEY, entriesArray);
            }

            return dic;
        }

        #endregion

        public string[] Names
        {
            get
            {
                return _entries.Select(o => o.FileName).ToArray();
            }
        }

        public override AddMethod Add
        {
            get
            {
                return base.Add;
            }
            set
            {
                base.Add = value;

                foreach (var entry in _entries)
                {
                    entry.Add = Add;
                }
            }
        }

        protected override string EntryType
        {
            get
            {
                return TYPE;
            }
        }

        public override BaseFileEntry Clone()
        {
            return new FolderEntry(this);
        }

        public BaseFileEntry[] Entries
        {
            get
            {
                return _entries.ToArray();
            }
        }

        void AddEntry(BaseFileEntry entry)
        {
            if (entry == null)
            {
                return;
            }

            if (_entries.FindIndex(o => o.Path == entry.Path) >= 0)
            {
                return;
            }

            entry.Add = Add;
            _entries.Add(entry);
        }

        public void Remove(BaseFileEntry entry)
        {
            _entries.Remove(entry);
        }

        public void RemoveAt(int index)
        {
            _entries.RemoveAt(index);
        }

        public int Count
        {
            get
            {
                return _entries.Count;
            }
        }

        public void RemoveMissingEntries()
        {
            List<BaseFileEntry> removeFiles = new List<BaseFileEntry>();

            foreach (var e in _entries)
            {
                if (!FileAndFolderEntryFactory.Exists(Path))
                {
                    removeFiles.Add(e);
                }
                else if (e is FolderEntry)
                {
                    var fe = e as FolderEntry;
                    fe.RemoveMissingEntries();
                }
            }

            foreach (var r in removeFiles)
            {
                _entries.Remove(r);
            }
        }

        public void RefreshFolder()
        {
            Populate();
            RemoveMissingEntries();
        }

        void Populate()
        {
            bool populated = _entries.Count > 0;
            var files = Directory.GetFiles(Path);

            foreach (var f in files)
            {
                if (!FileAndFolderEntryFactory.IsValidFileOrFolder(f))
                {
                    continue;
                }

                var rel = ProjectUtil.MakePathRelativeToProject(f);
                AddEntry(FileAndFolderEntryFactory.Create(rel, Add));
            }

            var folders = Directory.GetDirectories(Path);

            foreach (var f in folders)
            {
                if (!FileAndFolderEntryFactory.IsValidFileOrFolder(f))
                {
                    continue;
                }

                var rel = ProjectUtil.MakePathRelativeToProject(f);
                int index = -1;

                if (populated)
                {
                    index = _entries.FindIndex(o => o.Path == rel);
                }

                if (index < 0)
                {
                    AddEntry(FileAndFolderEntryFactory.Create(rel, Add));
                }
                else if (_entries[index] is FolderEntry)
                {
                    var subfe = _entries[index] as FolderEntry;
                    subfe.Populate();
                }
            }
        }
    }
}
