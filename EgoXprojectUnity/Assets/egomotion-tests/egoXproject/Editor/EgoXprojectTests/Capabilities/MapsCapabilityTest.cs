using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Egomotion.EgoXproject.Internal;
using NUnit.Framework;

namespace Egomotion.EgoXprojectTests.CapabilitiesTests
{
    [TestFixture]
    public class MapsCapabilityTest : BaseCapabilityTest
    {
        [Test]
        public void NoneSelected()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsNone.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void AllSelected()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Airplane = true;
            capability.Bike = true;
            capability.Bus = true;
            capability.Car = true;
            capability.Ferry = true;
            capability.Other = true;
            capability.Pedestrian = true;
            capability.RideSharing = true;
            capability.Streetcar = true;
            capability.Subway = true;
            capability.Taxi = true;
            capability.Train = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsAll.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Airplane()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Airplane = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsAirplane.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Bike()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Bike = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsBike.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Bus()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Bus = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsBus.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Car()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Car = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsCar.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Ferry()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Ferry = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsFerry.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Pedestrian()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Pedestrian = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsPedestrian.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void RideSharing()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.RideSharing = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsRideSharing.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Streetcar()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Streetcar = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsStreetcar.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Subway()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Subway = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsSubway.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Taxi()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Taxi = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsTaxi.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Train()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Train = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsTrain.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void Other()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            var capability = cf.Capabilities.Capability(SystemCapability.Maps) as MapsCapability;
            capability.Other = true;
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsOther.plist", TestInfoPlistFilePath);
        }

        [Test]
        public void TVOS()
        {
            CreateOriginalCopies();
            var xpm = new XcodeProjectManipulator();
            Assert.True(xpm.Load(XcodeProjectPath));
            var cf = new XcodeChangeFile();
            cf.Platform = BuildPlatform.tvOS;
            cf.Capabilities.EnableCapability(SystemCapability.Maps, true);
            Assert.True(xpm.ApplyChanges(cf));
            CompareProjectFiles("Maps.pbxproj", TestPBXFilePath);
            CompareInfoPlistFiles("MapsNone.plist", TestInfoPlistFilePath);
        }

    }
}
