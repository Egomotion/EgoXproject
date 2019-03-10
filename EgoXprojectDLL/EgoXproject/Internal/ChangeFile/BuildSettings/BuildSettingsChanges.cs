// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class BuildSettingsChanges : BaseChangeGroup
    {
        List<BaseBuildSettingEntry> _buildSettings = new List<BaseBuildSettingEntry>();

        XcodeBuildSettings _reference = XcodeBuildSettings.Instance();

        public BuildSettingsChanges()
        {
        }

        public BuildSettingsChanges(PListArray array)
        {
            if (array == null)
            {
                return;
            }

            for (int ii = 0; ii < array.Count; ++ii)
            {
                var dic = array.DictionaryValue(ii);

                if (dic == null)
                {
                    continue;
                }

                var name = dic.StringValue(BaseBuildSettingEntry.NAME_KEY);
                BaseBuildSetting refSetting;

                if (_reference.BuildSetting(name, out refSetting))
                {
                    if (refSetting is BoolBuildSetting)
                    {
                        _buildSettings.Add(new BoolBuildSettingEntry(dic));
                    }
                    else if (refSetting is ArrayBuildSetting || refSetting is StringListBuildSetting)
                    {
                        _buildSettings.Add(new CollectionBuildSettingEntry(dic));
                    }
                    else if (refSetting is EnumBuildSetting)
                    {
                        _buildSettings.Add(new EnumBuildSettingEntry(dic));
                    }
                    else if (refSetting is StringBuildSetting)
                    {
                        _buildSettings.Add(new StringBuildSettingEntry(dic));
                    }
                    else
                    {
                        throw new System.NotImplementedException("EgoXproject: Developer has forgotten to implement check for new build setting type.");
                    }
                }
                else
                {
                    _buildSettings.Add(new CustomStringBuildSettingEntry(dic));
                }
            }
        }

        #region implemented abstract members of BaseChangeGroup

        public override IPListElement Serialize()
        {
            var array = new PListArray();

            foreach (var entry in _buildSettings)
            {
                //don't save empty entries
                if (entry is CustomStringBuildSettingEntry)
                {
                    var custom = entry as CustomStringBuildSettingEntry;

                    if (string.IsNullOrEmpty(custom.Name) && string.IsNullOrEmpty(custom.Value))
                    {
                        continue;
                    }
                }

                array.Add(entry.Serialize());
            }

            return array;
        }

        public override bool HasChanges()
        {
            return _buildSettings.Count > 0;
        }

        public override void Clear()
        {
            _buildSettings.Clear();
        }

        #endregion

        public void Merge(BuildSettingsChanges other)
        {
            foreach (var otherEntry in other._buildSettings)
            {
                var entry = _buildSettings.Find(o => o.Name == otherEntry.Name);

                //new entry, so add
                if (entry == null)
                {
                    if (otherEntry is BoolBuildSettingEntry)
                    {
                        _buildSettings.Add(new BoolBuildSettingEntry(otherEntry as BoolBuildSettingEntry));
                    }
                    else if (otherEntry is EnumBuildSettingEntry)
                    {
                        _buildSettings.Add(new EnumBuildSettingEntry(otherEntry as EnumBuildSettingEntry));
                    }
                    else if (otherEntry is CollectionBuildSettingEntry)
                    {
                        var otherArray = otherEntry as CollectionBuildSettingEntry;

                        if (otherArray.Values.Count <= 0)
                        {
                            continue;
                        }

                        var array = new CollectionBuildSettingEntry(otherArray);

                        if (array.Values.Count <= 0)
                        {
                            continue;
                        }

                        _buildSettings.Add(array);
                    }
                    else if (otherEntry is CustomStringBuildSettingEntry)
                    {
                        var otherStr = otherEntry as CustomStringBuildSettingEntry;

                        if (string.IsNullOrEmpty(otherStr.Value.Trim()))
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(otherStr.Name.Trim()))
                        {
                            continue;
                        }

                        var str = new CustomStringBuildSettingEntry(otherStr);
                        _buildSettings.Add(str);
                    }
                    else if (otherEntry is StringBuildSettingEntry)
                    {
                        var otherStr = otherEntry as StringBuildSettingEntry;

                        if (string.IsNullOrEmpty(otherStr.Value.Trim()))
                        {
                            continue;
                        }

                        var str = new StringBuildSettingEntry(otherStr);
                        _buildSettings.Add(str);
                    }
                }
                //existing entry, so selective merge
                else
                {
                    if (otherEntry is CollectionBuildSettingEntry)
                    {
                        var a = entry as CollectionBuildSettingEntry;
                        var b = otherEntry as CollectionBuildSettingEntry;

                        foreach (var otherValue in b.Values)
                        {
                            if (string.IsNullOrEmpty(otherValue))
                            {
                                continue;
                            }

                            if (!a.Values.Contains(otherValue))
                            {
                                a.Values.Add(otherValue);
                            }
                        }
                    }
                    else if (otherEntry is CustomStringBuildSettingEntry)
                    {
                        if (string.IsNullOrEmpty(entry.Name.Trim()))
                        {
                            continue;
                        }

                        var a = entry as CustomStringBuildSettingEntry;

                        if (string.IsNullOrEmpty(a.Value.Trim()))
                        {
                            var b = otherEntry as CustomStringBuildSettingEntry;
                            a.Value = b.Value;
                        }
                    }
                    else if (otherEntry is StringBuildSettingEntry && entry is StringBuildSettingEntry)
                    {
                        var a = entry as StringBuildSettingEntry;

                        if (string.IsNullOrEmpty(a.Value.Trim()))
                        {
                            var b = otherEntry as StringBuildSettingEntry;
                            a.Value = b.Value;
                        }
                    }
                }
            }
        }

        //used by ui
        public void Add(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            //no dupes
            if (_buildSettings.FindIndex(o => o.Name == name) > -1)
            {
                return;
            }

            BaseBuildSettingEntry newEntry = CreateBuildSettingEntry(name);
            _buildSettings.Add(newEntry);
        }

        public void EditCustomBuildSetting(string currentName, string newName)
        {
            if (string.IsNullOrEmpty(currentName))
            {
                return;
            }

            //current must exist
            int index = _buildSettings.FindIndex(o => o.Name == currentName);

            if (index < 0)
            {
                return;
            }

            //must be custom
            var entry = _buildSettings[index] as CustomStringBuildSettingEntry;

            if (entry == null)
            {
                return;
            }

            //remove if new name is empty
            if (string.IsNullOrEmpty(newName))
            {
                RemoveAt(index);
                return;
            }

            //new must not exist
            if (_buildSettings.FindIndex(o => o.Name == newName) > -1)
            {
                return;
            }

            //remove old setting
            _buildSettings.RemoveAt(index);
            //add in new setting
            BaseBuildSettingEntry newEntry = CreateBuildSettingEntry(newName);
            _buildSettings.Insert(index, newEntry);
        }

        BaseBuildSettingEntry CreateBuildSettingEntry(string settingName)
        {
            BaseBuildSettingEntry newEntry;
            BaseBuildSetting refSetting;

            if (_reference.BuildSetting(settingName, out refSetting))
            {
                if (refSetting is BoolBuildSetting)
                {
                    var boolSetting = refSetting as BoolBuildSetting;
                    newEntry = new BoolBuildSettingEntry(settingName, boolSetting.Value);
                }
                else if (refSetting is ArrayBuildSetting || refSetting is StringListBuildSetting)
                {
                    newEntry = new CollectionBuildSettingEntry(settingName);
                }
                else if (refSetting is EnumBuildSetting)
                {
                    newEntry = new EnumBuildSettingEntry(settingName);
                }
                else if (refSetting is StringBuildSetting)
                {
                    var stringSetting = refSetting as StringBuildSetting;
                    newEntry = new StringBuildSettingEntry(settingName, stringSetting.Value);
                }
                else
                {
                    throw new System.NotImplementedException("EgoXproject: Developer has forgotten to implement check for new build setting type.");
                }
            }
            else
            {
                newEntry = new CustomStringBuildSettingEntry(settingName);
            }

            return newEntry;
        }

        //used by script editor
        public void Add(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (_buildSettings.FindIndex(o => o.Name == name) > -1)
            {
                return;
            }

            //let people set what they like.
            _buildSettings.Add(new CustomStringBuildSettingEntry(name, value));
            //TODO enforce values and types
        }

        public int Count
        {
            get
            {
                return _buildSettings.Count;
            }
        }

        public string NameAt(int index)
        {
            return _buildSettings[index].Name;
        }

        public void RemoveAt(int index)
        {
            _buildSettings.RemoveAt(index);
        }

        public BaseBuildSettingEntry EntryAt(int index)
        {
            return _buildSettings[index];
        }

        public string[] Names
        {
            get
            {
                return _buildSettings.Select(o => o.Name).ToArray();
            }
        }
    }
}
