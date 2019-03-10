using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Egomotion.EgoXproject;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;
using System.IO;
using UnityEngine.TestTools;

namespace Egomotion.EgoXprojectTests.PBXProjTests
{
    [TestFixture]
    public class PBXProjTest
    {
        string _testFilesPath;
        string _sampleDataPath;
        const string ORIGINAL_PROJECT_FILE_NAME = "original_project.pbxproj";
        const string TEST_COPY_FILE_NAME = "test-copy.pbxproj";

        [SetUp]
        public void SetUp()
        {
            _sampleDataPath = Path.Combine(Application.dataPath, "egomotion-tests/egoXproject/SampleData");
            _testFilesPath = Path.Combine(_sampleDataPath, "SampleXcodeProject/Unity-iPhone.xcodeproj");
        }

        [TearDown]
        public void TearDown()
        {
            CleanUpCopy();
        }

        PBXProj LoadCopyOfOriginal()
        {
            return LoadCopyOfPBXProjFile(ORIGINAL_PROJECT_FILE_NAME);
        }

        PBXProj LoadCopyOfPBXProjFile(string fileName)
        {
            CleanUpCopy();
            string originalFile = Path.Combine(_testFilesPath, fileName);
            string testFileCopy = Path.Combine(_testFilesPath, TEST_COPY_FILE_NAME);
            File.Copy(originalFile, testFileCopy);
            var pbx = new PBXProj();
            bool success = pbx.Load(testFileCopy);
            Assert.IsTrue(success);
            return pbx;
        }

        void CleanUpCopy()
        {
            string testFileCopy = Path.Combine(_testFilesPath, TEST_COPY_FILE_NAME);

            if (File.Exists(testFileCopy))
            {
                File.Delete(testFileCopy);
            }
        }

