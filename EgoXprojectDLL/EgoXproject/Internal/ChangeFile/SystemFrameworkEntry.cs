// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

namespace Egomotion.EgoXproject.Internal
{
    internal class SystemFrameworkEntry : BaseChangeEntry
    {
        const string NAME_KEY = "Name";
        const string OLD_PATH_KEY = "Path";
        const string LINK_TYPE_KEY = "LinkMethod";

        public SystemFrameworkEntry(string name, LinkType linkType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentNullException(nameof (name), "Name cannot be null or empty");
            }

            FileName = name;
            Link = linkType;
        }

        public SystemFrameworkEntry(PListDictionary dic)
        {
            FileName = dic.StringValue(NAME_KEY);

            //Fallback to old key name
            if (string.IsNullOrEmpty(FileName))
            {
                FileName = dic.StringValue(OLD_PATH_KEY);
            }

            if (string.IsNullOrEmpty(FileName))
            {
                throw new System.ArgumentException("No File name entry in dictionary");
            }

            var lm = dic.StringValue(LINK_TYPE_KEY);
            Link = (LinkType) System.Enum.Parse(typeof(LinkType), lm);
        }

        public SystemFrameworkEntry(SystemFrameworkEntry other)
        {
            if (other == null)
            {
                throw new System.ArgumentNullException(nameof (other), "SystemFrameworkEntry cannot be null");
            }

            FileName = other.FileName;
            Link = other.Link;
        }

        public string FileName
        {
            get;
            private set;
        }

        public LinkType Link
        {
            get;
            set;
        }

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.Add(NAME_KEY, FileName);
            dic.Add(LINK_TYPE_KEY, Link.ToString());
            return dic;
        }

        public SystemFrameworkEntry Clone()
        {
            return new SystemFrameworkEntry(this);
        }
    }
}

