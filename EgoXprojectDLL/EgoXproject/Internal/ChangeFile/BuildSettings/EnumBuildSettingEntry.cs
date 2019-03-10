// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

namespace Egomotion.EgoXproject.Internal
{
    internal class EnumBuildSettingEntry : BaseBuildSettingEntry
    {
        const string VALUE_KEY = "Value";

        EnumBuildSetting _buildSetting;
        int _index = 0;

        public EnumBuildSettingEntry(string name, string value)
        : base(name)
        {
            GetBuildSetting();
            ValidateAndSetValue(value);
        }

        public EnumBuildSettingEntry(string name)
        : base(name)
        {
            GetBuildSetting();
            _index = _buildSetting.DefaultIndex;
        }

        public EnumBuildSettingEntry(PListDictionary dic)
        : base(dic)
        {
            GetBuildSetting();
            var value = dic.StringValue(VALUE_KEY);
            ValidateAndSetValue(value);
        }

        void GetBuildSetting()
        {
            _buildSetting = XcodeBuildSettings.Instance().BuildSetting<EnumBuildSetting>(Name);

            if (_buildSetting == null)
            {
                throw new System.ArgumentException(Name + " is not an enum build setting.");
            }
        }

        void ValidateAndSetValue(string value)
        {
            if (!_buildSetting.IsValidValue(value))
            {
                UnityEngine.Debug.LogError(value + " is not a valid setting for " + Name + ". Resetting to default value: " + _buildSetting.DefaultValue);
                _index = _buildSetting.DefaultIndex;
            }
            else
            {
                _index = System.Array.IndexOf(_buildSetting.EnumValues, value);
            }
        }

        public EnumBuildSettingEntry(EnumBuildSettingEntry other)
        : base(other)
        {
            _buildSetting = other._buildSetting;
            _index = other._index;
        }

        #region implemented abstract members of BaseChangeEntry

        public override PListDictionary Serialize()
        {
            var dic = base.Serialize();
            dic.Add(VALUE_KEY, _buildSetting.EnumValues[_index]);
            return dic;
        }

        #endregion

        public string Value
        {
            get
            {
                return _buildSetting.EnumValues[_index];
            }
        }

        public string[] AcceptedValues
        {
            get
            {
                return _buildSetting.EnumValues;
            }
        }

        public string DefaultValue
        {
            get
            {
                return _buildSetting.DefaultValue;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _index;
            }
            set
            {
                if (value >= 0 || value < _buildSetting.EnumValues.Length)
                {
                    _index = value;
                }
            }
        }
    }
}
