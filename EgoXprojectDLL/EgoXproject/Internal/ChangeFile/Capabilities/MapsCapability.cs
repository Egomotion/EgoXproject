// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class MapsCapability : BaseCapability
    {
        const string AIRPLANE_KEY = "Airplane";
        const string BIKE_KEY = "Bike";
        const string BUS_KEY = "Bus";
        const string CAR_KEY = "Car";
        const string FERRY_KEY = "Ferry";
        const string PEDESTRIAN_KEY = "Pedestrian";
        const string RIDE_SHARING_KEY = "RideSharing";
        const string STREETCAR_KEY = "Streetcar";
        const string SUBWAY_KEY = "Subway";
        const string TAXI_KEY = "Taxi";
        const string TRAIN_KEY = "Train";
        const string OTHER_KEY = "Other";


        public MapsCapability()
        {
        }

        public MapsCapability(PListDictionary dic)
        {
            Airplane = dic.BoolValue(AIRPLANE_KEY);
            Bike = dic.BoolValue(BIKE_KEY);
            Bus = dic.BoolValue(BUS_KEY);
            Car = dic.BoolValue(CAR_KEY);
            Ferry = dic.BoolValue(FERRY_KEY);
            Pedestrian = dic.BoolValue(PEDESTRIAN_KEY);
            RideSharing = dic.BoolValue(RIDE_SHARING_KEY);
            Streetcar = dic.BoolValue(STREETCAR_KEY);
            Subway = dic.BoolValue(SUBWAY_KEY);
            Taxi = dic.BoolValue(TAXI_KEY);
            Train = dic.BoolValue(TRAIN_KEY);
            Other = dic.BoolValue(OTHER_KEY);
        }

        public MapsCapability(MapsCapability other)
        : base (other)
        {
            Airplane = other.Airplane;
            Bike = other.Bike;
            Bus = other.Bus;
            Car = other.Car;
            Ferry = other.Ferry;
            Pedestrian = other.Pedestrian;
            RideSharing = other.RideSharing;
            Streetcar = other.Streetcar;
            Subway = other.Subway;
            Taxi = other.Taxi;
            Train = other.Train;
            Other = other.Other;
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.AddIfTrue(AIRPLANE_KEY, Airplane);
            dic.AddIfTrue(BIKE_KEY, Bike);
            dic.AddIfTrue(BUS_KEY, Bus);
            dic.AddIfTrue(CAR_KEY, Car);
            dic.AddIfTrue(FERRY_KEY, Ferry);
            dic.AddIfTrue(PEDESTRIAN_KEY, Pedestrian);
            dic.AddIfTrue(RIDE_SHARING_KEY, RideSharing);
            dic.AddIfTrue(STREETCAR_KEY, Streetcar);
            dic.AddIfTrue(SUBWAY_KEY, Subway);
            dic.AddIfTrue(TAXI_KEY, Taxi);
            dic.AddIfTrue(TRAIN_KEY, Train);
            dic.AddIfTrue(OTHER_KEY, Other);
            return dic;
        }

        public override BaseCapability Clone()
        {
            return new MapsCapability(this);
        }

        #endregion

        public bool Airplane
        {
            get;
            set;
        }
        public bool Bike
        {
            get;
            set;
        }
        public bool Bus
        {
            get;
            set;
        }
        public bool Car
        {
            get;
            set;
        }
        public bool Ferry
        {
            get;
            set;
        }
        public bool Pedestrian
        {
            get;
            set;
        }
        public bool RideSharing
        {
            get;
            set;
        }
        public bool Streetcar
        {
            get;
            set;
        }
        public bool Subway
        {
            get;
            set;
        }
        public bool Taxi
        {
            get;
            set;
        }
        public bool Train
        {
            get;
            set;
        }
        public bool Other
        {
            get;
            set;
        }
    }
}

