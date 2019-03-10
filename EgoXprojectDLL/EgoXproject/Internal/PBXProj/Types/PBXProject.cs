//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXProject : PBXBaseObject
    {
        const string ATTRIBUTES_KEY = "attributes";
        const string BUILD_CONFIGURATION_LIST_KEY = "buildConfigurationList";
        const string COMPATIBILITY_VERSION_KEY = "compatibilityVersion";
        const string DEVELOPMENT_REGION_KEY = "developmentRegion";
        const string HAS_SCANNED_FOR_ENCODINGS_KEY = "hasScannedForEncodings";
        const string KNOWN_REGIONS_KEY = "knownRegions";
        const string MAIN_GROUP_KEY = "mainGroup";
        const string PROJECT_DIR_PATH_KEY = "projectDirPath";
        const string PROJECT_ROOT_KEY = "projectRoot";
        const string TARGETS_KEY = "targets";
        const string PRODUCT_REF_GROUP_KEY = "productRefGroup";
        const string PROJECT_REFERENCES_KEY = "projectReferences";

        const string TARGET_ATTRIBUTES_KEY = "TargetAttributes";
        const string DEVELOPMENT_TEAM_KEY = "DevelopmentTeam";
        const string PROVISIONING_STYLE_KEY = "ProvisioningStyle";
        const string SYSTEM_CAPABILITIES_KEY = "SystemCapabilities";
        const string ENABLED_KEY = "enabled";

        List<PBXNativeTarget> _targets = new List<PBXNativeTarget>();

        public PBXProject(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXProject, uid, dict)
        {
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            _targets = PopulateObjects<PBXNativeTarget>(TargetIDs, allObjects);
            MainGroup = PopulateObject<PBXGroup>(MainGroupID, allObjects);
            BuildConfigurationList = PopulateObject<XCConfigurationList>(BuildConfigurationListID, allObjects);
        }

        #endregion

        public string[] TargetIDs
        {
            get
            {
                return Dict.ArrayValue(TARGETS_KEY).ToStringArray();
            }
        }

        //TODO add and remove targets

        public string MainGroupID
        {
            get
            {
                return Dict.StringValue(MAIN_GROUP_KEY);
            }
        }

        public string BuildConfigurationListID
        {
            get
            {
                return Dict.StringValue(BUILD_CONFIGURATION_LIST_KEY);
            }
        }

        public string ProductRefGroupID
        {
            get
            {
                return Dict.StringValue(PRODUCT_REF_GROUP_KEY);
            }
        }

        //this appears when another project is dropped in to a project.
        //will ref a group and an file ref.
        //TODO ignore this for now
        // projectReferences = (
        // {
        //    ProductGroup = C4E221681AF4E1C8001A6694 /* Products */;
        //      ProjectRef = C4E221671AF4E1C8001A6694 /* ShareKit.xcodeproj */;
        // },
        //      public string[] ProjectReferences
        //      {
        //          get {
        //              return Dict.ArrayValue(_projectReferencesKey).ToStringArray();
        //          }
        //      }

        public PBXNativeTarget[] Targets
        {
            get
            {
                return _targets.ToArray();
            }
        }

        public PBXGroup MainGroup
        {
            get;
            private set;
        }

        public XCConfigurationList BuildConfigurationList
        {
            get;
            private set;
        }

        //public PBXGroup ProductRefGroup { get; private set; }

        public PBXProjDictionary Attributes
        {
            get
            {
                var val = Dict.DictionaryValue (ATTRIBUTES_KEY);

                if (val == null)
                {
                    val = new PBXProjDictionary ();
                    Dict [ATTRIBUTES_KEY] = val;
                }

                return val;
            }
        }

        public PBXProjDictionary TargetAttributes
        {
            get
            {
                var val = Attributes.DictionaryValue(TARGET_ATTRIBUTES_KEY);

                if (val == null)
                {
                    val = new PBXProjDictionary ();
                    Attributes [TARGET_ATTRIBUTES_KEY] = val;
                }

                return val;
            }
        }

        public PBXProjDictionary TargetAttributesEntry(string targetKey)
        {
            var val = TargetAttributes.DictionaryValue (targetKey);

            if (val == null)
            {
                val = new PBXProjDictionary ();
                TargetAttributes[targetKey] = val;
            }

            return val;
        }

        public string DevelopmentTeam(string targetKey)
        {
            var attribs = TargetAttributesEntry(targetKey);

            if (!attribs.ContainsKey (DEVELOPMENT_TEAM_KEY))
            {
                return string.Empty;
            }

            return attribs.StringValue(DEVELOPMENT_TEAM_KEY);
        }

        public void SetDevelopmentTeam(string targetKey, string teamId)
        {
            var attribs = TargetAttributesEntry(targetKey);
            attribs[DEVELOPMENT_TEAM_KEY] = new PBXProjString(teamId);
        }

        public void EnableAutomaticProvisioning(string targetKey, bool enable)
        {
            var attribs = TargetAttributesEntry(targetKey);

            //is on by default, so don't add the key. this ensures we don't add things to the project that have not been explictly asked for
            if (!attribs.ContainsKey (PROVISIONING_STYLE_KEY) && enable)
            {
                return;
            }

            attribs[PROVISIONING_STYLE_KEY] = new PBXProjString(enable ? "Automatic" : "Manual");
        }

        public void EnableSystemCapability(string targetKey, string capabilityKey, bool enabled)
        {
            var attribs = TargetAttributesEntry(targetKey);
            var systemCapabilities = attribs.DictionaryValue(SYSTEM_CAPABILITIES_KEY);

            if (systemCapabilities == null)
            {
                systemCapabilities = new PBXProjDictionary();
                attribs[SYSTEM_CAPABILITIES_KEY] = systemCapabilities;
            }

            var enabledDic = new PBXProjDictionary();
            enabledDic.Add(ENABLED_KEY, enabled ? "1" : "0");
            systemCapabilities[capabilityKey] = enabledDic;
        }
    }
}
