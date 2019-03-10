using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Egomotion.EgoXproject.Internal;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Egomotion.EgoXprojectTests.PListTests
{
    [TestFixture]
    public class PListTests
    {
        PList _basicPlist;
        string[] _basicKeys;
        string _sampleFilePath;
        string _basicPlistPath;

        [SetUp]
        public void SetUp()
        {
            _sampleFilePath = Path.Combine(Application.dataPath, "egomotion-tests/egoXproject/SampleData");
            _basicPlistPath = Path.Combine(_sampleFilePath, "basic.plist");
            _basicPlist = new PList();
            bool bOK = _basicPlist.Load(_basicPlistPath);
            Assert.True(bOK);
            string basicKeysPath = Path.Combine(_sampleFilePath, "basic-keys.txt");
            _basicKeys = File.ReadAllLines(basicKeysPath);
        }

        [TearDown]
        public void TearDown()
        {
            _basicKeys = null;
            _basicPlist = null;
        }

        [Test]
        public void InitTest()
        {
            //All content moved to setup
            Assert.IsNotNull(_basicPlist);
        }

        [Test]
        public void ParseNonExistantTest()
        {
            PList plist = new PList();
            bool bOK = plist.Load("Does/Not/Exist/Info.plist");
            Assert.False(bOK);
        }

        [Test]
        public void ListKeys()
        {
            List<string> keys = _basicPlist.Root.Keys.ToList();
            Assert.IsNotNull(keys);
            Assert.Greater(keys.Count, 0);

            foreach (var k in _basicKeys)
            {
                Assert.Contains(k, keys);
            }
        }

        [Test]
        public void GetInt()
        {
            PListInteger element = _basicPlist.Root["IntValue"] as PListInteger;
            Assert.IsNotNull(element);
            Assert.AreEqual(element.IntValue, 10);
        }

        [Test]
        public void GetBool()
        {
            PListBoolean element = _basicPlist.Root["BoolValueTrue"] as PListBoolean;
            Assert.IsNotNull(element);
            Assert.True(element.Value);
            element = _basicPlist.Root["BoolValueFalse"] as PListBoolean;
            Assert.IsNotNull(element);
            Assert.False(element.Value);
        }

        [Test]
        public void GetReal()
        {
            PListReal element = _basicPlist.Root["RealValue"] as PListReal;
            Assert.IsNotNull(element);
            Assert.AreEqual(element.FloatValue, 3.14f);
        }

        [Test]
        public void GetString()
        {
            PListString element = _basicPlist.Root["StringValue"] as PListString;
            Assert.IsNotNull(element);
            Assert.AreEqual(element.Value, "Foo");
        }

        [Test]
        public void GetDate()
        {
            PListDate element = _basicPlist.Root["DateValue"] as PListDate;
            Assert.IsNotNull(element);
            Assert.AreEqual(element.ToString(), "2014-03-07T13:28:45Z");
        }

        [Test]
        public void GetData()
        {
            PListData element = _basicPlist.Root["DataValue"] as PListData;
            Assert.IsNotNull(element);
            Assert.AreEqual(element.ToString(), "bXkgcGhvdG8=");
        }

        [Test]
        public void GetArray()
        {
            PListArray element = _basicPlist.Root["ArrayValue"] as PListArray;
            Assert.IsNotNull(element);
            Assert.AreEqual(element.Count, 6); //dont handle date or data
            var b = element.Where(e => (e as PListBoolean).Value == true);
            Assert.IsNotNull(b);
            var s = element.Where(e => (e as PListString).Value == "Array Element");
            Assert.IsNotNull(s);
            var i = element.Where(e => (e as PListInteger).IntValue == 20);
            Assert.IsNotNull(i);
            var r = element.Where(e => (e as PListReal).FloatValue == 20.12f);
            Assert.IsNotNull(r);
            var date = element.Where(e => (e as PListDate).StringValue == "2014-03-08T13:31:13Z");
            Assert.IsNotNull(date);
            var data = element.Where(e => (e as PListData).Value == "ASNFZw==");
            Assert.IsNotNull(data);
        }

        [Test]
        public void GetDictionary()
        {
            PListDictionary element = _basicPlist.Root["DictionaryValue"] as PListDictionary;
            Assert.IsNotNull(element);
            Assert.AreEqual(element.Count, 6); //dont handle date or data
            Assert.Contains("DicInt", element.Keys);
            Assert.AreEqual(30, (element["DicInt"] as PListInteger).IntValue);
            Assert.Contains("DicReal", element.Keys);
            Assert.AreEqual(30.12f, (element["DicReal"] as PListReal).FloatValue);
            Assert.Contains("DicBool", element.Keys);
            Assert.AreEqual(true, (element["DicBool"] as PListBoolean).Value);
            Assert.Contains("DicString", element.Keys);
            Assert.AreEqual("Dictionary Element", (element["DicString"] as PListString).Value);
            Assert.Contains("DicDate", element.Keys);
            Assert.AreEqual("2014-03-09T13:33:00Z", (element["DicDate"] as PListDate).StringValue);
            Assert.Contains("DicDate", element.Keys);
            Assert.AreEqual("ASNFZ4mrze8=", (element["DicData"] as PListData).Value);
        }

        public void ThrowNonExistant()
        {
            var v = _basicPlist.Root["DoesNotExist"];
            v.GetType();
        }

        [Test]
        public void GetNonExistant()
        {
            Assert.Throws(typeof(KeyNotFoundException), ThrowNonExistant);
        }

        [Test]
        public void Save()
        {
            string savePath = Path.Combine(_sampleFilePath, "basic-unmodified.plist");

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            Assert.False(File.Exists(savePath));
            bool ok = _basicPlist.Save(savePath);
            Assert.True(ok);
            Assert.True(File.Exists(savePath));
            PList unmod = new PList();
            ok = unmod.Load(savePath);
            Assert.IsTrue(ok);
            Assert.AreEqual(_basicPlist.ToString(), unmod.ToString());
            File.Delete(savePath);
        }

        [Test]
        public void SaveEmpty()
        {
            LogAssert.ignoreFailingMessages = true;

            PList p = new PList();
            Assert.IsFalse(p.Save(""));
        }

        [Test]
        public void SaveDir()
        {
            LogAssert.ignoreFailingMessages = true;

            string path = Path.Combine(_sampleFilePath, "dir");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            PList p = new PList();

            Assert.IsTrue(Directory.Exists(path));
            Assert.IsFalse(p.Save(path));
        }

        [Test]
        public void SaveNotExistingDir()
        {
            LogAssert.ignoreFailingMessages = true;

            PList p = new PList();
            string path = _sampleFilePath + "/DoesNotExist/myfile.plist";
            Assert.IsFalse(p.Save(path));
        }

        [Test]
        public void SaveInvalidFileName()
        {
            LogAssert.ignoreFailingMessages = true;

            PList p = new PList();
            string[] filenames = {
                ".test.plist",
                " test.plist",
                "test.plist ",
                ":test.plist",
                "tes:t.plist"
            };
            //TODO add more

            foreach (string filename in filenames) {
                string path = Path.Combine(_sampleFilePath, filename);
                Assert.IsFalse(p.Save(path));
            }
        }

        [Test]
        public void SaveOverwrite()
        {
            LogAssert.ignoreFailingMessages = true;

            PList p = new PList();
            string path = Path.Combine(_sampleFilePath, "existing.txt");
            Assert.IsFalse(p.Save(path));
        }

        [Test]
        public void ContainsKey()
        {
            Assert.IsTrue(_basicPlist.Root.ContainsKey(_basicKeys[0]));
            Assert.IsFalse(_basicPlist.Root.ContainsKey("MadeUpKey"));
        }

        [Test]
        public void SetBool()
        {
            var plist = new PList();
            string newKey = "NewBool";
            Assert.IsFalse(plist.Root.ContainsKey(newKey));
            PListBoolean b = new PListBoolean();
            b.Value = true;
            plist.Root.Add(newKey, b);
            Assert.IsTrue(plist.Root.ContainsKey(newKey));
            var b2 = plist.Root[newKey] as PListBoolean;
            Assert.IsNotNull(b2);
            Assert.IsTrue(b2.Value);
        }

        [Test]
        public void Create()
        {
            PList p = new PList();
            var dict = p.Root;
            dict.Add("IntValue", new PListInteger(10));
            dict.Add("RealValue", new PListReal(3.14f));
            dict.Add("StringValue", new PListString("Foo"));
            dict.Add("BoolValueTrue", new PListBoolean(true));
            dict.Add("BoolValueFalse", new PListBoolean(false));
            dict.Add("DateValue", new PListDate("2014-03-07T13:28:45Z"));
            dict.Add("DataValue", new PListData("bXkgcGhvdG8="));
            var array = new PListArray();
            array.Add(new PListString("Array Element"));
            array.Add(new PListBoolean(true));
            array.Add(new PListInteger(20));
            array.Add(new PListReal(20.12f));
            array.Add(new PListDate("2014-03-08T13:31:13Z"));
            array.Add(new PListData("ASNFZw=="));
            dict.Add("ArrayValue", array);
            var d = new PListDictionary();
            d.Add("DicInt", new PListInteger(30));
            d.Add("DicReal", new PListReal(30.12f));
            d.Add("DicString", new PListString("Dictionary Element"));
            d.Add("DicBool", new PListBoolean(true));
            d.Add("DicData", new PListData("ASNFZ4mrze8="));
            d.Add("DicDate", new PListDate("2014-03-09T13:33:00Z"));
            dict.Add("DictionaryValue", d);;
            string path = Path.Combine(_sampleFilePath, "manual.plist");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assert.IsFalse(File.Exists(path));
            bool ok = p.Save(path);
            Assert.IsTrue(ok);
            PList reload = new PList();
            ok = reload.Load(path);
            Assert.IsTrue(ok);
            Assert.AreEqual(_basicPlist.ToString(), reload.ToString());
            File.Delete(path);
        }


        [Test]
        public void Copy()
        {
            var copy = new PList();
            copy.Root = _basicPlist.Root.Copy() as PListDictionary;
            Assert.AreNotSame(copy.Root, _basicPlist.Root);
            Assert.AreEqual(copy.Root.Count, _basicPlist.Root.Count);
            Assert.AreEqual(copy.ToString(), _basicPlist.ToString());
        }
    }
}
