using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;
using System.IO;

namespace Egomotion.EgoXprojectTests.Utils
{
    [TestFixture]
    public class PathUtilsTests
    {
        Dictionary<string, string[]> _testPaths;

        [SetUp]
        public void Setup()
        {
            _testPaths = new Dictionary<string, string[]>()
            {
                { "a/b/c/d.m", new string[] { "a", "b", "c", "d.m" }
                },
                { "d.m", new string[] { "d.m" } },
                { "", new string[] { "" } },
                { "../a/b/c/d.m", new string[] { "../a", "b", "c", "d.m" } },
                { "a/b b/c/d.m", new string[] { "a", "b b", "c", "d.m" } },
                { ".n", new string[] { ".n" } },
                { "a/.n", new string[] { "a", ".n" } },
                { "a/.b/.n", new string[] { "a", ".b", ".n" } },
                { "a/../b/c.m", new string[] { "a", "../b", "c.m" } },
                { "../a/../b/c.m", new string[] { "../a", "../b", "c.m" } },
                { "./a/b.m", new string[] { "./a", "b.m" } },
                { "./a.m", new string[] { "./a.m" } },
                { "../a/./b/c.m", new string[] { "../a", "./b", "c.m" } },
                { "..", new string[] { ".." } },
                { "../", new string[] { ".." } },
                { ".", new string[] { "." } },
                { "./", new string[] { "." } }
            };
        }

        [Test]
        public void ComponentsFromPathTest()
        {
            foreach (var kvp in _testPaths)
            {
                var components = PathUtil.ComponentsFromPath(kvp.Key);
                Assert.AreEqual(components.Length, kvp.Value.Length);

                for (int ii = 0; ii < components.Length; ++ii)
                {
                    Assert.AreEqual(components[ii], kvp.Value[ii]);
                }
            }
        }

        [Test]
        public void PathFromComponentsTest()
        {
            foreach (var kvp in _testPaths)
            {
                var p = PathUtil.PathFromComponents(kvp.Value);

                if (kvp.Key == "../")
                {
                    Assert.AreEqual(p, "..");
                }
                else if (kvp.Key == "./")
                {
                    Assert.AreEqual(p, ".");
                }
                else
                {
                    Assert.AreEqual(p, kvp.Key);
                }
            }
        }

        //Mono's MakeRelativeUri is borked. Does not give the same results as the MS .net version
        //      [Test]
        //      public void MsdnUriTest()
        //      {
        //          List<string[]> uris = new List<string[]>();
        //          uris.Add(new string[] {"http://www.contoso.com/",
        //              "http://www.contoso.com/test/test.htm",
        //              "test/test.htm"});
        //
        //          uris.Add(new string[]{"http://www.contoso.com/test1/",
        //              "http://www.contoso.com/",
        //              "../"});
        //
        ////            uris.Add(new string[] {"http://www.contoso.com:8000/",
        ////                "http://www.contoso.com/test/test.htm",
        ////                "http://www.contoso.com/test/test.htm"});
        //
        //          uris.Add(new string[] {"http://username@www.contoso.com/",
        //              "http://www.contoso.com/test1/test1.txt",
        //              "test1/test1.txt"});
        //
        //          foreach (var entry in uris)
        //          {
        //              var res = PathUtil.MakePathRelativeToRootPath(entry[1], entry[0]);
        ////                var a = new System.Uri(Path.Combine(entry[0], "a"));
        ////                var b = new System.Uri(entry[1]);
        ////                var res = a.MakeRelativeUri(b).ToString();
        //              Assert.AreEqual(entry[2], res);
        //
        //          }
        //      }

        [Test]
        public void FullPathTest()
        {
            string rootPath = "/a/b/c";
            Dictionary<string, string> paths = new Dictionary<string, string>()
            {
                { "/a/b/c/file.txt", "file.txt" },
                { "/a/b/c/d/file.txt", "d/file.txt" },
                { "/a/b/file.txt", "../file.txt" },
                { "/a/file.txt", "../../file.txt" },
                { "/a/q/w/file.txt", "../../q/w/file.txt" },
                //              {"", ""},
            };

            foreach (var kvp in paths)
            {
                string res = PathUtil.MakePathRelativeToRootPath(kvp.Key, rootPath);
                Assert.AreEqual(kvp.Value, res);
            }
        }
    }
}
