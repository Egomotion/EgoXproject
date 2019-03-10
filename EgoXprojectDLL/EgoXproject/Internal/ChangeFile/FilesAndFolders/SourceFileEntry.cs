// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//
using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class SourceFileEntry : BaseFileEntry
    {
        const string COMPILER_FLAGS_KEY = "CompilerFlags";
        public const string TYPE = "Source";

        public SourceFileEntry(string path, AddMethod addMethod, string attributes)
        : base (path, addMethod)
        {
            CompilerFlags = attributes;
        }

        public SourceFileEntry(PListDictionary dic)
        : base(dic)
        {
            CompilerFlags = dic.StringValue(COMPILER_FLAGS_KEY);

            //check for old key name
            if (string.IsNullOrEmpty(CompilerFlags))
            {
                CompilerFlags = dic.StringValue("Attributes");
            }
        }

        public SourceFileEntry(SourceFileEntry other)
        : base(other)
        {
            CompilerFlags = other.CompilerFlags;
        }

        public string CompilerFlags
        {
            get;
            set;
        }

        public override PListDictionary Serialize()
        {
            var dic = base.Serialize();

            if (!string.IsNullOrEmpty(CompilerFlags))
            {
                dic.Add(COMPILER_FLAGS_KEY, CompilerFlags);
            }

            return dic;
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
            return new SourceFileEntry(this);
        }

    }
}

