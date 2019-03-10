//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Egomotion.EgoXproject.Internal
{
    internal class PBXProj
    {
        #region keys

        const string ROOT_OBJECT_KEY = "rootObject";
        const string ARCHIVE_VERSION_KEY = "archiveVersion";
        const string CLASSES_KEY = "classes";
        const string OBJECT_VERSION_KEY = "objectVersion";
        const string OBJECTS_KEY = "objects";

        #endregion

        #region environment variables

        const string PROJECT_DIR = "$(PROJECT_DIR)";
        const string SRC_ROOT = "$(SRCROOT)";

        #endregion

        #region private vars

        //        /some path/ios/Unity-iPhone.xcodeproj/project.pbxproj
        // PathToXcodeProject ^
        // PathToProjectContainer  ^
        // PathToProjectFile                                  ^
        public string PathToXcodeProject
        {
            get;
            private set;
        }
        public string PathToProjectContainer
        {
            get;
            private set;
        }
        public string PathToProjectFile
        {
            get;
            private set;
        }

        // 45 = 3.1
        // 46 = 3.2
        // 47 = 6.3
        // 48 = 8.0
        readonly string[] _supportedVersions = { "45", "46", "47", "48" };

        HashSet<string> _usedUids = new HashSet<string>();

        string _errorMesssage = "";

        #endregion

        #region project elements

        PBXProjDictionary _dict;

        Dictionary<string, PBXBaseObject> _allObjects = new Dictionary<string, PBXBaseObject>();

        string _rootObjectId;

        #endregion

        #region public interface

        public string[] InfoPlistPaths
        {
            get;
            private set;
        }

        public string[] EntitlementsFilePaths
        {
            get;
            private set;
        }

        public bool Load(string pathToPBXProjFile)
        {
            pathToPBXProjFile = Path.GetFullPath(pathToPBXProjFile);
            Reset();
            PathToProjectFile = "";
            var parser = new PBXProjParser();
            _dict = parser.Parse(pathToPBXProjFile);

            if (_dict == null)
            {
                return false;
            }

            if (!CanReadProjectFile())
            {
                _dict = null;
                return false;
            }

            PathToProjectFile = pathToPBXProjFile;
            PathToProjectContainer = Path.GetDirectoryName(PathToProjectFile);
            PathToXcodeProject = Path.GetDirectoryName(PathToProjectContainer);
            return ReadProjectFile();
        }

        public bool Save()
        {
            if (_dict == null)
            {
                _errorMesssage = "Project file not loaded";
                return false;
            }

            if (string.IsNullOrEmpty(PathToProjectFile))
            {
                _errorMesssage = "No save path specified";
                return false;
            }

            PrettyWriter();
            return true;
        }

        public string SavePath
        {
            get
            {
                return PathToProjectFile;
            }
        }

        public string ErrorMessage
        {
            get
            {
                var m = _errorMesssage;
                _errorMesssage = "";
                return m;
            }
        }

        //path is relative to SDK root
        public void AddSystemFramework(string path, LinkType linkType)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("EgoXproject: System Framework path cannot be empty");
                return;
            }

            if (Path.IsPathRooted(path))
            {
                Debug.LogError("EgoXproject: System framework path must be relative to the SDK root: " + path);
                return;
            }

            if (Path.GetExtension(path) == ".framework")
            {
                if (!path.StartsWith(XcodeSDKFinder.DEFAULT_FRAMEWORKS_SUBPATH, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    path = Path.Combine(XcodeSDKFinder.DEFAULT_FRAMEWORKS_SUBPATH, path);
                }
            }
            else
            {
                if (!path.StartsWith(XcodeSDKFinder.DEFUALT_LIB_SUBPATH, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    path = Path.Combine(XcodeSDKFinder.DEFUALT_LIB_SUBPATH, path);
                }

                if (Path.GetExtension(path) == ".dylib")
                {
                    string fileName = Path.GetFileName(path);
                    string tdbPath = Path.ChangeExtension(path, ".tbd");
                    string tbdFileName = Path.GetFileName(tdbPath);
                    Debug.LogWarning(fileName + " replaced with " + tbdFileName + ". Update your change files to avoid this warning with Xcode 7 and newer.");
                    path = tdbPath;
                }
            }

            AddFramework(path, AddMethod.Link, linkType, true);
        }

        //path is absolute
        public void AddCustomFramework(string path, AddMethod addMethod, LinkType linkType)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("EgoXproject: Custom Framework path cannot be empty");
                return;
            }

            path = Path.GetFullPath(path);
            AddFramework(path, addMethod, linkType, false);
        }

        public void AddEmbeddedFramework(string path)
        {
            //link will always be required
            //add is always copy
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("EgoXproject: Embedded Framework path cannot be empty");
                return;
            }

            path = Path.GetFullPath(path);
            PBXFileType fileType = PBXFileTypeHelper.FileTypeFromFileName(path);

            if (!PBXFileTypeHelper.IsFramework(fileType))
            {
                Debug.LogError("EgoXproject: Not a valid framework: " + path);
                return;
            }

            var frameworkFileRef = AddFramework(path, AddMethod.Copy, LinkType.Required, false);

            if (frameworkFileRef == null)
            {
                //TODO log error?
                return;
            }

            var embeddedPhases = MainTarget.EmbeddedFrameworksBuildPhases;
            PBXCopyFilesBuildPhase copyPhase = null;

            if (embeddedPhases.Length <= 0)
            {
                copyPhase = PBXCopyFilesBuildPhase.Create(GenerateUID());
                copyPhase.BuildActionMask = "2147483647";
                copyPhase.DstSubfolderSpec = PBXCopyFilesBuildPhase.CopyDestination.FRAMEWORKS;
                copyPhase.Name = "Embed Frameworks";
                copyPhase.RunOnlyForDeploymentPostprocessing = "0";
                AddObject(copyPhase);
                MainTarget.AddBuildPhase(copyPhase);
            }
            else
            {
                copyPhase = embeddedPhases[0];
            }

            var relativePath = PathRelativeToProjectPath(path);
            PBXBuildFile buildFile = null;
            PBXFileReference fileRef = null;

            if (!FindBuildFileAndFileReference(embeddedPhases, relativePath, out buildFile, out fileRef))
            {
                buildFile = PBXBuildFile.Create(GenerateUID(), frameworkFileRef);
                AddObject(buildFile);
                copyPhase.AddFile(buildFile);
            }

            //make sure these options are set
            buildFile.AddAttribute(PBXBuildFile.CODE_SIGN_ON_COPY_KEY);
            buildFile.AddAttribute(PBXBuildFile.REMOVE_HEADERS_ON_COPY_KEY);

            foreach (var buildConfig in MainTarget.BuildConfigurationList.BuildConfigurations)
            {
                buildConfig.AddRunpathSearchPath("@executable_path/Frameworks");
            }
        }

        public void AddScript(string name, string shell, string script)
        {
            name = name.Trim();
            shell = shell.Trim();
            script = script.Trim();

            //don't add empty scripts
            if (string.IsNullOrEmpty(script))
            {
                return;
            }

            bool hasName = !string.IsNullOrEmpty(name);
            var scripts = MainTarget.ShellScriptBuildPhases;

            if (scripts != null && scripts.Length > 0)
            {
                foreach (var s in scripts)
                {
                    //replace scripts with same name
                    if (hasName && s.Name == name)
                    {
                        s.Script = script;
                        s.ShellPath = shell;
                        return;
                    }

                    //replace scripts with same script content
                    if (s.Script.Trim() == script.Trim())
                    {
                        s.Name = name;
                        s.ShellPath = shell;
                        return;
                    }
                }
            }

            var buildPhase = PBXShellScriptBuildPhase.Create(GenerateUID(), name, script, shell);
            AddObject(buildPhase);
            MainTarget.AddBuildPhase(buildPhase);
        }

        public void AddFileOrFolder(BaseFileEntry entry)
        {
            AddFileOrFolder(entry, null);
        }

        void AddFileOrFolder(BaseFileEntry entry, GroupPath groupPath)
        {
            if (entry == null)
            {
                Debug.LogError("EgoXproject: entry cannot be null");
                return;
            }

            if (entry is FolderEntry)
            {
                AddFolder(entry as FolderEntry, groupPath);
            }
            else if (entry is FrameworkEntry)
            {
                var f = entry as FrameworkEntry;

                if (f.Embedded)
                {
                    AddEmbeddedFramework(f.Path);
                }
                else
                {
                    AddCustomFramework(f.Path, f.Add, f.Link);
                }
            }
            else if (entry is StaticLibraryEntry)
            {
                var sl = entry as StaticLibraryEntry;
                AddCustomFramework(sl.Path, sl.Add, sl.Link);
            }
            else if (entry is SourceFileEntry || entry is FileEntry)
            {
                AddFileOrContainer(entry, groupPath);
            }
            else
            {
                Debug.LogError("EgoXproject: Unknown entry type. Yell at the developer.");
            }
        }

        #region Build Settings

        public void AddStringBuildSetting(string setting, string value)
        {
            AddStringBuildSettingToBuildConfigurations(MainTarget.BuildConfigurationList.BuildConfigurations, setting, value);
        }

        public void AddCustomStringBuildSetting (string setting, string value)
        {
            AddCustomStringBuildSettingToBuildConfigurations (MainTarget.BuildConfigurationList.BuildConfigurations, setting, value);
        }

        public void AddBoolBuildSetting(string setting, bool value)
        {
            AddBoolBuildSettingToBuildConfigurations(MainTarget.BuildConfigurationList.BuildConfigurations, setting, value);
        }

        public void AddCollectionBuildSetting(string setting, string[] values, MergeMethod mergeMethod)
        {
            AddCollectionBuildSettingToBuildConfigurations(MainTarget.BuildConfigurationList.BuildConfigurations, setting, values, mergeMethod);
        }

        public void AddEnumBuildSetting(string setting, string value)
        {
            AddEnumBuildSettingToBuildConfigurations(MainTarget.BuildConfigurationList.BuildConfigurations, setting, value);
        }

        void AddStringBuildSettingToBuildConfigurations(XCBuildConfiguration[] buildConfigurations, string setting, string value)
        {
            foreach (var buildConfig in buildConfigurations)
            {
                buildConfig.AddStringBuildSetting(setting, value);
            }
        }

        void AddCustomStringBuildSettingToBuildConfigurations (XCBuildConfiguration [] buildConfigurations, string setting, string value)
        {
            foreach (var buildConfig in buildConfigurations)
            {
                buildConfig.AddCustomStringBuildSetting (setting, value);
            }
        }

        void AddBoolBuildSettingToBuildConfigurations(XCBuildConfiguration[] buildConfigurations, string setting, bool value)
        {
            foreach (var buildConfig in buildConfigurations)
            {
                buildConfig.AddBoolBuildSetting(setting, value);
            }
        }

        void AddCollectionBuildSettingToBuildConfigurations(XCBuildConfiguration[] buildConfigurations, string setting, string[] values, MergeMethod mergeMethod)
        {
            foreach (var buildConfig in buildConfigurations)
            {
                buildConfig.AddCollectionBuildSetting(setting, values, mergeMethod);
            }
        }

        void AddEnumBuildSettingToBuildConfigurations(XCBuildConfiguration[] buildConfigurations, string setting, string value)
        {
            foreach (var buildConfig in buildConfigurations)
            {
                buildConfig.AddEnumBuildSetting(setting, value);
            }
        }

        #endregion

        #endregion

        #region serialization

        void PrettyWriter()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(PBXProjParser.header);
            sb.AppendLine("{");
            sb.Append(_dict.KeyValueToString(ARCHIVE_VERSION_KEY, 1, PBXProjDictionary.PrintStyle.Indented));
            sb.Append(_dict.KeyValueToString(CLASSES_KEY, 1, PBXProjDictionary.PrintStyle.Indented));
            sb.Append(_dict.KeyValueToString(OBJECT_VERSION_KEY, 1, PBXProjDictionary.PrintStyle.Indented));
            sb.AppendLine("\t" + OBJECTS_KEY + " = {");
            var objectDic = _dict.Element<PBXProjDictionary>(OBJECTS_KEY);

            foreach (var kvp in objectDic)
            {
                var entryDic = kvp.Value as PBXProjDictionary;
                bool used = false;

                if (entryDic != null)
                {
                    var isa = entryDic.Element<PBXProjString>("isa");

                    if (isa != null)
                    {
                        if (isa.Value == PBXTypes.PBXBuildFile.ToString() || isa.Value == PBXTypes.PBXFileReference.ToString())
                        {
                            sb.Append(objectDic.KeyValueToString(kvp.Key, 2, PBXProjDictionary.PrintStyle.Inline));
                            used = true;
                        }
                    }
                }

                if (!used)
                {
                    sb.Append(objectDic.KeyValueToString(kvp.Key, 2, PBXProjDictionary.PrintStyle.Indented));
                }
            }

            sb.AppendLine("\t};");
            sb.Append(_dict.KeyValueToString(ROOT_OBJECT_KEY, 1, PBXProjDictionary.PrintStyle.Indented));
            sb.AppendLine("}");
            File.WriteAllText(PathToProjectFile, sb.ToString());
        }


        bool CanReadProjectFile()
        {
            var archiveVersion = _dict.StringValue(ARCHIVE_VERSION_KEY);

            if (archiveVersion != "1")
            {
                _errorMesssage = "Incomatible archive version.";
                return false;
            }

            var classes = _dict.DictionaryValue(CLASSES_KEY);

            if (classes == null/* || classes.Count != 0 */)
            {
                _errorMesssage = "Malformed file. No classes entry.";
                return false;
            }

            var objectVersion = _dict.StringValue(OBJECT_VERSION_KEY);

            if (!_supportedVersions.Contains(objectVersion))
            {
                _errorMesssage = "Unsupported version: " + objectVersion;
                return false;
            }

            var root = _dict.StringValue(ROOT_OBJECT_KEY);

            if (string.IsNullOrEmpty(root))
            {
                _errorMesssage = "Malformed file. No root object.";
                return false;
            }

            var objects = _dict.DictionaryValue(OBJECTS_KEY);

            if (objects == null || objects.Count <= 0)
            {
                _errorMesssage = "Malformed file. No objects.";
                return false;
            }

            return true;
        }

        bool ReadProjectFile()
        {
            var objects = _dict.Element<PBXProjDictionary>(OBJECTS_KEY);

            foreach (var kvp in objects)
            {
                _usedUids.Add(kvp.Key);
                var element = kvp.Value as PBXProjDictionary;

                if (element != null)
                {
                    //create our object representations
                    AddElement(kvp.Key, element);
                }
            }

            _rootObjectId = _dict.StringValue(ROOT_OBJECT_KEY);

            //link everything together
            foreach (var o in _allObjects.Values)
            {
                o.Populate(_allObjects);
            }

            RootProject = _allObjects[_rootObjectId] as PBXProject;

            if (RootProject == null)
            {
                _errorMesssage = "Could not find the root project!";
                Reset();
                return false;
            }

            foreach (var t in RootProject.Targets)
            {
                //set the main application. This is the one we will be modifing. We don't touch the unit tests or the project settings
                if (t.IsMainApplication)
                {
                    MainTarget = t;
                }

                //link the target build configurations to the project configurations
                foreach (var buildConfig in t.BuildConfigurationList.BuildConfigurations)
                {
                    buildConfig.Parent = RootProject.BuildConfigurationList.BuildConfigurations.Single(bc => bc.Name.Equals(buildConfig.Name));
                }
            }

            if (MainTarget == null)
            {
                _errorMesssage = "EgoXproject: Could not find the main application target!";
                Reset();
                return false;
            }

            //info plist locations
            var infoPlistLocations = new HashSet<string>();

            foreach (var buildConfig in MainTarget.BuildConfigurationList.BuildConfigurations)
            {
                //TODO something better than this
                var str = buildConfig.StringForKey("INFOPLIST_FILE");

                if (!string.IsNullOrEmpty(str))
                {
                    infoPlistLocations.Add(Path.Combine(PathToXcodeProject, str));
                }
            }

            InfoPlistPaths = infoPlistLocations.ToArray();
            //entitlements file locations
            var entitlementsLocations = new HashSet<string>();

            foreach (var buildConfig in MainTarget.BuildConfigurationList.BuildConfigurations)
            {
                //TODO something better than this
                var str = buildConfig.StringForKey("CODE_SIGN_ENTITLEMENTS");

                if (!string.IsNullOrEmpty(str))
                {
                    entitlementsLocations.Add(Path.Combine(PathToXcodeProject, str));
                }
            }

            EntitlementsFilePaths = entitlementsLocations.ToArray();
            return true;
        }

        void Reset()
        {
            _allObjects.Clear();
            _usedUids.Clear();
        }

        void AddElement(string key, PBXProjDictionary section)
        {
            var isaValue = section.Element<PBXProjString>("isa");

            if (isaValue == null)
            {
                //TODO err message?
                return;
            }

            PBXTypes isa;

            try
            {
                isa = (PBXTypes) System.Enum.Parse(typeof(PBXTypes), isaValue.Value);
            }
            catch
            {
                //Unknown type
                return;
            }

            switch (isa)
            {
            case PBXTypes.PBXBuildFile:
                _allObjects.Add(key, new PBXBuildFile(key, section));
                break;

            case PBXTypes.PBXContainerItemProxy:
                _allObjects.Add(key, new PBXContainerItemProxy(key, section));
                break;

            case PBXTypes.PBXCopyFilesBuildPhase:
                _allObjects.Add(key, new PBXCopyFilesBuildPhase(key, section));
                break;

            case PBXTypes.PBXFileReference:
                _allObjects.Add(key, new PBXFileReference(key, section));
                break;

            case PBXTypes.PBXFrameworksBuildPhase:
                _allObjects.Add(key, new PBXFrameworksBuildPhase(key, section));
                break;

            case PBXTypes.PBXGroup:
                _allObjects.Add(key, new PBXGroup(key, section));
                break;

            case PBXTypes.PBXNativeTarget:
                _allObjects.Add(key, new PBXNativeTarget(key, section));
                break;

            case PBXTypes.PBXProject:
                _allObjects.Add(key, new PBXProject(key, section));
                break;

            case PBXTypes.PBXResourcesBuildPhase:
                _allObjects.Add(key, new PBXResourcesBuildPhase(key, section));
                break;

            case PBXTypes.PBXShellScriptBuildPhase:
                _allObjects.Add(key, new PBXShellScriptBuildPhase(key, section));
                break;

            case PBXTypes.PBXSourcesBuildPhase:
                _allObjects.Add(key, new PBXSourcesBuildPhase(key, section));
                break;

            case PBXTypes.PBXTargetDependency:
                _allObjects.Add(key, new PBXTargetDependency(key, section));
                break;

            case PBXTypes.PBXVariantGroup:
                _allObjects.Add(key, new PBXVariantGroup(key, section));
                break;

            case PBXTypes.XCBuildConfiguration:
                _allObjects.Add(key, new XCBuildConfiguration(key, section));
                break;

            case PBXTypes.XCConfigurationList:
                _allObjects.Add(key, new XCConfigurationList(key, section));
                break;

            case PBXTypes.XCVersionGroup:
                _allObjects.Add(key, new XCVersionGroup(key, section));
                break;

            case PBXTypes.PBXHeadersBuildPhase:
                _allObjects.Add(key, new PBXHeadersBuildPhase(key, section));
                break;

            default:
                return;
            }
        }

        #endregion

        #region Project navigation

        PBXProject RootProject
        {
            get;
            set;
        }

        PBXNativeTarget MainTarget
        {
            get;
            set;
        }

        #endregion

        #region Frameworks

        PBXFileReference AddFramework(string path, AddMethod addMethod, LinkType linkType, bool isSystemFramework)
        {
            //First confirm the path and extract the name of the lib
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("EgoXproject: Framework path is null");
                return null;
            }

            PBXFileType fileType = PBXFileTypeHelper.FileTypeFromFileName(path);

            if (!PBXFileTypeHelper.IsFrameworkOrLibrary(fileType))
            {
                Debug.LogError("EgoXproject: Not a valid framework or library: " + path);
                return null;
            }

            //frameworks appear as files, but are really directories, so we have to handle that here
            if (!isSystemFramework)
            {
                //copy if required
                if (addMethod == AddMethod.Copy)
                {
                    var destPath = Path.Combine(PathToXcodeProject, Path.GetFileName(path));

                    if (!CopyToDestination(path, destPath))
                    {
                        return null;
                    }

                    //update path to the new location
                    path = destPath;
                }

                //make relative to the project
                path = PathRelativeToProjectPath(path);
            }

            //third, find an existing file ref or create if not
            var frameworkBuildPhases = MainTarget.FrameworksBuildPhases;
            PBXFileReference fileRef = null;
            PBXBuildFile buildFile = null;

            //if no framework build phase add one
            if (frameworkBuildPhases == null || frameworkBuildPhases.Length <= 0)
            {
                var fbp = PBXFrameworksBuildPhase.Create(GenerateUID());
                AddObject(fbp);
                MainTarget.AddBuildPhase(fbp);
                //refresh
                frameworkBuildPhases = MainTarget.FrameworksBuildPhases;
            }

            bool found = FindBuildFileAndFileReference(frameworkBuildPhases, path, out buildFile, out fileRef);

            //add fileref and buildfile, and frameworks build phase if missing
            if (!found)
            {
                //find group for fileref
                var frameworkGroup = RootProject.MainGroup.GetGroupWithName("Frameworks", true);

                if (frameworkGroup == null)
                {
                    frameworkGroup = PBXGroup.CreateWithName(GenerateUID(), "Frameworks", RootProject.MainGroup);
                    AddObject(frameworkGroup);
                }

                //create the appropriate file ref
                if (isSystemFramework)
                {
                    fileRef = PBXFileReference.CreateRelativeToSdkRoot(GenerateUID(), path, frameworkGroup);
                }
                else
                {
                    //TODO make this group. will mean changing all demo project sample files.
                    fileRef = PBXFileReference.CreateRelativeToSourceRoot(GenerateUID(), path, frameworkGroup);
                    //                  fileRef = PBXFileReference.CreateRelativeToGroup(GenerateUID(), Path.GetFileName(path), framworkGroup);
                }

                AddObject(fileRef);
                //create build file
                buildFile = PBXBuildFile.Create(GenerateUID(), fileRef);
                AddObject(buildFile);
                //add to first framework build phase. most likely will only be one.
                frameworkBuildPhases[0].AddFile(buildFile);
                //add file ref to the framework group
                frameworkGroup.AddChild(fileRef);
            }

            //set link type
            switch (linkType)
            {
            case LinkType.Optional:
                buildFile.AddAttribute (PBXBuildFile.WEAK_KEY);
                break;

            case LinkType.Required:
            default:
                buildFile.RemoveAttribute (PBXBuildFile.WEAK_KEY);
                break;
            }

            //finally, add the path to the framework/library search path if external file
            if (!isSystemFramework)
            {
                string searchPath = Path.Combine(SRC_ROOT, Path.GetDirectoryName(path));

                foreach (var buildConfig in MainTarget.BuildConfigurationList.BuildConfigurations)
                {
                    if (fileType == PBXFileType.WrapperFramework)
                    {
                        buildConfig.AddFrameworkSearchPath(searchPath);
                    }
                    else
                    {
                        buildConfig.AddLibrarySearchPath(searchPath);
                    }
                }
            }

            return fileRef;
        }

        #endregion

        #region Files and folders

        /// <summary>
        /// Adds the folder.
        /// </summary>
        /// <param name="entry">Entry. the folder entry to add. will contain files and folders, but not every item in the folders</param>
        /// <param name="groupPath">Group Path. The group the folder should be added to. null means it will form a top level group.</param>
        void AddFolder(FolderEntry entry, GroupPath groupPath)
        {
            if (string.IsNullOrEmpty(entry.Path))
            {
                Debug.LogError("EgoXproject: folder path cannot be null or empty");
                return;
            }

            if (groupPath == null)
            {
                var groupRootPath = "";

                if (entry.Add == AddMethod.Copy)
                {
                    groupRootPath = entry.FileName;
                }
                else
                {
                    groupRootPath = PathRelativeToProjectPath(entry.Path);
                }

                groupPath = new GroupPath(groupRootPath);
            }
            else
            {
                groupPath = new GroupPath(groupPath);
                groupPath.AddComponentToSubPath(entry.FileName);
            }

            //we don't copy the whole dir across, only the files and folders that are in the FolderEntry

            //work through the remaining files and folders
            foreach (var e in entry.Entries)
            {
                AddFileOrFolder(e, groupPath);
            }
        }

        /// <summary>
        /// Adds the file or container.
        /// </summary>
        /// <param name="entry">File info</param>
        /// <param name="groupPath">Group path. which group the file should be added to</param>
        void AddFileOrContainer(BaseFileEntry entry, GroupPath groupPath)
        {
            if (string.IsNullOrEmpty(entry.Path))
            {
                Debug.LogError("EgoXproject: file path cannot be null or empty");
                return;
            }

            string path = Path.GetFullPath(entry.Path);

            if (!FileAndFolderEntryFactory.Exists(path))
            {
                Debug.LogError("EgoXproject: " + path + " does not exist. Skipping.");
                return;
            }

            if (groupPath == null)
            {
                groupPath = new GroupPath();
            }

            //copy the file to the target location if required
            if (entry.Add == AddMethod.Copy)
            {
                string destPath = Path.Combine(Path.Combine(PathToXcodeProject, groupPath.RelativePath), Path.GetFileName(path));

                if (!CopyToDestination(path, destPath))
                {
                    return;
                }

                path = destPath;
            }

            string fullPath = path;
            string relativePath = PathRelativeToProjectPath(path);
            PBXFileType fileType = PBXFileTypeHelper.FileTypeFromFileName(relativePath);

            if (entry is SourceFileEntry)
            {
                var srcEntry = entry as SourceFileEntry;

                //add source files to compile sources build phase
                if (PBXFileTypeHelper.IsCoreDatamodel(fileType))        //have to check this first because it is a special case
                {
                    //check for erroroneous entry in filerefs and remove if found
                    //then add the core data model
                    AddCoreDataModel(relativePath, fullPath, groupPath, srcEntry.CompilerFlags);
                }
                else if (PBXFileTypeHelper.IsSourceCodeFile(fileType))
                {
                    AddSourceFile(relativePath, groupPath, srcEntry.CompilerFlags);
                }
            }
            else if (entry is FileEntry)
            {
                //TODO is there a better way of doing this. entitlements files are just xml files.
                if (PBXFileTypeHelper.IsEntitlementsFile(relativePath))
                {
                    AddNotBuiltFile(relativePath, groupPath);
                }
                else if (PBXFileTypeHelper.IsHeaderFile(fileType))
                {
                    AddHeaderFile(relativePath, groupPath);
                }
                else if (PBXFileTypeHelper.IsResourceFile(fileType))
                {
                    AddResourceFile(relativePath, groupPath);
                }
                else if (PBXFileTypeHelper.IsFrameworkOrLibrary(fileType))
                {
                    //should not occur. but in case
                    AddCustomFramework(path, AddMethod.Link, LinkType.Required);
                }
                else
                {
                    AddNotBuiltFile(relativePath, groupPath);
                }
            }
        }

        PBXSourcesBuildPhase[] SourcesBuildPhases()
        {
            var srcBuildPhases = MainTarget.SourcesBuildPhases;

            //create source build phase if missing
            if (srcBuildPhases == null || srcBuildPhases.Length <= 0)
            {
                var srcBuildPhase = PBXSourcesBuildPhase.Create(GenerateUID());
                AddObject(srcBuildPhase);
                MainTarget.AddBuildPhase(srcBuildPhase);
                srcBuildPhases = MainTarget.SourcesBuildPhases;
            }

            return srcBuildPhases;
        }

        PBXResourcesBuildPhase[] ResourcesBuildPhases()
        {
            var resBuildPhases = MainTarget.ResourcesBuildPhases;

            //create if none
            if (resBuildPhases == null || resBuildPhases.Length <= 0)
            {
                var buildPhase = PBXResourcesBuildPhase.Create(GenerateUID());
                AddObject(buildPhase);
                MainTarget.AddBuildPhase(buildPhase);
                resBuildPhases = MainTarget.ResourcesBuildPhases;
            }

            return resBuildPhases;
        }

        void AddSourceFile(string relativePath, GroupPath groupPath, string compilerFlags)
        {
            var srcBuildPhases = SourcesBuildPhases();
            PBXBuildFile buildFile = null;
            AddFileToBuildPhase(srcBuildPhases, relativePath, groupPath, out buildFile);
            buildFile.CompilerFlags = compilerFlags;
        }

        void AddResourceFile(string relativePath, GroupPath groupPath)
        {
            var resBuildPhases = ResourcesBuildPhases();
            PBXBuildFile buildFile = null;
            AddFileToBuildPhase(resBuildPhases, relativePath, groupPath, out buildFile);
        }

        void AddHeaderFile(string relativePath, GroupPath groupPath)
        {
            var headerBuildPhases = MainTarget.HeadersBuildPhases;

            //don't create, as not normal to have in application, only in lib projects
            if (headerBuildPhases == null || headerBuildPhases.Length <= 0)
            {
                AddNotBuiltFile(relativePath, groupPath);
            }
            else
            {
                PBXBuildFile buildFile = null;
                AddFileToBuildPhase(headerBuildPhases, relativePath, groupPath, out buildFile);
            }
        }

        void AddNotBuiltFile(string relativePath, GroupPath groupPath)
        {
            var fileRef = FindFileReference(relativePath);

            if (fileRef != null)
            {
                return;
            }

            var grp = GetGroup(groupPath);
            //TODO add to source root? or relative to group folder?
            //fileRef = PBXFileReference.CreateRelativeToSourceRoot(GenerateUID(), relativePath, grp);
            //TODO how best to set the path. should just be the filename.
            var grpPath = grp.PathRelativeToGroup(relativePath);
            fileRef = PBXFileReference.CreateRelativeToGroup(GenerateUID(), grpPath, grp);
            AddObject(fileRef);
            grp.AddChild(fileRef);
        }

        void AddFileToBuildPhase<T>(T[] buildPhases, string relativePath, GroupPath groupPath, out PBXBuildFile buildFile) where T : PBXBaseBuildPhase
        {
            //find existing ref to this file
            PBXFileReference fileRef = null;
            buildFile = null;

            //create if not found
            if (!FindBuildFileAndFileReference(buildPhases, relativePath, out buildFile, out fileRef))
            {
                var grp = GetGroup(groupPath);
                //TODO add to source root? or relative to group folder?
                //fileRef = PBXFileReference.CreateRelativeToSourceRoot(GenerateUID(), relativePath, grp);
                //TODO how best to set the path. should just be the filename.
                var grpPath = grp.PathRelativeToGroup(relativePath);
                //Path.GetFileName(relativePath)
                fileRef = PBXFileReference.CreateRelativeToGroup(GenerateUID(), grpPath, grp);
                AddObject(fileRef);
                grp.AddChild(fileRef);
                buildFile = PBXBuildFile.Create(GenerateUID(), fileRef);
                AddObject(buildFile);
                buildPhases[0].AddFile(buildFile);
            }
        }

        bool FindBuildFileAndFileReference<T>(T[] buildPhases, string relativePath, out PBXBuildFile buildFile, out PBXFileReference fileRef) where T : PBXBaseBuildPhase
        {
            buildFile = null;
            fileRef = null;

            //find the fileref if it exists
            foreach (var phase in buildPhases)
            {
                foreach (var bf in phase.Files)
                {
                    if (bf.FileReference != null)
                    {
                        if (bf.FileReference.FullPath == relativePath)
                        {
                            fileRef = bf.FileReference;
                            buildFile = bf;
                            return true;
                        }
                    }
                    else if (bf.VariantGroup != null)
                    {
                        foreach (var c in bf.VariantGroup.ChildFileReferences)
                        {
                            if (c.FullPath == relativePath)
                            {
                                fileRef = c;
                                buildFile = bf;
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        PBXFileReference FindFileReference(string relativePath)
        {
            var fileRefs = _allObjects.Values.OfType<PBXFileReference>();
            return fileRefs.Where(o => o.FullPath == relativePath).FirstOrDefault();
        }

        #endregion

        #region Groups

        //TODO test
        /// <summary>
        /// Gets the group with path.
        /// Path is the relative path to the group eg a/b/c or ../a/b/c
        /// RootPath is the section of the path that forms the first group, eg a, or ../a/b
        /// </summary>
        /// <returns>The group with path.</returns>
        /// <param name="groupPath">Group Path.</param>
        PBXGroup GetGroup(GroupPath groupPath)
        {
            var currentGroup = RootProject.MainGroup;
            var components = groupPath.PathComponents;

            if (components.Length <= 0)
            {
                return currentGroup;
            }

            foreach (var c in components)
            {
                var groups = currentGroup.ChildGroups;
                PBXGroup grp = groups.Where(o => o.Path == c).FirstOrDefault();

                if (grp != null)
                {
                    currentGroup = grp;
                }
                else
                {
                    //create group
                    grp = PBXGroup.CreateWithPath(GenerateUID(), c, currentGroup);
                    AddObject(grp);
                    currentGroup.AddChild(grp);
                    currentGroup = grp;
                }
            }

            return currentGroup;
        }

        #endregion


        #region Utils

#if DETERMINISTIC
        string[] _guidPool;
        bool _guidFirstCall = true;
        bool _useDeterministicGuids = false;
        int _guidIndex = 0;

        bool LoadGuids()
        {
            string path = "Assets/guids.txt";

            if (!File.Exists(path))
            {
                return false;
            }

            _guidPool = File.ReadAllLines(path);
            _guidIndex = 0;
            return (_guidPool != null && _guidPool.Length > 0);
        }

        string GenerateUID()
        {
            if (_guidFirstCall)
            {
                _guidFirstCall = false;
                _useDeterministicGuids = LoadGuids();
                Debug.LogWarning(_useDeterministicGuids ? "EgoXproject: Using deterministic GUIDS" : "Using normal GUIDs");
            }

            if (_useDeterministicGuids)
            {
                if (_guidIndex >= _guidPool.Length)
                {
                    _useDeterministicGuids = false;
                    Debug.LogWarning("EgoXproject: GUID pool exhausted");
                    return NormalGenerateUID();
                }

                string uid;

                do
                {
                    uid = _guidPool[_guidIndex];
                    _guidIndex++;
                }
                while (_usedUids.Contains(uid));

                _usedUids.Add(uid);
                return uid;
            }
            else
            {
                return NormalGenerateUID();
            }
        }

        string NormalGenerateUID()
#else
        string GenerateUID()
#endif
        {
            string uid;

            do
            {
                uid = System.Guid.NewGuid().ToString();
                uid = uid.Replace("-", "");
                uid = uid.Substring(0, 24);
                uid = uid.ToUpper();
            }
            while (_usedUids.Contains(uid));

            _usedUids.Add(uid);
            return uid;
        }

        void AddObject(PBXBaseObject obj)
        {
            if (obj == null)
            {
                return;
            }

            if (_allObjects.ContainsKey(obj.UID))
            {
                throw new System.ArgumentException("Trying to add an object with the same UID as an existing object: " + obj.UID);
            }

            _allObjects.Add(obj.UID, obj);
            ObjectsDictionary.Add(obj.UID, obj.Dict);
        }

        PBXProjDictionary ObjectsDictionary
        {
            get
            {
                return _dict.DictionaryValue(OBJECTS_KEY);
            }
        }

        string PathRelativeToProjectPath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.GetFullPath(path);
            }

            string currentPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(PathToXcodeProject);
            var rel = PathUtil.MakePathRelativeToRootPath(path, PathToXcodeProject);
            Directory.SetCurrentDirectory(currentPath);
            return rel;
        }

        #endregion

        #region file and directory management

        // copy a file or directory to the specified location. 
        // eg cp /path/to/src/file.m /path/to/dest/file.m
        bool CopyToDestination(string sourcePath, string destPath)
        {
            if (!FileAndFolderEntryFactory.Exists(sourcePath))
            {
                Debug.LogError("EgoXproject: " + sourcePath + " does not exist");
                return false;
            }

            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }
            else if (Directory.Exists(destPath))
            {
                Directory.Delete(destPath, true);
            }

            if (File.Exists(sourcePath))
            {
                //create the dir if required
                Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                File.Copy(sourcePath, destPath);
            }
            else
            {
                return DirectoryCopy(sourcePath, destPath);
            }

            return true;
        }

        bool DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                Debug.LogError("EgoXproject: Source directory does not exist or could not be found: " + sourceDirName);
                return false;
            }

            if (File.Exists(destDirName))
            {
                Debug.LogError("EgoXproject: Cannot copy dir as file exists at destination: " + destDirName);
                return false;
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                //skip invalid files
                if (IgnoredFiles.ShouldIgnore(file.FullName))
                {
                    continue;
                }

                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // copy subdirectories and their contents to new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                //skip invalid files
                if (IgnoredFiles.ShouldIgnore(subdir.FullName))
                {
                    continue;
                }

                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }

            return true;
        }


        #endregion

        #region core data

        /// <summary>
        /// Adds the core data model.
        /// some xcode modifiers will wreck core data entries and place them incorrectly in the
        /// file ref section meaning the included model is not accessable or known about by Xcode.
        /// So we have to find the bad entries and remove them, and then add the correct entries
        /// </summary>
        /// <param name="relativePath">Path relative to xcode project.</param>
        /// <param name="fullPath">Full path to path.</param>
        /// <param name="groupPath">Group path. group to place the file in</param>
        void AddCoreDataModel(string relativePath, string fullPath, GroupPath groupPath, string attributes)
        {
            if (!Directory.Exists(fullPath))
            {
                Debug.LogError("EgoXproject: Core data model does not exist");
                return;
            }

            /*
             * Structure
             * Model.xcdatamodeld
             *  |- Modelv1.xcdatamodel
             *  |   \- contents
             *  |- Modelv2.xcdatamodel
             *  |   \- contents
             *  \- .xccurrentversion
             */
            var srcBuildPhases = SourcesBuildPhases();
            PBXBuildFile buildFile = null;
            PBXFileReference fileRef = null;
            //first need to check if has been added to fileref
            bool found = FindBuildFileAndFileReference(srcBuildPhases, relativePath, out buildFile, out fileRef);

            if (found)
            {
                //Debug.Log("EgoXproject: Removing core data file ref entry " + fileRef.UID);
                RemoveFileRef(fileRef, buildFile);
            }

            //check for entry in XCVersionGroup list
            XCVersionGroup versionGroup = null;

            if (!FindBuildFileAndXCVersionGroup(srcBuildPhases, relativePath, out buildFile, out versionGroup))
            {
                var grp = GetGroup(groupPath);
                var grpPath = grp.PathRelativeToGroup(relativePath);
                versionGroup = XCVersionGroup.Create(GenerateUID(), grpPath, grp);
                AddObject(versionGroup);
                grp.AddChild(versionGroup);
                buildFile = PBXBuildFile.Create(GenerateUID(), versionGroup);
                buildFile.CompilerFlags = attributes;
                AddObject(buildFile);
                srcBuildPhases[0].AddFile(buildFile);
            }

            //now find and add the actual models
            versionGroup.RemoveAllChildren();
            string currentVersionFileName = ".xccurrentversion";
            string currentVersionNameKey = "_XCCurrentVersionName";
            string currentVersion = "";

            try
            {
                //the file does not always exist
                var currentVersionPlist = new PList(Path.Combine(fullPath, currentVersionFileName));
                currentVersion = currentVersionPlist.Root.StringValue(currentVersionNameKey);
            }
            catch
            {
                //this is the intended behaviour
            }

            var models = Directory.GetDirectories(fullPath);

            foreach (var model in models)
            {
                string modelFileName = Path.GetFileName(model);
                string modelRelativePath = PathRelativeToProjectPath(model);
                PBXFileReference modelFileRef = FindFileReference(modelRelativePath);

                if (modelFileRef == null)
                {
                    modelFileRef = PBXFileReference.CreateRelativeToGroup(GenerateUID(), modelFileName, versionGroup);
                    AddObject(modelFileRef);
                }

                versionGroup.AddChild(modelFileRef);

                if (modelFileName == currentVersion)
                {
                    versionGroup.CurrentVersionID = modelFileRef.UID;
                }
            }

            //set it to the first child in the group if not specified
            if (string.IsNullOrEmpty(versionGroup.CurrentVersionID) && versionGroup.ChildCount > 0)
            {
                versionGroup.CurrentVersionID = versionGroup.ChildrenIDs[0];
            }
        }

        void RemoveFileRef(PBXFileReference fileRef, PBXBuildFile buildFile)
        {
            if (buildFile != null)
            {
                bool removeBuildFile = true;
                var variantGroup = buildFile.VariantGroup;

                if (variantGroup != null)
                {
                    variantGroup.RemoveChild(fileRef);

                    if (variantGroup.ChildCount > 0)
                    {
                        //don't delete if more varants exist
                        removeBuildFile = false;
                    }
                    else
                    {
                        //remove variant group and its references
                        RemoveObject(variantGroup);
                        var grp = RootProject.MainGroup.GroupContainingObject(variantGroup);

                        if (grp != null)
                        {
                            grp.RemoveChild(variantGroup);
                        }
                    }
                }

                if (removeBuildFile)
                {
                    //will remove refs to object and their entries in the dic
                    MainTarget.RemoveBuildFile(buildFile);
                    RemoveObject(buildFile);
                }
            }

            RemoveObject(fileRef);
            var g = RootProject.MainGroup.GroupContainingObject(fileRef);

            if (g != null)
            {
                g.RemoveChild(fileRef);
            }
        }

        void RemoveObject(PBXBaseObject obj)
        {
            _allObjects.Remove(obj.UID);
            ObjectsDictionary.Remove(obj.UID);
        }

        bool FindBuildFileAndXCVersionGroup(PBXSourcesBuildPhase[] buildPhases, string relativePath, out PBXBuildFile buildFile, out XCVersionGroup versionGroup)
        {
            buildFile = null;
            versionGroup = null;

            //find the fileref if it exists
            foreach (var phase in buildPhases)
            {
                foreach (var bf in phase.Files)
                {
                    if (bf.VersionGroup == null)
                    {
                        continue;
                    }

                    if (bf.VersionGroup.FullPath == relativePath)
                    {
                        versionGroup = bf.VersionGroup;
                        buildFile = bf;
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Signing

        public void SetTeamId(string teamId)
        {
            if (string.IsNullOrEmpty(teamId))
            {
                return;
            }

            AddStringBuildSetting("DEVELOPMENT_TEAM", teamId);
            RootProject.SetDevelopmentTeam(MainTarget.UID, teamId);
        }

        public void EnableAutomaticProvisioning(bool enable)
        {
            RootProject.EnableAutomaticProvisioning(MainTarget.UID, enable);
        }

        #endregion

        #region Capabilities

        public void EnableSystemCapability(string capabilityKey, bool enabled)
        {
            if (!string.IsNullOrEmpty (capabilityKey))
            {
                RootProject.EnableSystemCapability (MainTarget.UID, capabilityKey, enabled);
            }
        }

        #endregion

        #region Entitlements

        public void SetEntitlementsFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new System.ArgumentNullException(nameof(path), "Path cannot be null or empty");
            }

            foreach (var existingPath in EntitlementsFilePaths)
            {
                if (existingPath == path)
                {
                    return;
                }
            }

            var relPath = PathUtil.MakePathRelativeToRootPath(path, PathToXcodeProject);
            var fileRef = PBXFileReference.CreateRelativeToSourceRoot(GenerateUID(), relPath, RootProject.MainGroup);
            AddObject(fileRef);
            RootProject.MainGroup.AddChild(fileRef);
            EntitlementsFilePaths = new string[] { path };

            foreach (var buildConfig in MainTarget.BuildConfigurationList.BuildConfigurations)
            {
                buildConfig.AddStringBuildSetting("CODE_SIGN_ENTITLEMENTS", relPath);
            }
        }

        #endregion

        public string ProductName
        {
            get
            {
                string name = "";
                string releaseName = "";

                foreach (var buildConfig in MainTarget.BuildConfigurationList.BuildConfigurations)
                {
                    var productName = buildConfig.StringForKey("PRODUCT_NAME");

                    if (string.IsNullOrEmpty(productName))
                    {
                        continue;
                    }

                    if (buildConfig.Name == "Release")
                    {
                        releaseName = productName;
                    }
                    else
                    {
                        name = productName;
                    }
                }

                if (!string.IsNullOrEmpty(releaseName))
                {
                    return releaseName;
                }

                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }

                return "ProductName";
            }
        }
    }
}