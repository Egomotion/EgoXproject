// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

namespace Egomotion.EgoXproject.Internal
{
    internal class ScriptEntry : BaseChangeEntry
    {
        public const string DEFAULT_SHELL = "/bin/sh";
        const string NAME_KEY = "Name";
        const string SHELL_KEY = "Shell";
        const string SCRIPT_KEY = "Script";

        public string Name
        {
            get;
            set;
        }

        public string Shell
        {
            get;
            set;
        }

        public string Script
        {
            get;
            set;
        }

        public ScriptEntry()
        {
            Shell = DEFAULT_SHELL;
        }

        public ScriptEntry(string script)
        : this()
        {
            Script = script;
        }

        public ScriptEntry(string script, string shell)
        : this(script)
        {
            Shell = shell;
        }

        public ScriptEntry(string script, string shell, string name)
        : this(script, shell)
        {
            Name = name;
        }

        public ScriptEntry(PListDictionary dic)
        {
            if (dic == null)
            {
                throw new System.ArgumentNullException(nameof (dic), "Dictionary cannot be null");
            }

            Name = dic.StringValue(NAME_KEY);
            Shell = dic.StringValue(SHELL_KEY);

            if (string.IsNullOrEmpty(Shell))
            {
                Shell = DEFAULT_SHELL;
            }

            Script = dic.StringValue(SCRIPT_KEY);
        }

        public ScriptEntry(ScriptEntry other)
        {
            Name = other.Name;
            Shell = other.Shell;
            Script = other.Script;
        }

        #region IChangeInfo implementation

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.Add(NAME_KEY, Name);
            dic.Add(SHELL_KEY, Shell);
            dic.Add(SCRIPT_KEY, Script);
            return dic;
        }

        #endregion
    }
}

