// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class PlatformConfiguration
    {
        public const string DEFAULT_CONFIG_NAME = "Default";

        const string ACTIVE_CONFIGURATION_KEY = "ActiveConfiguration";
        const string CONFIGURATIONS_KEY = "Configurations";

        Dictionary<string, HashSet<string>> _configurations = new Dictionary<string, HashSet<string>>();
        string _activeConfiguration = DEFAULT_CONFIG_NAME;

        public PlatformConfiguration()
        {
        }

        public PlatformConfiguration(PListDictionary dic)
        {
            if (dic == null)
            {
                return;
            }

            var configs = dic.DictionaryValue(CONFIGURATIONS_KEY);

            if (configs != null && configs.Count > 0)
            {
                foreach (var kvp in configs)
                {
                    var entries = kvp.Value as PListArray;

                    if (entries == null || entries.Count <= 0)
                    {
                        AddConfiguration(kvp.Key);
                        continue;
                    }

                    for (int ii = 0; ii < entries.Count; ++ii)
                    {
                        AddChangeFileToConfiguration(entries.StringValue(ii), kvp.Key);
                    }
                }
            }

            ActiveConfiguration = dic.StringValue(ACTIVE_CONFIGURATION_KEY);
            IsDirty = false;
        }

        public string[] Configurations
        {
            get
            {
                var list = new List<string>(_configurations.Keys);
                list.Insert(0, DEFAULT_CONFIG_NAME);
                return list.ToArray();
            }
        }

        public void AddConfiguration(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (!IsValidConfigurationName(name))
            {
                return;
            }

            _configurations.Add(name, new HashSet<string>());
            IsDirty = true;
        }

        public void RemoveConfiguration(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            _configurations.Remove(name);

            if (ActiveConfiguration == name)
            {
                ActiveConfiguration = DEFAULT_CONFIG_NAME;
            }

            IsDirty = true;
        }

        public void RenameConfiguration(string currentName, string newName)
        {
            if (string.IsNullOrEmpty(currentName) || !_configurations.ContainsKey(currentName))
            {
                return;
            }

            if (!IsValidConfigurationName(newName))
            {
                return;
            }

            var hs = _configurations[currentName];
            _configurations.Remove(currentName);
            _configurations.Add(newName, hs);

            if (ActiveConfiguration == currentName)
            {
                ActiveConfiguration = newName;
            }

            IsDirty = true;
        }

        public bool IsValidConfigurationName(string name)
        {
            name = name.Trim();

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (name == DEFAULT_CONFIG_NAME)
            {
                return false;
            }

            if (_configurations.ContainsKey(name))
            {
                return false;
            }

            return true;
        }

        public void AddChangeFileToConfiguration(string changeFile, string configuration)
        {
            if (string.IsNullOrEmpty(changeFile) || string.IsNullOrEmpty(configuration))
            {
                return;
            }

            if (!_configurations.ContainsKey(configuration))
            {
                AddConfiguration(configuration);

                if (!_configurations.ContainsKey(configuration))
                {
                    return;
                }
            }

            _configurations[configuration].Add(changeFile);
            IsDirty = true;
        }

        public void RemoveChangeFileFromConfiguration(string changeFile, string configuration)
        {
            if (string.IsNullOrEmpty(changeFile) || string.IsNullOrEmpty(configuration))
            {
                return;
            }

            if (!_configurations.ContainsKey(configuration))
            {
                return;
            }

            _configurations[configuration].Remove(changeFile);
            IsDirty = true;
        }

        public string[] ChangeFilesInConfiguration(string configuration)
        {
            if (string.IsNullOrEmpty(configuration))
            {
                return null;
            }

            HashSet<string> configs = null;
            _configurations.TryGetValue(configuration, out configs);

            if (configs != null)
            {
                return configs.ToArray();
            }
            else
            {
                return null;
            }
        }

        public string ActiveConfiguration
        {
            get
            {
                return _activeConfiguration;
            }
            set
            {
                if (_configurations.ContainsKey(value))
                {
                    _activeConfiguration = value;
                }
                else
                {
                    _activeConfiguration = DEFAULT_CONFIG_NAME;
                }

                IsDirty = true;
            }
        }

        public void Refresh(string[] allChangeFiles)
        {
            HashSet<string> all = new HashSet<string>(allChangeFiles);

            foreach (var kvp in _configurations)
            {
                var toRemove = new List<string>();

                foreach (var entry in kvp.Value)
                {
                    if (!all.Contains(entry))
                    {
                        toRemove.Add(entry);
                    }
                }

                foreach (var r in toRemove)
                {
                    kvp.Value.Remove(r);
                }
            }
        }

        public bool IsDirty { get; set; }

        public PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.Add(ACTIVE_CONFIGURATION_KEY, _activeConfiguration);
            var configs = new PListDictionary();

            foreach (var kvp in _configurations)
            {
                var entries = new PListArray();

                foreach (var e in kvp.Value)
                {
                    entries.Add(e);
                }

                configs.Add(kvp.Key, entries);
            }

            dic.Add(CONFIGURATIONS_KEY, configs);
            return dic;
        }
    }
}

