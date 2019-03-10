// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class ScriptChanges : BaseChangeGroup
    {
        List<ScriptEntry> _scripts = new List<ScriptEntry>();

        public ScriptChanges()
        {
        }

        public ScriptChanges(PListArray array)
        {
            if (array == null)
            {
                return;
            }

            for (int ii = 0; ii < array.Count; ++ii)
            {
                var entry = array.DictionaryValue(ii);

                if (entry != null)
                {
                    _scripts.Add(new ScriptEntry(entry));
                }
            }
        }

        #region implemented abstract members of ChangeGroup

        public override IPListElement Serialize()
        {
            return Serialize(_scripts);
        }

        public override bool HasChanges()
        {
            return _scripts.Count > 0;
        }

        #endregion

        public void Merge(ScriptChanges other)
        {
            foreach (var s in other._scripts)
            {
                if (string.IsNullOrEmpty(s.Script.Trim()))
                {
                    continue;
                }

                ScriptEntry existing = null;

                //only match scripts that have names
                if (!string.IsNullOrEmpty(s.Name))
                {
                    existing = _scripts.Find(o => o.Name == s.Name);
                }

                if (existing == null)
                {
                    var entry = new ScriptEntry(s);
                    _scripts.Add(entry);
                }
                else
                {
                    existing.Script = s.Script;
                    existing.Shell = s.Script;
                }
            }
        }

        public void AddScript()
        {
            _scripts.Add(new ScriptEntry());
        }

        public void AddScript(string script)
        {
            _scripts.Add(new ScriptEntry(script));
        }

        public void AddScript(string script, string shell)
        {
            _scripts.Add(new ScriptEntry(script, shell));
        }

        public void AddScript(string script, string shell, string name)
        {
            _scripts.Add(new ScriptEntry(script, shell, name));
        }

        public void RemoveAt(int index)
        {
            _scripts.RemoveAt(index);
        }

        public override void Clear()
        {
            _scripts.Clear();
        }

        public int Count
        {
            get
            {
                return _scripts.Count;
            }
        }

        public string ScriptAt(int index)
        {
            return _scripts[index].Script;
        }

        public void SetScriptAt(int index, string script)
        {
            _scripts[index].Script = script;
        }

        public string ShellAt(int index)
        {
            return _scripts[index].Shell;
        }

        public void SetShellAt(int index, string shell)
        {
            if (string.IsNullOrEmpty(shell))
            {
                shell = ScriptEntry.DEFAULT_SHELL;
            }

            _scripts[index].Shell = shell;
        }

        public string NameAt(int index)
        {
            return _scripts[index].Name;
        }

        public void SetNameAt(int index, string name)
        {
            _scripts[index].Name = name;
        }
    }
}

