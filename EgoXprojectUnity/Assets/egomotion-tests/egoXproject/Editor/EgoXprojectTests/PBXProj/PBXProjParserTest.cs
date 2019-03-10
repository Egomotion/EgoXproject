using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;
using System.IO;

namespace Egomotion.EgoXprojectTests.PBXProjTests
{
    [TestFixture]
    public class PBXProjParserTest
    {
        [Test]
        public void ParseFile()
        {
            string sampleFilePath = Path.Combine(Application.dataPath, "egomotion-tests/egoXproject/SampleData");
            string testFile = Path.Combine(sampleFilePath, "sample.pbxproj");
            var parser = new PBXProjParser();
            var dic = parser.Parse(testFile);
            Assert.IsNotNull(dic);
            Assert.AreEqual("C464EA4E18CDBEA5000F8F92", (dic["myString"] as PBXProjString).Value);
            Assert.AreEqual("\"qstr\"", (dic["C464EA4E18CDBEA5000F8F92"] as PBXProjString).Value);
            Assert.IsTrue((dic["bool1"] as PBXProjBoolean).Value);
            Assert.IsFalse((dic["bool2"] as PBXProjBoolean).Value);
            var emptyDic = dic["emptyDic"] as PBXProjDictionary;
            Assert.AreEqual(0, emptyDic.Count);
            var emptyArray = dic["emptyArray"] as PBXProjArray;
            Assert.AreEqual(0, emptyArray.Count);
            var dic2 = dic["dic"] as PBXProjDictionary;
            Assert.AreEqual(4, dic2.Count);
            var key1Dic = dic2["key1"] as PBXProjDictionary;
            Assert.AreEqual(2, key1Dic.Count);
            Assert.AreEqual("PBXBuildFile", (key1Dic["isa"] as PBXProjString).Value);
            Assert.AreEqual("keyA", (key1Dic["fileRef"] as PBXProjString).Value);
            var key2Dic = dic2["key2"] as PBXProjDictionary;
            Assert.AreEqual(2, key2Dic.Count);
            Assert.AreEqual("PBXBuildFile", (key2Dic["isa"] as PBXProjString).Value);
            Assert.AreEqual("keyB", (key2Dic["fileRef"] as PBXProjString).Value);
            var key3Dic = dic2["key3"] as PBXProjDictionary;
            Assert.AreEqual(4, key3Dic.Count);
            Assert.AreEqual("PBXGroup", (key3Dic["isa"] as PBXProjString).Value);
            var array = key3Dic["children"] as PBXProjArray;
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual("valueA", (array[0] as PBXProjString).Value);
            Assert.AreEqual("valueB", (array[1] as PBXProjString).Value);
            Assert.AreEqual("\"<group>\"", (key3Dic["quoted"] as PBXProjString).Value);
            Assert.AreEqual(0, (key3Dic["empty"] as PBXProjArray).Count);
            Assert.AreEqual("\"$(ARCHS_STANDARD_INCLUDING_64_BIT)\"", (dic2["quoted2"] as PBXProjString).Value);
        }
    }
}