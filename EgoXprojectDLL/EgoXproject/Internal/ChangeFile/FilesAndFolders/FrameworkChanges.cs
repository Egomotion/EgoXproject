// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class FrameworkChanges : BaseChangeGroup
    {
        static string SYSTEM_FRAMEWORKS_KEY = "System";

        List<SystemFrameworkEntry> _frameworks = new List<SystemFrameworkEntry>();

        public FrameworkChanges()
        {
        }

        public FrameworkChanges(PListDictionary dic)
        {
            if (dic == null)
            {
                return;
            }

            var sys = dic.ArrayValue(SYSTEM_FRAMEWORKS_KEY);

            if (sys != null)
            {
                Load(sys, _frameworks);
            }
        }

        void Load(PListArray array, List<SystemFrameworkEntry> list)
        {
            for (int ii = 0; ii < array.Count; ++ii)
            {
                var dic = array.DictionaryValue(ii);

                if (dic == null)
                {
                    continue;
                }

                var entry = new SystemFrameworkEntry(dic);
                list.Add(entry);
            }
        }

        public void Add(string name, LinkType linkType = LinkType.Required)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (_frameworks.FindIndex(o => o.FileName == name) >= 0)
            {
                return;
            }

            _frameworks.Add(new SystemFrameworkEntry(name, linkType));
        }

        bool IsFrameworkOrLibrary(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            var type = PBXFileTypeHelper.FileTypeFromFileName(path);

            if (!PBXFileTypeHelper.IsFrameworkOrLibrary(type))
            {
                return false;
            }

            return true;
        }

        public void RemoveAt(int index)
        {
            _frameworks.RemoveAt(index);
        }

        public int Count
        {
            get
            {
                return _frameworks.Count;
            }
        }

        public string[] Names
        {
            get
            {
                return _frameworks.Select(o => o.FileName).ToArray();
            }
        }

        public string FileNameAt(int index)
        {
            return _frameworks[index].FileName;
        }

        public LinkType LinkTypeAt(int index)
        {
            return _frameworks[index].Link;
        }

        public void SetLinkTypeAt(int index, LinkType linkType)
        {
            _frameworks[index].Link = linkType;
        }

        public override void Clear()
        {
            _frameworks.Clear();
        }

        #region implemented abstract members of ChangeGroup

        public override IPListElement Serialize()
        {
            var dic = new PListDictionary();
            dic.Add(SYSTEM_FRAMEWORKS_KEY, Serialize(_frameworks));
            return dic;
        }

        public override bool HasChanges()
        {
            return Count > 0;
        }

        #endregion

        public void Merge(FrameworkChanges other)
        {
            foreach (var otherSys in other._frameworks)
            {
                if (_frameworks.FindIndex(o => o.FileName == otherSys.FileName) < 0)
                {
                    _frameworks.Add(otherSys.Clone());
                }
            }
        }
    }
}

