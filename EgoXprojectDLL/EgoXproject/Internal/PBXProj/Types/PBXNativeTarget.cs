//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXNativeTarget : PBXBaseObject
    {
        const string BUILD_CONFIGURATION_LIST_KEY = "buildConfigurationList";
        const string BUILD_PHASES_KEY = "buildPhases";
        const string BUILD_RULES_KEY = "buildRules";
        const string DEPENDENCIES_KEY = "dependencies";
        const string NAME_KEY = "name";
        const string PRODUCT_NAME_KEY = "productName";
        const string PRODUCT_REFERENCE_KEY = "productReference";
        const string PRODUCT_TYPE_KEY = "productType";

        const string MAIN_APPLICATION_KEY = "com.apple.product-type.application";
        const string OC_UNIT_TEST_KEY = "com.apple.product-type.bundle.ocunit-test";
        const string UNIT_TEST = "com.apple.product-type.bundle.unit-test";
        //xctest
        const string WATCHKIT_EXT_KEY = "com.apple.product-type.watchkit-extension";
        const string WATCH_APP_KEY = "com.apple.product-type.application.watchapp";
        const string LIBRARY_STATIC_KEY = "com.apple.product-type.library.static";
        const string LIBRARY_DYNAMIC_KEY = "com.apple.product-type.library.dynamic";
        const string FRAMEWORK_KEY = "com.apple.product-type.framework";
        const string IN_APP_PURCHASE_KEY = "com.apple.product-type.in-app-purchase-content";

        public enum TargetType
        {
            Unknown,
            Application,
            OCUnitTest,
            XCUnitTest,
            WatchKitExtension,
            WatchApp,
            StaticLibrary,
            DynamicLibary,
            Framework,
            InAppPurchase
        };

        List<PBXBaseBuildPhase> _buildPhases = new List<PBXBaseBuildPhase>();
        List<PBXTargetDependency> _dependencies = new List<PBXTargetDependency>();

        public PBXNativeTarget(string uid, PBXProjDictionary dict)
        : base(PBXTypes.PBXNativeTarget, uid, dict)
        {
        }

        #region implemented abstract members of PBXBaseObject

        public override void Populate(Dictionary<string, PBXBaseObject> allObjects)
        {
            _buildPhases = PopulateObjects<PBXBaseBuildPhase>(BuildPhaseIDs, allObjects);
            _dependencies = PopulateObjects<PBXTargetDependency>(DependencyIDs, allObjects);
            BuildConfigurationList = PopulateObject<XCConfigurationList>(BuildConfigurationListID, allObjects);
            ProductReference = PopulateObject<PBXFileReference>(ProductReferenceID, allObjects);
        }

        #endregion


        public string[] BuildPhaseIDs
        {
            get
            {
                return Dict.ArrayValue(BUILD_PHASES_KEY).ToStringArray();
            }
        }

        //TODO change to adding the actual object
        public void AddBuildPhase(PBXBaseBuildPhase phase)
        {
            if (phase == null)
            {
                return;
            }

            if (BuildPhaseIDs.Contains(phase.UID))
            {
                return;
            }

            _buildPhases.Add(phase);
            Dict.ArrayValue(BUILD_PHASES_KEY).Add(phase.UID);
        }

        public string Name
        {
            get
            {
                return Dict.StringValue(NAME_KEY);
            }
        }

        public string ProductName
        {
            get
            {
                return Dict.StringValue(PRODUCT_NAME_KEY);
            }
        }

        public string ProductReferenceID
        {
            get
            {
                return Dict.StringValue(PRODUCT_REFERENCE_KEY);
            }
        }

        public string ProductType
        {
            get
            {
                return Dict.StringValue(PRODUCT_TYPE_KEY).FromLiteral();
            }
        }

        public string BuildConfigurationListID
        {
            get
            {
                return Dict.StringValue(BUILD_CONFIGURATION_LIST_KEY);
            }
        }

        public XCConfigurationList BuildConfigurationList
        {
            get;
            private set;
        }

        public PBXBaseBuildPhase[] BuildPhases
        {
            get
            {
                return _buildPhases.ToArray();
            }
        }

        public PBXCopyFilesBuildPhase[] CopyFilesBuildPhases
        {
            get
            {
                return _buildPhases.OfType<PBXCopyFilesBuildPhase>().ToArray();
            }
        }

        public PBXFrameworksBuildPhase[] FrameworksBuildPhases
        {
            get
            {
                return _buildPhases.OfType<PBXFrameworksBuildPhase>().ToArray();
            }
        }

        public PBXResourcesBuildPhase[] ResourcesBuildPhases
        {
            get
            {
                return _buildPhases.OfType<PBXResourcesBuildPhase>().ToArray();
            }
        }

        public PBXShellScriptBuildPhase[] ShellScriptBuildPhases
        {
            get
            {
                return _buildPhases.OfType<PBXShellScriptBuildPhase>().ToArray();
            }
        }

        public PBXSourcesBuildPhase[] SourcesBuildPhases
        {
            get
            {
                return _buildPhases.OfType<PBXSourcesBuildPhase>().ToArray();
            }
        }

        public PBXHeadersBuildPhase[] HeadersBuildPhases
        {
            get
            {
                return _buildPhases.OfType<PBXHeadersBuildPhase>().ToArray();
            }
        }

        public PBXCopyFilesBuildPhase[] EmbeddedFrameworksBuildPhases
        {
            get
            {
                var embed = new List<PBXCopyFilesBuildPhase>();
                var copyPhases = CopyFilesBuildPhases;

                foreach (var cp in copyPhases)
                {
                    if (cp.DstSubfolderSpec == PBXCopyFilesBuildPhase.CopyDestination.FRAMEWORKS)
                    {
                        embed.Add(cp);
                    }
                }

                return embed.ToArray();
            }
        }

        public PBXFileReference ProductReference
        {
            get;
            private set;
        }

        public TargetType Product
        {
            get
            {
                var pt = ProductType;

                if (pt == MAIN_APPLICATION_KEY)
                {
                    return TargetType.Application;
                }

                if (pt == OC_UNIT_TEST_KEY)
                {
                    return TargetType.OCUnitTest;
                }

                if (pt == WATCH_APP_KEY)
                {
                    return TargetType.WatchApp;
                }

                if (pt == UNIT_TEST)
                {
                    return TargetType.XCUnitTest;
                }

                if (pt == WATCHKIT_EXT_KEY)
                {
                    return TargetType.WatchKitExtension;
                }

                if (pt == LIBRARY_STATIC_KEY)
                {
                    return TargetType.StaticLibrary;
                }

                if (pt == LIBRARY_DYNAMIC_KEY)
                {
                    return TargetType.DynamicLibary;
                }

                if (pt == FRAMEWORK_KEY)
                {
                    return TargetType.Framework;
                }

                if (pt == IN_APP_PURCHASE_KEY)
                {
                    return TargetType.InAppPurchase;
                }

                return TargetType.Unknown;
            }
        }

        public bool IsMainApplication
        {
            get
            {
                return Product == TargetType.Application;
            }
        }

        public string[] DependencyIDs
        {
            get
            {
                return Dict.ArrayValue(DEPENDENCIES_KEY).ToStringArray();
            }
        }

        public PBXTargetDependency[] Dependencies
        {
            get
            {
                return _dependencies.ToArray();
            }
        }

        public void RemoveBuildFile(PBXBuildFile buildFile)
        {
            foreach (var bf in _buildPhases)
            {
                bf.RemoveFile(buildFile);
            }
        }
    }
}
