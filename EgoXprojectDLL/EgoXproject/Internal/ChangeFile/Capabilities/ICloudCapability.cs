// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class ICloudCapability : BaseCapability
    {
        const string KEY_VALUE_STORAGE_KEY = "KeyValueStorage";
        const string ICLOUD_DOCUMENTS_KEY = "iCloudDocuments";
        const string CLOUDKIT_KEY = "CloudKit";
        const string USE_CUSTOM_CONTAINERS_KEY = "UseCustomContainers";
        const string CUSTOM_CONTAINERS_KEY = "CustomContainers";

        public List<string> CustomContainers
        {
            get;
            private set;
        }

        public ICloudCapability()
        {
            CustomContainers = new List<string>();
        }

        public ICloudCapability(PListDictionary dic)
        {
            KeyValueStorage = dic.BoolValue(KEY_VALUE_STORAGE_KEY);
            iCloudDocuments = dic.BoolValue(ICLOUD_DOCUMENTS_KEY);
            CloudKit = dic.BoolValue(CLOUDKIT_KEY);
            UseCustomContainers = dic.BoolValue(USE_CUSTOM_CONTAINERS_KEY);
            var groups = dic.ArrayValue(CUSTOM_CONTAINERS_KEY);

            if (groups != null && groups.Count > 0)
            {
                CustomContainers = new List<string>(groups.ToStringArray());
            }
            else
            {
                CustomContainers = new List<string>();
            }
        }

        public ICloudCapability(ICloudCapability other)
        : base (other)
        {
            KeyValueStorage = other.KeyValueStorage;
            iCloudDocuments = other.iCloudDocuments;
            CloudKit = other.CloudKit;
            UseCustomContainers = other.UseCustomContainers;
            CustomContainers = new List<string>(other.CustomContainers);
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.AddIfTrue(KEY_VALUE_STORAGE_KEY, KeyValueStorage);
            dic.AddIfTrue(ICLOUD_DOCUMENTS_KEY, iCloudDocuments);
            dic.AddIfTrue(CLOUDKIT_KEY, CloudKit);
            dic.AddIfTrue(USE_CUSTOM_CONTAINERS_KEY, UseCustomContainers);

            if (CustomContainers.Count > 0)
            {
                dic.Add(CUSTOM_CONTAINERS_KEY, new PListArray(CustomContainers));
            }

            return dic;
        }

        public override BaseCapability Clone()
        {
            return new ICloudCapability(this);
        }

        #endregion

        public bool KeyValueStorage
        {
            get;
            set;
        }
        public bool iCloudDocuments
        {
            get;
            set;
        }
        public bool CloudKit
        {
            get;
            set;
        }

        public bool UseCustomContainers
        {
            get;
            set;
        }

    }
}