        void CompareFiles(string expectedFileName, string actualFilePath)
        {
            string expectedFilePath = Path.Combine(_testFilesPath, expectedFileName);
            var expected = File.ReadAllText(expectedFilePath);
            var actual = File.ReadAllText(actualFilePath);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LoadSave()
        {
            var pbx = LoadCopyOfOriginal();
            pbx.Save();
            CompareFiles(ORIGINAL_PROJECT_FILE_NAME, pbx.SavePath);
        }

        [Test]
        public void AddSystemFramework()
        {
            var pbx = LoadCopyOfOriginal();
            pbx.AddSystemFramework("CoreData.framework", LinkType.Required);
            pbx.AddSystemFramework("System/Library/Frameworks/HomeKit.framework", LinkType.Optional);
            pbx.AddSystemFramework("libxml2.tbd", LinkType.Required);
            pbx.AddSystemFramework("usr/lib/libz.tbd", LinkType.Required);
            pbx.Save();
            CompareFiles("AddSystemFramework.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddBuildSettings()
        {
            var pbx = LoadCopyOfOriginal();
            //bool
            pbx.AddBoolBuildSetting("GCC_WARN_UNUSED_VALUE", false);
            pbx.AddBoolBuildSetting("CUSTOM_BOOL", false);
            //enum
            pbx.AddEnumBuildSetting("STRIP_STYLE", "debugging");
            pbx.AddEnumBuildSetting("CUSTOM_ENUM", "foo");
            //string
            pbx.AddStringBuildSetting("BUNDLE_LOADER", "AFakeEntry");
            pbx.AddStringBuildSetting("CUSTOM_STRING", "custom string");
            //array
            pbx.AddCollectionBuildSetting("ALTERNATE_PERMISSIONS_FILES", new string[] {"fileA", "\"File B\"", "dir/file-C", "dir/dir 2/file d"}, MergeMethod.Append);
            //string list
            pbx.AddCollectionBuildSetting("INCLUDED_RECURSIVE_SEARCH_PATH_SUBDIRECTORIES", new string[] {"subdirA", "subDir b", "subdir-C"}, MergeMethod.Append);
            //custom collection
            pbx.AddCollectionBuildSetting("CUSTOM_ARRAY", new string[] {"SOME", "test variables", "\"with different\"", "_values-$"}, MergeMethod.Replace);
            //path
            pbx.AddStringBuildSetting("CLANG_OPTIMIZATION_PROFILE_FILE", "some/path/to a/file.profdata");
            //path string list
            pbx.AddCollectionBuildSetting("USER_HEADER_SEARCH_PATHS", new string[] {"a/path/to/search", "another/path to/search", "\"a/final/path to-search/\""}, MergeMethod.Append);
            //path array
            pbx.AddCollectionBuildSetting("REZ_SEARCH_PATHS", new string[] {"rez/path/to/search", "another/rez/path to/search", "\"a/final/rez/path to-search/\""}, MergeMethod.Append);
            //inherited
            pbx.AddCollectionBuildSetting("LD_RUNPATH_SEARCH_PATHS", new string[] {"a/path/to/search", "another/path to/search", "\"a/final/path to-search/\""}, MergeMethod.Append); //string list
            pbx.Save();
            CompareFiles("AddBuildSettings.pbxproj", pbx.SavePath);
        }

        [Test]
        public void UpdateBuildSettings()
        {
            var pbx = LoadCopyOfPBXProjFile("UpdateBuildSettings_orignal.pbxproj");
            //bool
            pbx.AddBoolBuildSetting("GCC_WARN_UNUSED_VALUE", true);
            pbx.AddBoolBuildSetting("CUSTOM_BOOL", true);
            //enum
            pbx.AddEnumBuildSetting("STRIP_STYLE", "non-global");
            pbx.AddEnumBuildSetting("CUSTOM_ENUM", "bar");
            //string
            pbx.AddStringBuildSetting("BUNDLE_LOADER", "UpdatedEntry");
            pbx.AddStringBuildSetting("CUSTOM_STRING", "Updated string");
            //array
            pbx.AddCollectionBuildSetting("ALTERNATE_PERMISSIONS_FILES", new string[] {"fileZ", "fileA", "\"File B\"", "dir/file - C"}, MergeMethod.Append);
            //string list
            pbx.AddCollectionBuildSetting("INCLUDED_RECURSIVE_SEARCH_PATH_SUBDIRECTORIES", new string[] {"subdirA", "subDir z", "subdir -C"}, MergeMethod.Append);
            //path
            pbx.AddStringBuildSetting("CLANG_OPTIMIZATION_PROFILE_FILE", "updated/path/newfile.profdata");
            //path string list
            pbx.AddCollectionBuildSetting("USER_HEADER_SEARCH_PATHS", new string[] {"new/path/to/search", "another/new/path to/search"}, MergeMethod.Replace);
            //path array
            pbx.AddCollectionBuildSetting("REZ_SEARCH_PATHS", new string[] {"new/rez/path/to/search", "another/new/rez/path to/search"}, MergeMethod.Replace);
            //inherited
            pbx.AddCollectionBuildSetting("LD_RUNPATH_SEARCH_PATHS", new string[] {"a/new/path/to/search", "\"a/final/path to-search/\""}, MergeMethod.Append); //string list
            pbx.Save();
            CompareFiles("UpdateBuildSettings.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddBuildSettingsAsWrongType()
        {
            LogAssert.ignoreFailingMessages = true;

            var pbx = LoadCopyOfOriginal();
            //bool
            pbx.AddBoolBuildSetting("STRIP_STYLE", false);
            //enum
            pbx.AddEnumBuildSetting("GCC_WARN_UNUSED_VALUE", "debugging");
            //string
            pbx.AddStringBuildSetting("ALTERNATE_PERMISSIONS_FILES", "AFakeEntry");
            //collection
            pbx.AddCollectionBuildSetting("GCC_WARN_UNUSED_VALUE", new string[] {"a", "b", "c"}, MergeMethod.Append);
            pbx.Save();
            //should not have added anything
            CompareFiles(ORIGINAL_PROJECT_FILE_NAME, pbx.SavePath);
        }

        [Test]
        public void UpdateConditionalBuildSetting()
        {
            var pbx = LoadCopyOfPBXProjFile("UpdateConditionalBuildSetting_original.pbxproj");
            pbx.AddStringBuildSetting("CODE_SIGN_IDENTITY", "Some Developer");
            pbx.Save();
            CompareFiles("UpdateConditionalBuildSetting.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddScript()
        {
            var pbx = LoadCopyOfOriginal();
            pbx.AddScript("", "/bin/sh", "echo Hello World");
            pbx.Save();
            CompareFiles("AddScript.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddNamedScript()
        {
            var pbx = LoadCopyOfOriginal();
            pbx.AddScript("My script", "/bin/sh", "echo Hello World");
            pbx.Save();
            CompareFiles("AddNamedScript.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddResourceFile()
        {
            var pbx = LoadCopyOfOriginal();
            var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.pdf");
            var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
            Assert.IsNotNull(fileEntry);
            Assert.IsTrue(fileEntry is FileEntry);
            pbx.AddFileOrFolder(fileEntry);
            pbx.Save();
            CompareFiles("AddResourceFile.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddSourceFile()
        {
            //TODO add compile flags
            var pbx = LoadCopyOfOriginal();
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.m");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is SourceFileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.mm");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is SourceFileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.h");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is FileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.hpp");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is FileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.swift");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is SourceFileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.asm");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is SourceFileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.cpp");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is SourceFileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.cc");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is SourceFileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.c");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is SourceFileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.cxx");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is SourceFileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            {
                var path = Path.Combine(Application.dataPath, "Sample Data/Files/test.hxx");
                var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
                Assert.IsNotNull(fileEntry);
                Assert.IsTrue(fileEntry is FileEntry);
                pbx.AddFileOrFolder(fileEntry);
            }
            pbx.Save();
            CompareFiles("AddSourceFile.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddCustomFrameworkEntry()
        {
            var rootPath = Path.Combine(Application.dataPath, "Sample Data/Frameworks");
            var fwPath = Path.Combine(rootPath, "MyTest.framework");
            var pbx = LoadCopyOfOriginal();
            var fileEntry = FileAndFolderEntryFactory.Create(fwPath, AddMethod.Link);
            Assert.IsNotNull(fileEntry);
            Assert.IsTrue(fileEntry is FrameworkEntry);
            pbx.AddFileOrFolder(fileEntry);
            pbx.Save();
            CompareFiles("AddCustomFramework.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddCustomFramework()
        {
            var rootPath = Path.Combine(Application.dataPath, "Sample Data/Frameworks");
            var fwPath = Path.Combine(rootPath, "MyTest.framework");
            var pbx = LoadCopyOfOriginal();
            pbx.AddCustomFramework(fwPath, AddMethod.Link, LinkType.Required);
            pbx.Save();
            CompareFiles("AddCustomFramework.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddCustomStaticLibraryEntry()
        {
            var pbx = LoadCopyOfOriginal();
            var path = Path.Combine(Application.dataPath, "Sample Data/Frameworks/test.a");
            var fileEntry = FileAndFolderEntryFactory.Create(path, AddMethod.Link);
            Assert.IsNotNull(fileEntry);
            Assert.IsTrue(fileEntry is StaticLibraryEntry);
            (fileEntry as StaticLibraryEntry).Link = LinkType.Optional;
            pbx.AddFileOrFolder(fileEntry);
            pbx.Save();
            CompareFiles("AddCustomStaticLibrary.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddCustomStaticLibrary()
        {
            var pbx = LoadCopyOfOriginal();
            var path = Path.Combine(Application.dataPath, "Sample Data/Frameworks/test.a");
            pbx.AddCustomFramework(path, AddMethod.Link, LinkType.Optional);
            pbx.Save();
            CompareFiles("AddCustomStaticLibrary.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddEmbeddedFrameworkEntry()
        {
            var rootPath = Path.Combine(Application.dataPath, "Sample Data/Frameworks");
            var fwPath = Path.Combine(rootPath, "MyTest.framework");
            var pbx = LoadCopyOfOriginal();
            var destPath = Path.Combine(pbx.PathToXcodeProject, "MyTest.framework");
            long lastAccess = 0;

            if (File.Exists(destPath))
            {
                lastAccess = File.GetLastAccessTime(destPath).Ticks;
            }

            var entry = FileAndFolderEntryFactory.CreateFrameworkEntry(fwPath, AddMethod.Link, LinkType.Required, true);
            pbx.AddFileOrFolder(entry);
            pbx.Save();
            CompareFiles("AddEmbeddedFramework.pbxproj", pbx.SavePath);
            var updatedAccess = File.GetLastAccessTime(destPath).Ticks;
            Assert.Less(lastAccess, updatedAccess);
            Directory.Delete(destPath, true);
        }

        [Test]
        public void AddEmbeddedFramework()
        {
            var rootPath = Path.Combine(Application.dataPath, "Sample Data/Frameworks");
            var fwPath = Path.Combine(rootPath, "MyTest.framework");
            var pbx = LoadCopyOfOriginal();
            var destPath = Path.Combine(pbx.PathToXcodeProject, "MyTest.framework");
            long lastAccess = 0;

            if (File.Exists(destPath))
            {
                lastAccess = File.GetLastAccessTime(destPath).Ticks;
            }

            pbx.AddEmbeddedFramework(fwPath);
            pbx.Save();
            CompareFiles("AddEmbeddedFramework.pbxproj", pbx.SavePath);
            var updatedAccess = File.GetLastAccessTime(destPath).Ticks;
            Assert.Less(lastAccess, updatedAccess);
            Directory.Delete(destPath, true);
        }

        [Test]
        public void AddFolders()
        {
            var mainPath = Path.Combine(Application.dataPath, "Sample Data/Main Folder");
            var mainFolderEntry = new FolderEntry(mainPath, AddMethod.Link);
            var pbx = LoadCopyOfOriginal();
            pbx.AddFileOrFolder(mainFolderEntry);
            pbx.Save();
            CompareFiles("AddFolder.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddBundle()
        {
            var bundlePath = Path.Combine(Application.dataPath, "Sample Data/Files/Settings.bundle");
            var bundleFolderEntry = new FileEntry(bundlePath, AddMethod.Link);
            var pbx = LoadCopyOfOriginal();
            pbx.AddFileOrFolder(bundleFolderEntry);
            pbx.Save();
            CompareFiles("AddBundle.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AddExistingFile()
        {
            var filePath = Path.Combine(Application.dataPath, "Sample Data/Files/LaunchScreen-iPad.png");
            var fileEntry = new FileEntry(filePath, AddMethod.Copy);
            var pbx = LoadCopyOfOriginal();
            var destPath = Path.Combine(pbx.PathToXcodeProject, "LaunchScreen-iPad.png");
            long lastAccess = 0;

            if (File.Exists(destPath))
            {
                lastAccess = File.GetLastAccessTime(destPath).Ticks;
            }

            pbx.AddFileOrFolder(fileEntry);
            pbx.Save();
            CompareFiles(ORIGINAL_PROJECT_FILE_NAME, pbx.SavePath);
            var updatedAccess = File.GetLastAccessTime(destPath).Ticks;
            Assert.Less(lastAccess, updatedAccess);
            File.Delete(destPath);
        }

        [Test]
        public void AddExistingFolder()
        {
            var folderPath = Path.Combine(Application.dataPath, "Sample Data/Classes");
            var folderEntry = new FolderEntry(folderPath, AddMethod.Copy);
            var pbx = LoadCopyOfOriginal();
            var destPath = Path.Combine(pbx.PathToXcodeProject, "Classes/UnityAppController.mm");
            long lastAccess = 0;

            if (File.Exists(destPath))
            {
                lastAccess = File.GetLastAccessTime(destPath).Ticks;
            }

            pbx.AddFileOrFolder(folderEntry);
            pbx.Save();
            CompareFiles(ORIGINAL_PROJECT_FILE_NAME, pbx.SavePath);
            var updatedAccess = File.GetLastAccessTime(destPath).Ticks;
            Assert.Less(lastAccess, updatedAccess);
            Directory.Delete(Path.Combine(pbx.PathToXcodeProject, "Classes"), true);
        }

        [Test]
        public void AddTeamId()
        {
            string teamId = "ASDFGHJKL1";
            var pbx = LoadCopyOfOriginal();
            pbx.SetTeamId(teamId);
            pbx.Save();
            CompareFiles("AddTeamId.pbxproj", pbx.SavePath);
        }

        [Test]
        public void ReplaceTeamId()
        {
            string teamId = "QWERTYUIOP";
            var pbx = LoadCopyOfPBXProjFile("AddTeamId.pbxproj");
            pbx.SetTeamId(teamId);
            pbx.Save();
            CompareFiles("ReplaceTeamId.pbxproj", pbx.SavePath);
        }

        [Test]
        public void AutomaticProvisioning()
        {
            var pbx = LoadCopyOfOriginal();
            pbx.EnableAutomaticProvisioning(false);
            pbx.Save();
            CompareFiles("ManualProvisioning.pbxproj", pbx.SavePath);
            pbx.EnableAutomaticProvisioning(true);
            pbx.Save();
            CompareFiles(ORIGINAL_PROJECT_FILE_NAME, pbx.SavePath);
        }


        //datamodel
        //xcasssets
        //playground
        //other file types - e.g. ones not added to build process, range of types
    }
}
