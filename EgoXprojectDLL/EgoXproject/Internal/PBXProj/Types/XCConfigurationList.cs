//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.Internal
{
    internal class XCConfigurationList : PBXBaseObject
    {
        const string BUILD_CONFIGURATIONS_KEY = "buildConfigurations";
        const string DEFAULT_CONFIGURATION_IS_VISIBLE_KEY = "defaultConfigurationIsVisible";
        const string DEFAULT_CONFIGURATION_NAME_KEY = "defaultConfigurationName";

        List<XCBuildConfiguration> _buildConfigurations = new List<XCBuildConfiguration>();

        public XCConfigurationList(string uid, PBXProjDictionary dict)
        : base(PBXTypes.XCConfigurationList, uid, dict)
        {
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            _buildConfigurations = PopulateObjects<XCBuildConfiguration>(BuildConfigurationIDs, allObjects);
        }

        #endregion


        public string[] BuildConfigurationIDs
        {
            get
            {
                return Dict.ArrayValue(BUILD_CONFIGURATIONS_KEY).ToStringArray();
            }
        }

        public XCBuildConfiguration[] BuildConfigurations
        {
            get
            {
                return _buildConfigurations.ToArray();
            }
        }
    }
}
