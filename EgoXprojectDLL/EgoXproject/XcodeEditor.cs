//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;

namespace Egomotion.EgoXproject
{
    public static class XcodeEditor
    {
        static bool PlatformFromTarget(BuildTarget target, out BuildPlatform platform)
        {
            switch (target)
            {
            case BuildTarget.iOS:
                platform = BuildPlatform.iOS;
                return true;

            case BuildTarget.tvOS:
                platform = BuildPlatform.tvOS;
                return true;

            default:
                platform = BuildPlatform.iOS;
                return false;
            }
        }

        static void InvalidTargetMessage()
        {
            Debug.LogError("Invalid target specified. Only iOS and tvOS are supported");
        }

        //Get or set whether egoXproject should run automatically. true = enabled.
        public static bool AutoRunEnabled
        {
            get
            {
                return XcodeController.Instance().Settings.AutoRunEnabled;
            }
            set
            {
                XcodeController.Instance().Settings.AutoRunEnabled = value;
            }
        }

        //Get the array of configuration names
        public static string[] Configurations(BuildTarget target)
        {
            BuildPlatform platform;

            if (PlatformFromTarget(target, out platform))
            {
                return XcodeController.Instance().Configuration(platform).Configurations;
            }
            else
            {
                InvalidTargetMessage();
                return null;
            }
        }

        //Get the name of the active configuration
        public static string ActiveConfiguration(BuildTarget target)
        {
            BuildPlatform platform;

            if (PlatformFromTarget(target, out platform))
            {
                return XcodeController.Instance().Configuration(platform).ActiveConfiguration;
            }
            else
            {
                InvalidTargetMessage();
                return "";
            }
        }

        //Set the name of the active configuration
        public static void SetActiveConfigutation(BuildTarget target, string activeConfiguration)
        {
            BuildPlatform platform;

            if (PlatformFromTarget(target, out platform))
            {
                XcodeController.Instance().Configuration(platform).ActiveConfiguration = activeConfiguration;
            }
            else
            {
                InvalidTargetMessage();
            }
        }

        //Apply the changes to the specified project.
        //target is the BuildTarget that gets passed to the PostProcessBuild attributed function.
        //pathToBuiltProject is the path that gets passed to the PostProcessBuild attributed function.
        public static void ApplyChanges(BuildTarget target, string pathToBuiltProject)
        {
            XcodeController.Instance().ModifyXcodeProject(target, pathToBuiltProject);
        }

        #region info plist

        //Set a string value in the Info.plist
        public static void SetInfoPlistEntry(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.InfoPlistChanges[key] = new PListString(value);
        }

        //Set an int value in the Info.plist
        public static void SetInfoPlistEntry(string key, int value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.InfoPlistChanges[key] = new PListInteger(value);
        }

        //Set a float value in the Info.plist
        public static void SetInfoPlistEntry(string key, float value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.InfoPlistChanges[key] = new PListReal(value);
        }

        //Set a bool value in the Info.plist
        public static void SetInfoPlistEntry(string key, bool value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.InfoPlistChanges[key] = new PListBoolean(value);
        }

        //Set a date value in the Info.plist
        public static void SetInfoPlistEntry(string key, System.DateTime value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.InfoPlistChanges[key] = new PListDate(value);
        }

        #endregion

        #region frameworks

        //Add a system framework or library by name (eg. Accelerate.framework),
        //and set its link type (i.e. whether it is Optional or Required).
        public static void AddSystemFrameworkOrLibrary(string name, LinkType linkType)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.Frameworks.Add(name, linkType);
        }

        //Add a system framework or library by name (eg. Accelerate.framework)
        //Link type will be set to Required.
        public static void AddSystemFrameworkOrLibrary(string name)
        {
            AddSystemFrameworkOrLibrary(name, LinkType.Required);
        }

