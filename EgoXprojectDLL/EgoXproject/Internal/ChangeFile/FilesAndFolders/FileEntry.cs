// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

namespace Egomotion.EgoXproject.Internal
{
    internal class FileEntry : BaseFileEntry
    {
        public const string TYPE = "File";

        public FileEntry(string path, AddMethod addMethod)
        : base(path, addMethod)
        {
        }

        public FileEntry(PListDictionary dic)
        : base(dic)
        {
        }

        public FileEntry(FileEntry file)
        : base(file)
        {
        }

        #region IChangeInfo implementation

        public override PListDictionary Serialize()
        {
            var dic = base.Serialize();
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
            return new FileEntry(this);
        }
    }
}
