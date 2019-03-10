using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;
using System.IO;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public abstract class BaseCapabilityTest
    {
        const string ORIGINAL_PROJECT_FILE_NAME = "original.pbxproj";
        const string TEST_COPY_PROJECT_FILE_NAME = "project.pbxproj";

        const string ORIGINAL_INFO_PLIST_FILE_NAME = "original.plist";
        const string TEST_COPY_INFO_PLIST_FILE_NAME = "Info.plist";

        const string ORIGINAL_ENTITLEMENTS_FILE_NAME = "original.entitlements";
        const string TEST_COPY_ENTITLEMENTS_FILE_NAME = "ProductName.entitlements";

        string _testProjectFilesPath;
        string _testInfoPlistFilesPath;
        string _testEntitlementsFilesPath;

        string OriginalPBXFilePath
        {
            get;
            set;
        }
        string OriginalInfoPlistFilePath
        {
            get;
            set;
        }
        string OriginalEntitlementsFilePath
        {
            get;
            set;
        }

        protected string XcodeProjectPath
        {
            get;
            private set;
        }
        protected string TestPBXFilePath
        {
            get;
            private set;
        }
        protected string TestInfoPlistFilePath
        {
            get;
            private set;
        }
        protected string TestEntitlementsFilePath
        {
            get;
            private set;
        }

        [SetUp]
        public void SetUp()
        {
            XcodeProjectPath = Path.Combine(Application.dataPath, "egomotion-tests/egoXproject/SampleData/CapabilitesProject");
            _testProjectFilesPath = Path.Combine(XcodeProjectPath, "Unity-iPhone.xcodeproj");
            _testInfoPlistFilesPath = XcodeProjectPath;
            _testEntitlementsFilesPath = XcodeProjectPath;
            OriginalPBXFilePath = Path.Combine(_testProjectFilesPath, ORIGINAL_PROJECT_FILE_NAME);
            OriginalInfoPlistFilePath = Path.Combine(_testInfoPlistFilesPath, ORIGINAL_INFO_PLIST_FILE_NAME);
            OriginalEntitlementsFilePath = Path.Combine(_testEntitlementsFilesPath, ORIGINAL_ENTITLEMENTS_FILE_NAME);
            TestPBXFilePath = Path.Combine(_testProjectFilesPath, TEST_COPY_PROJECT_FILE_NAME);
            TestInfoPlistFilePath = Path.Combine(_testInfoPlistFilesPath, TEST_COPY_INFO_PLIST_FILE_NAME);
            TestEntitlementsFilePath = Path.Combine(_testEntitlementsFilesPath, TEST_COPY_ENTITLEMENTS_FILE_NAME);
        }

        [TearDown]
        public void TearDown()
        {
            CleanUpProjectCopy();
            CleanUpEntitlementsCopy();
            CleanUpInfoPlistCopy();
        }

        protected void CreateOriginalCopies()
        {
            CleanUpCopy(TestPBXFilePath);
            File.Copy(OriginalPBXFilePath, TestPBXFilePath);
            CleanUpCopy(TestInfoPlistFilePath);
            File.Copy(OriginalInfoPlistFilePath, TestInfoPlistFilePath);
            CleanUpCopy(TestEntitlementsFilePath);
            File.Copy(OriginalEntitlementsFilePath, TestEntitlementsFilePath);
        }

        void CleanUpCopy(string testFileCopy)
        {
            if (File.Exists(testFileCopy))
            {
                File.Delete(testFileCopy);
            }
        }

        protected void CleanUpProjectCopy()
        {
            string testFileCopy = Path.Combine(_testProjectFilesPath, TEST_COPY_PROJECT_FILE_NAME);
            CleanUpCopy(testFileCopy);
        }

        protected void CleanUpInfoPlistCopy()
        {
            string testFileCopy = Path.Combine(_testInfoPlistFilesPath, TEST_COPY_INFO_PLIST_FILE_NAME);
            CleanUpCopy(testFileCopy);
        }

        protected void CleanUpEntitlementsCopy()
        {
            string testFileCopy = Path.Combine(_testEntitlementsFilesPath, TEST_COPY_ENTITLEMENTS_FILE_NAME);
            CleanUpCopy(testFileCopy);
        }

        protected void CompareProjectFiles(string expectedFileName, string actualFilePath)
        {
            string expectedFilePath = Path.Combine(_testProjectFilesPath, expectedFileName);
            CompareFiles(expectedFilePath, actualFilePath);
        }

        protected void CompareInfoPlistFiles(string expectedFileName, string actualFilePath)
        {
            string expectedFilePath = Path.Combine(_testInfoPlistFilesPath, expectedFileName);
            CompareFiles(expectedFilePath, actualFilePath);
        }

        protected void CompareEntitlementFiles(string expectedFileName, string actualFilePath)
        {
            string expectedFilePath = Path.Combine(_testEntitlementsFilesPath, expectedFileName);
            CompareFiles(expectedFilePath, actualFilePath);
        }

        void CompareFiles(string expectedFilePath, string actualFilePath)
        {
            var expected = File.ReadAllText(expectedFilePath);
            var actual = File.ReadAllText(actualFilePath);
            Assert.AreEqual(expected, actual);
        }
    }
}