        //Add a custom framework or library.
        //Path must be absolute or relative to the project folder (e.g. "Assets/Third Party/Foo.framework")
        //Set its link type (i.e. whether it is Optional or Required).
        //Set its add method (i.e. whether it should be copied to the Xcode project or just linked)
        public static void AddCustomFrameworkOrLibrary(string pathToFrameworkOrLibrary, LinkType linkType, AddMethod addMethod)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.FilesAndFolders.AddFrameworkOrLibrary(pathToFrameworkOrLibrary, addMethod, linkType);
        }

        //Add a custom framework or library.
        //Path must be absolute or relative to the project folder (e.g. "Assets/Third Party/Foo.framework")
        //Set its link type (i.e. whether it is Optional or Required).
        //Add method will be set to link
        public static void AddCustomFrameworkOrLibrary(string pathToFrameworkOrLibrary, LinkType linkType)
        {
            AddCustomFrameworkOrLibrary(pathToFrameworkOrLibrary, linkType, AddMethod.Link);
        }

        //Add a custom framework or library.
        //Path must be absolute or relative to the project folder (e.g. "Assets/Third Party/Foo.framework")
        //Link type will be set to Required, and Add method will be set to link
        public static void AddCustomFrameworkOrLibrary(string pathToFrameworkOrLibrary)
        {
            AddCustomFrameworkOrLibrary(pathToFrameworkOrLibrary, LinkType.Required);
        }

        //Add a custom framework as an embedded framework
        //Path must be absolute or relative to the project folder (e.g. "Assets/Third Party/Foo.framework")
        public static void AddEmbeddedFramework(string pathToFramework)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.FilesAndFolders.AddEmbeddedFramework(pathToFramework);
        }

        #endregion

        #region files and folders

        //Add a file or folder
        //Path must be absolute or relative to the project folder (e.g. "Assets/Third Party/Foo.m")
        //Add method will be set to link
        //No attributes will be set
        public static void AddFileOrFolder(string pathToFileOrFolder)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.FilesAndFolders.AddFileOrFolder(pathToFileOrFolder, AddMethod.Link);
        }

        //Add a file or folder
        //Path must be absolute or relative to the project folder (e.g. "Assets/Third Party/Foo.m")
        //Set its add method (i.e. whether it should be copied to the Xcode project or just linked)
        //No attributes will be set
        public static void AddFileOrFolder(string pathToFileOrFolder, AddMethod addMethod)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.FilesAndFolders.AddFileOrFolder(pathToFileOrFolder, addMethod);
        }

        //Add a file or folder
        //Path must be absolute or relative to the project folder (e.g. "Assets/Third Party/Foo.m")
        //Set its add method (i.e. whether it should be copied to the Xcode project or just linked)
        //Set any compiler flags the file may require (e.g. -fobjc-arc)
        [System.Obsolete]
        public static void AddFileOrFolder(string pathToFileOrFolder, AddMethod addMethod, string compilerFlags)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.FilesAndFolders.AddSourceFile(pathToFileOrFolder, addMethod, compilerFlags);
        }

        //Add a source file
        //Path must be absolute or relative to the project folder (e.g. "Assets/Third Party/Foo.m")
        //Set its add method (i.e. whether it should be copied to the Xcode project or just linked)
        //Set any compiler flags the file may require (e.g. -fobjc-arc)
        public static void AddSourceFile(string pathToSourceFile, AddMethod addMethod, string compilerFlags)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.FilesAndFolders.AddSourceFile(pathToSourceFile, addMethod, compilerFlags);
        }

        #endregion

        #region build settings

        //Set a build setting to a specified value
        public static void SetBuildSetting(string buildSetting, string value)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.BuildSettings.Add(buildSetting, value);
        }

        #endregion

        #region scripts

        //Add a script
        //Uses the default bash shell
        public static void AddScript(string script)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.Scripts.AddScript(script);
        }

        //Add a script with a specified shell
        public static void AddScript(string script, string shell)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.Scripts.AddScript(script, shell);
        }

        //Add a script with a specified shell and name
        public static void AddScript(string script, string shell, string name)
        {
            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.Scripts.AddScript(script, shell, name);
        }

        #endregion

        #region signing

        // Set the development team id
        public static void SetTeamId(string teamId)
        {
            if (string.IsNullOrEmpty(teamId))
            {
                Debug.LogError("EgoXproject: Team Id cannot be empty");
                return;
            }

            var changeFile = XcodeController.Instance().ScriptingChangeFile;
            changeFile.Signing.TeamId = teamId;
        }

        public static void EnableAutomaticProvisioning(bool enable)
        {
            XcodeController.Instance().ScriptingChangeFile.Signing.AutomaticProvisioning = enable;
        }

        #endregion
    }
}
