// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//

namespace Egomotion.EgoXproject.Internal
{
    internal class StaticLibraryEntry : BaseFileEntry
    {
        const string LINK_TYPE_KEY = "LinkMethod";
        public const string TYPE = "StaticLibrary";

        public LinkType Link
        {
            get;
            set;
        }

        public StaticLibraryEntry(string path, AddMethod addMethod, LinkType linkType)
        : base(path, addMethod)
        {
            Link = linkType;
        }

        public StaticLibraryEntry(PListDictionary dic)
        : base(dic)
        {
            var lm = dic.StringValue(LINK_TYPE_KEY);
            Link = (LinkType) System.Enum.Parse(typeof(LinkType), lm);
        }

        public StaticLibraryEntry(StaticLibraryEntry other)
        : base(other)
        {
            Link = other.Link;
        }

        #region IChangeInfo implementation

        public override PListDictionary Serialize()
        {
            var dic = base.Serialize();
            dic.Add(LINK_TYPE_KEY, Link.ToString());
            return dic;
        }

        #endregion

        protected override string EntryType
        {
            get
            {
                return TYPE;
            }
        }

        public override BaseFileEntry Clone()
        {
            return new StaticLibraryEntry(this);
        }
    }
}
