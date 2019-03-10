using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Egomotion.EgoXproject.Internal;
using System.IO;
using System.Linq;

namespace Egomotion.EgoXprojectTests.PListTests
{
    [TestFixture]
    public class PListManagerTest
    {
        //      PListManager _plistManager;
        ////        string _sampleFilePath;
        //
        //      [SetUp]
        //      public void Setup()
        //      {
        ////            _sampleFilePath = Path.Combine(Application.dataPath, "egomotion-tests/egoXproject/SampleData");
        //
        //          _plistManager = PListManager.Instance();
        //
        //          string testFile = Path.Combine(_plistManager.SavePath, "test.egoplist");
        //          if (File.Exists(testFile)) {
        //              File.Delete(testFile);
        //          }
        //      }
        //
        //      [TearDown]
        //      public void TearDown()
        //      {
        //
        //      }
        //
        //      string CreateTestPlist()
        //      {
        //          bool b = _plistManager.Create("test");
        //
        //          Assert.IsTrue(b);
        //
        //          string path = Path.Combine(_plistManager.SavePath, "test.egoplist");
        //
        //          Assert.IsTrue(File.Exists(path));
        //
        //          return path;
        //      }
        //
        //      [Test]
        //      public void CreatePList()
        //      {
        //          string path = CreateTestPlist();
        //          _plistManager.DeleteCurrent();
        //
        //          Assert.IsFalse(File.Exists(path));
        //      }
        //
        //      [Test]
        //      public void CreatePListExisting()
        //      {
        //          var path = CreateTestPlist();
        //
        //          bool ok = _plistManager.Create("test");
        //          Assert.IsFalse(ok);
        //
        //          _plistManager.DeleteCurrent();
        //          Assert.IsFalse(File.Exists(path));
        //      }
        //
        //      [Test]
        //      public void CreatePListEmptyName()
        //      {
        //          bool b = _plistManager.Create("");
        //          Assert.IsFalse(b);
        //      }
        //
        //      [Test]
        //      public void CreatePListInvalidName()
        //      {
        //          bool p;
        //
        //          p = _plistManager.Create(".test");
        //          Assert.IsFalse(p);
        //          p = _plistManager.Create(" test");
        //          Assert.IsFalse(p);
        //          p = _plistManager.Create("test ");
        //          Assert.IsFalse(p);
        //          p = _plistManager.Create(":test");
        //          Assert.IsFalse(p);
        //          p = _plistManager.Create("tes:t");
        //          Assert.IsFalse(p);
        //
        //          //TODO a more complete check
        //  //      char[] chars = new char[]{'±','§','!','@','£','$','%','^','&','*','(',')','+','=','{','[',']','}',';',':','\'','\"','\\','|','`','<','>',',','/','?'};
        //  //      char[] startChars = new char[]{'.',' ','~'};
        //  //      PList p = null;
        //  //
        //  //      foreach (char c in Path.GetInvalidFileNameChars())
        //  //      {
        //  //          Debug.Log(c);
        //  //          p = _plistManager.Create("test" + c);
        //  //          Assert.IsNull(p);
        //  //          p = _plistManager.Create(c + "test");
        //  //          Assert.IsNull(p);
        //  //          p = _plistManager.Create("test" + c + "test");
        //  //          Assert.IsNull(p);
        //  //      }
        //  //
        //  //      foreach (char c in startChars)
        //  //      {
        //  //          p = _plistManager.Create(c + "test");
        //  //          Assert.IsNull(p);
        //  //          p = _plistManager.Create("test" + c);
        //  //          Assert.IsNotNull(p);
        //  //          File.Delete(p.SavePath);
        //  //          p = _plistManager.Create("test" + c + "test");
        //  //          Assert.IsNotNull(p);
        //  //          File.Delete(p.SavePath);
        //  //      }
        //      }
        //
        //      [Test]
        //      public void AllPListFiles()
        //      {
        //          throw new System.NotImplementedException();
        //
        //  //      Assert.Greater(_plistManager.PListCount, 0);
        //  //      for (int ii = 0; ii < _plistManager.PListCount; ++ii)
        //  //      {
        //  //          Assert.IsNotNull(_plistManager.PListAtIndex(ii));
        //  //      }
        //
        //          //TODO know how many there should be
        //      }
        //
        //      [Test]
        //      public void Refesh()
        //      {
        //          throw new System.NotImplementedException();
        //          /*
        //          string testListPath = Path.Combine(_sampleFilePath, "test.egoplist");
        //
        //          if (File.Exists(testListPath)) {
        //              File.Delete(testListPath);
        //          }
        //
        //          Assert.IsFalse(File.Exists(testListPath));
        //
        //          Assert.Greater(_plistManager.PListCount, 0);
        //          int count = _plistManager.PListCount;
        //
        //          for (int ii = 0; ii < _plistManager.PListCount; ++ii)
        //          {
        //              Assert.AreNotEqual(_plistManager.PListAtIndex(ii).SavePath, testListPath);
        //          }
        //
        //          var testList = new PList();
        //          bool ok = testList.Save(testListPath);
        //          Assert.IsTrue(ok);
        //
        //          _plistManager.Refesh();
        //
        //          Assert.AreEqual(count+1, _plistManager.PListCount);
        //
        //          int found = 0;
        //          for (int ii = 0; ii < _plistManager.PListCount; ++ii)
        //          {
        //              if (_plistManager.PListAtIndex(ii).SavePath == testListPath) {
        //                  found++;
        //              }
        //          }
        //
        //          Assert.AreEqual(found, 1);
        //
        //          File.Delete(testListPath);
        //
        //
        //          _plistManager.Refesh();
        //
        //          Assert.AreEqual(count, _plistManager.PListCount);
        //
        //          for (int ii = 0; ii < _plistManager.PListCount; ++ii)
        //          {
        //              Assert.AreNotEqual(_plistManager.PListAtIndex(ii).SavePath, testListPath);
        //          }
        //          */
        //      }
        //
        //      [Test]
        //      public void GetTarget()
        //      {
        //          throw new System.NotImplementedException();
        //          //verify that target is null or save the current one
        //          //set target to test file
        //          //get target plist
        //      }
        //
        //      [Test]
        //      public void TestMerge()
        //      {
        //          throw new System.NotImplementedException();
        //          //set target
        //          //merge
        //          //verify the merge is as expected
        //      }
        //
        //      [Test]
        //      public void BuildMerge()
        //      {
        //          throw new System.NotImplementedException();
        //          //call the static merge
        //          //check output
        //      }
        //
        //      [Test]
        //      public void InvalidTarget()
        //      {
        //          throw new System.NotImplementedException();
        //          //check that setting an invalid target fails correctly
        //      }
        //
        //      [Test]
        //      public void InvalidBuildTarget()
        //      {
        //          throw new System.NotImplementedException();
        //          //check that using the wrong build target doesn't build
        //      }
        //
        //      [Test]
        //      public void InvalidBuildTargetPath()
        //      {
        //          throw new System.NotImplementedException();
        //          //check that setting an invalid path fails correctly
        //      }
        //
        //      [Test]
        //      public void AddPList()
        //      {
        //          throw new System.NotImplementedException();
        //          //add a new Plist to the list of available merge targets
        //      }

    }
}