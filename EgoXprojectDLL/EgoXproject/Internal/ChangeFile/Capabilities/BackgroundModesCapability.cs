// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class BackgroundModesCapability : BaseCapability
    {
        const string AUDIO_AIRPLAY_PIP_KEY = "AudioAirplayPIP";
        const string LOCATION_UPDATES_KEY = "LocationUpdates";
        const string VOIP_KEY = "VOIP";
        const string NEWSSTAND_DOWNLOADS_KEY = "NewsstandDownloads";
        const string EXTERNAL_ACC_COMMS_KEY = "ExternalAccessortCommunication";
        const string USES_BTLE_ACC_KEY = "UsesBTLEAccessories";
        const string ACTS_AS_BTLE_ACC_KEY = "ActsAsBTLEAccessory";
        const string BACKGROUND_FETCH_KEY = "BackgroundFetch";
        const string REMOTE_NOTIFICATIONS_KEY = "RemoteNotifications";

        public BackgroundModesCapability()
        {
        }

        public BackgroundModesCapability(PListDictionary dic)
        {
            AudioAirplayPIP = dic.BoolValue(AUDIO_AIRPLAY_PIP_KEY);
            LocationUpdates = dic.BoolValue(LOCATION_UPDATES_KEY);
            VOIP = dic.BoolValue(VOIP_KEY);
            NewsstandDownloads = dic.BoolValue(NEWSSTAND_DOWNLOADS_KEY);
            ExternalAccComms = dic.BoolValue(EXTERNAL_ACC_COMMS_KEY);
            UsesBTLEAcc = dic.BoolValue(USES_BTLE_ACC_KEY);
            ActsAsBTLEAcc = dic.BoolValue(ACTS_AS_BTLE_ACC_KEY);
            BackgroundFetch = dic.BoolValue(BACKGROUND_FETCH_KEY);
            RemoteNotifications = dic.BoolValue(REMOTE_NOTIFICATIONS_KEY);
        }

        public BackgroundModesCapability(BackgroundModesCapability other)
        : base (other)
        {
            AudioAirplayPIP = other.AudioAirplayPIP;
            LocationUpdates = other.LocationUpdates;
            VOIP = other.VOIP;
            NewsstandDownloads = other.NewsstandDownloads;
            ExternalAccComms = other.ExternalAccComms;
            UsesBTLEAcc = other.UsesBTLEAcc;
            ActsAsBTLEAcc = other.ActsAsBTLEAcc;
            BackgroundFetch = other.BackgroundFetch;
            RemoteNotifications = other.RemoteNotifications;
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.AddIfTrue(AUDIO_AIRPLAY_PIP_KEY, AudioAirplayPIP);
            dic.AddIfTrue(LOCATION_UPDATES_KEY, LocationUpdates);
            dic.AddIfTrue(VOIP_KEY, VOIP);
            dic.AddIfTrue(NEWSSTAND_DOWNLOADS_KEY, NewsstandDownloads);
            dic.AddIfTrue(EXTERNAL_ACC_COMMS_KEY, ExternalAccComms);
            dic.AddIfTrue(USES_BTLE_ACC_KEY, UsesBTLEAcc);
            dic.AddIfTrue(ACTS_AS_BTLE_ACC_KEY, ActsAsBTLEAcc);
            dic.AddIfTrue(BACKGROUND_FETCH_KEY, BackgroundFetch);
            dic.AddIfTrue(REMOTE_NOTIFICATIONS_KEY, RemoteNotifications);
            return dic;
        }

        public override BaseCapability Clone()
        {
            return new BackgroundModesCapability(this);
        }

        public bool AudioAirplayPIP
        {
            get;
            set;
        }
        public bool LocationUpdates
        {
            get;
            set;
        }
        public bool VOIP
        {
            get;
            set;
        }
        public bool NewsstandDownloads
        {
            get;
            set;
        }
        public bool ExternalAccComms
        {
            get;
            set;
        }
        public bool UsesBTLEAcc
        {
            get;
            set;
        }
        public bool ActsAsBTLEAcc
        {
            get;
            set;
        }
        public bool BackgroundFetch
        {
            get;
            set;
        }
        public bool RemoteNotifications
        {
            get;
            set;
        }

        #endregion
    }
}
