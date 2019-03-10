// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

namespace Egomotion.EgoXproject.Internal
{
    internal class FrameworkEntry : BaseFileEntry
    {
        const string LINK_TYPE_KEY = "LinkMethod";
        const string EMBEDDED_KEY = "Embedded";
        public const string TYPE = "Framework";

        public LinkType Link
        {
            get;
            set;
        }

        public bool Embedded
        {
            get;
            set;
        }

        public FrameworkEntry(string path, AddMethod addMethod, LinkType linkType, bool embedded = false)
        : base(path, addMethod)
        {
            Link = linkType;
            Embedded = embedded;
        }

        public FrameworkEntry(PListDictionary dic)
        : base(dic)
        {
            var lm = dic.StringValue(LINK_TYPE_KEY);
            Link = (LinkType) System.Enum.Parse(typeof(LinkType), lm);
            Embedded = dic.BoolValue(EMBEDDED_KEY);
        }

        public FrameworkEntry(FrameworkEntry other)
        : base(other)
        {
            Link = other.Link;
            Embedded = other.Embedded;
        }

        #region IChangeInfo implementation

        public override PListDictionary Serialize()
        {
            var dic = base.Serialize();
            dic.Add(LINK_TYPE_KEY, Link.ToString());

            if (Embedded)
            {
                dic.Add(EMBEDDED_KEY, Embedded);
            }

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
            return new FrameworkEntry(this);
        }
    }
}
