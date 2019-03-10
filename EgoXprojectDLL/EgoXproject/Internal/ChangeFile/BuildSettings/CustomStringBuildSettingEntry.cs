// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

namespace Egomotion.EgoXproject.Internal
{
    internal class CustomStringBuildSettingEntry : StringBuildSettingEntry
    {
        public CustomStringBuildSettingEntry(string name, string value = "")
        : base(name, value)
        {
        }

        public CustomStringBuildSettingEntry(PListDictionary dic)
        : base(dic)
        {
        }

        public CustomStringBuildSettingEntry(CustomStringBuildSettingEntry other)
        : base(other)
        {
        }

        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Name = "";
            }
            else
            {
                Name = name.Trim();
            }
        }
    }
}
