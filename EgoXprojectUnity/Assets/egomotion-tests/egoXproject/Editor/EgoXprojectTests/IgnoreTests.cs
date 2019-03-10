using UnityEngine;
using System.Collections;
using NUnit.Framework;
using Egomotion.EgoXproject.Internal;
using System.Text.RegularExpressions;
using System.Linq;

namespace Egomotion.EgoXprojectTests.IngoreFiles
{
    [TestFixture]
    public class IgnoreTests
    {
        [Test]
        public void IgnoreDefaultFiles()
        {
            string[] ignoredFiles = new string[]
            {
                ".git",
                ".svn",
                ".hg",
                "file.meta",
                ".DS_Store",
                "thumbs.db",
                "Thumbs.db",
                "file.bak",
                ".file~",
                "file~",
                "EgoXproject.dll",
                "egoxproject.settings",
                "egoxproject.configurations",
                "change.egoxc"
            };

            foreach (var item in ignoredFiles)
            {
                Assert.IsTrue(IgnoredFiles.ShouldIgnore(item));
            }

            foreach (var item in ignoredFiles)
            {
                Assert.IsTrue(IgnoredFiles.ShouldIgnore(item.ToLower()));
            }

            foreach (var item in ignoredFiles)
            {
                Assert.IsTrue(IgnoredFiles.ShouldIgnore(item.ToUpper()));
            }
        }

        [Test]
        public void DontIngoreFiles()
        {
            //".git", ".svn", ".hg", ".meta", ".DS_Store", "thumbs.db", "Thumbs.db", "*.bak", ".*~", "*~";
            string[] files = new string[]
            {
                "file.c",
                "file.cpp",
                "file",
                "file.framework",
                "lib.a",
                ".file",
                "file.hg.c",
                "file.meta.jpg",
                "file.bak.png",
                "file.git"
            };

            foreach (var item in files)
            {
                Assert.IsFalse(IgnoredFiles.ShouldIgnore(item));
            }
        }

        [Test]
        public void Custom()
        {
            string[] custom = new string[] { "*.foo", "a*b.test", "test.ego", "any*" };
            IgnoredFiles.SetIngnoredFiles(custom);
            string[] ignore = new string[]
            {
                ".foo",
                "file.foo",
                "asdb.test",
                "ab.test",
                "AvB.test",
                "test.ego",
                "anymore",
                "any",
                "any.bar"
            };
            string[] pass = new string[] { "a.foo.bar", "bacb.test", "a.test", "atest.ego", "test.egomotion", "manymore", "many" };

            foreach (var item in ignore)
            {
                Assert.IsTrue(IgnoredFiles.ShouldIgnore(item));
            }

            foreach (var item in pass)
            {
                Assert.IsFalse(IgnoredFiles.ShouldIgnore(item));
            }
        }
    }
}