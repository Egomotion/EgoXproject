//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System.ComponentModel;

namespace Egomotion.EgoXproject.Internal
{
    internal static class SourceTreeLocation
    {
        public const string SDK_ROOT = "SDKROOT";
        public const string SOURCE_ROOT = "SOURCE_ROOT";
        public const string GROUP = "\"<group>\"";
        public const string ABSOLUTE = "\"<absolute>\"";
        public const string BUILT_PRODUCTS_DIR = "BUILT_PRODUCTS_DIR";
        public const string DEVELOPER_DIR = "DEVELOPER_DIR";
    }

    internal static class XcodeBool
    {
        public const string YES = "YES";
        public const string NO = "NO";
    }

    internal enum BoolEnum
    {
        Yes,
        No
    }

    internal enum MergeMethod
    {
        Append,
        Replace
    }

    internal enum BuildPlatform
    {
        iOS,
        tvOS,
    };

    internal enum PBXTypes
    {
        PBXBuildFile,
        PBXContainerItemProxy,
        PBXCopyFilesBuildPhase,
        PBXFileReference,
        PBXFrameworksBuildPhase,
        PBXGroup,
        PBXNativeTarget,
        PBXProject,
        PBXResourcesBuildPhase,
        PBXShellScriptBuildPhase,
        PBXSourcesBuildPhase,
        PBXTargetDependency,
        PBXVariantGroup,
        XCBuildConfiguration,
        XCConfigurationList,
        XCVersionGroup,
        PBXHeadersBuildPhase,
    };
}
