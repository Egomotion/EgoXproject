using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Egomotion.EgoXproject.Internal;

namespace Egomotion.EgoXprojectTests.Utils
{
    [TestFixture]
    public class ProjectUtilTest
    {
        string _originalPath;

        [SetUp]
        public void SetUp()
        {
            _originalPath = System.Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.dataPath));
        }

        [TearDown]
        public void TearDown()
        {
            Directory.SetCurrentDirectory(_originalPath);
        }

        [Test]
        public void ProjectPath()
        {
            Assert.AreEqual(Path.GetDirectoryName(Application.dataPath), ProjectUtil.ProjectPath);
        }

        [Test]
        public void MakeRootedRelativeToProject()
        {
            string subdir = "Some/Sub/Dir";
            string path = Path.Combine(Application.dataPath, subdir);
            string relative = ProjectUtil.MakePathRelativeToProject(path);
            Assert.AreEqual(Path.Combine("Assets", subdir), relative);
        }


        [Test]
        public void MakeRelativePathRelativeToProject()
        {
            string relTestPath = "Assets/Foo/Bar";
            string rel = ProjectUtil.MakePathRelativeToProject(relTestPath);
            Assert.AreEqual(relTestPath, rel);
        }

        [Test]
        public void MakeRootedRelativeToAssets()
        {
            string subdir = "Some/Sub/Dir";
            string path = Path.Combine(Application.dataPath, subdir);
            string relative = ProjectUtil.MakePathRelativeToAssets(path);
            Assert.AreEqual(subdir, relative);
        }

        [Test]
        public void MakeRelativePathRelativeToAssets()
        {
            string relTestPath = "Assets/Foo/Bar";
            string rel = ProjectUtil.MakePathRelativeToAssets(relTestPath);
            Assert.AreEqual(Path.Combine("Foo", "Bar"), rel);
        }

        [Test]
        public void MakeRelativeExternalPathPRelativeToProject()
        {
            string path = "../../foo foo";
            string rel = ProjectUtil.MakePathRelativeToProject(path);
            Assert.AreEqual(rel, path);
        }

        [Test]
        public void MakeAbsoluteExternalPathPRelativeToProject()
        {
            string path = Directory.GetParent(Application.dataPath).Parent.Parent.FullName;
            path = Path.Combine(path, "foo foo");
            string rel = ProjectUtil.MakePathRelativeToProject(path);
            Assert.AreEqual(rel, "../../foo foo");
        }
    }
}