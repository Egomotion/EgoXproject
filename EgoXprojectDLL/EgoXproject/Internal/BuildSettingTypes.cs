// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal abstract class BaseBuildSetting
    {
        public string DisplayName
        {
            get;
            private set;
        }

        public string BuildSettingName
        {
            get;
            private set;
        }

        public string Group
        {
            get;
            private set;
        }

        protected BaseBuildSetting(string buildSettingName, string displayName, string group)
        {
            if (string.IsNullOrEmpty(buildSettingName))
            {
                throw new System.ArgumentNullException(nameof (buildSettingName), "Must have a name for the build setting");
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new System.ArgumentNullException(nameof (displayName), "Must have a name for the friendly name");
            }

            if (string.IsNullOrEmpty(group))
            {
                throw new System.ArgumentNullException(nameof (group), "Must have a group");
            }

            BuildSettingName = buildSettingName;
            DisplayName = displayName;
            Group = group;
        }
    }

    internal class EnumBuildSetting : BaseBuildSetting
    {
        public string[] EnumNames
        {
            get;
            private set;
        }

        public string[] EnumValues
        {
            get;
            private set;
        }

        public int DefaultIndex
        {
            get;
            private set;
        }

        public EnumBuildSetting(string buildSettingName,
                                string displayName,
                                string group,
                                string[] enumValues,
                                string[] enumNames,
                                int defaultIndex = 0)
        : base(buildSettingName, displayName, group)
        {
            if (enumValues != null && enumNames != null)
            {
                //must be same length
                if (enumValues.Length != enumNames.Length)
                {
                    throw new System.ArgumentException("Arrays enumValues and enumNames must be the same length");
                }

                //must have a valid index
                if (defaultIndex < 0 || defaultIndex >= enumValues.Length)
                {
                    throw new System.ArgumentException("Default index must be a valid index value");
                }

                if (enumValues.Length > 0)
                {
                    EnumValues = enumValues;
                    EnumNames = enumNames;
                    DefaultIndex = defaultIndex;
                }
                else
                {
                    EnumValues = new string[] { };
                    EnumNames = new string[] { };
                    DefaultIndex = -1;
                }
            }
            else
            {
                EnumValues = new string[] { };
                EnumNames = new string[] { };
                DefaultIndex = -1;
            }
        }

        public string DefaultValue
        {
            get
            {
                if (DefaultIndex > -1)
                {
                    return EnumValues[DefaultIndex];
                }

                return "";
            }
        }

        public string DefaultNameValue
        {
            get
            {
                if (DefaultIndex > -1)
                {
                    return EnumNames[DefaultIndex];
                }

                return "";
            }
        }

        public bool IsValidValue(string value)
        {
            return System.Array.IndexOf(EnumValues, value) >= 0;
        }
    }

    internal class StringBuildSetting : BaseBuildSetting
    {
        public string Value
        {
            get;
            private set;
        }

        public bool IsPath
        {
            get;
            private set;
        }

        public StringBuildSetting(string buildSettingName, string displayName, string group, bool isPath = false, string defaultValue = "")
        : base(buildSettingName, displayName, group)
        {
            Value = defaultValue;
            IsPath = isPath;
        }
    }

    internal class BoolBuildSetting : BaseBuildSetting
    {
        public bool Value
        {
            get;
            private set;
        }

        public BoolBuildSetting(string buildSettingName, string displayName, string group, bool defaultValue)
        : base(buildSettingName, displayName, group)
        {
            Value = defaultValue;
        }
    }

    internal class ArrayBuildSetting : BaseBuildSetting
    {
        public bool AddInherited
        {
            get;
            private set;
        }

        public bool IsPath
        {
            get;
            private set;
        }

        public ArrayBuildSetting(string buildSettingName, string displayName, string group, bool isPath = false, bool addInherited = false)
        : base(buildSettingName, displayName, group)
        {
            AddInherited = addInherited;
            IsPath = isPath;
        }
    }

    internal class StringListBuildSetting : BaseBuildSetting
    {
        public bool AddInherited
        {
            get;
            private set;
        }

        public bool IsPath
        {
            get;
            private set;
        }

        public StringListBuildSetting(string buildSettingName, string displayName, string group, bool isPath = false, bool addInherited = false)
        : base(buildSettingName, displayName, group)
        {
            AddInherited = addInherited;
            IsPath = isPath;
        }
    }
}
