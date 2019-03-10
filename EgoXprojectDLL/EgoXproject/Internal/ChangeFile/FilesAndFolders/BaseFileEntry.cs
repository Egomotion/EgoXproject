// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------


namespace Egomotion.EgoXproject.Internal
{
    internal abstract class BaseFileEntry : BaseChangeEntry
    {
        public const string PATH_KEY = "Path";
        const string ADD_METHOD_KEY = "AddMethod";
        public const string TYPE_KEY = "Type";

        public string Path
        {
            get;
            private set;
        }

        public virtual AddMethod Add
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            private set;
        }

        protected BaseFileEntry(string path, AddMethod addMethod)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new System.ArgumentNullException(nameof (path), "Path cannot be null");
            }

            Path = path;
            Add = addMethod;
            SetFileName();
        }

        protected BaseFileEntry(PListDictionary dic)
        {
            if (dic == null)
            {
                throw new System.ArgumentNullException(nameof (dic), "Dictionary cannot be null");
            }

            Path = dic.StringValue(PATH_KEY);

            if (string.IsNullOrEmpty(Path))
            {
                throw new System.ArgumentException("No Path entry in dictionary");
            }

            var am = dic.StringValue(ADD_METHOD_KEY);
            Add = (AddMethod) System.Enum.Parse(typeof(AddMethod), am);
            SetFileName();
        }

        protected BaseFileEntry(BaseFileEntry other)
        {
            if (other == null)
            {
                throw new System.ArgumentNullException(nameof (other), "BaseFileEntry cannot be null");
            }

            Path = other.Path;
            Add = other.Add;
            SetFileName();
        }

        #region implemented abstract members of BaseChangeEntry

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.Add(PATH_KEY, Path);
            dic.Add(TYPE_KEY, EntryType);
            dic.Add(ADD_METHOD_KEY, Add.ToString());
            return dic;
        }

        #endregion

        void SetFileName()
        {
            FileName = System.IO.Path.GetFileName(Path);
        }

        protected abstract string EntryType
        {
            get;
        }

        public abstract BaseFileEntry Clone();
    }
}
