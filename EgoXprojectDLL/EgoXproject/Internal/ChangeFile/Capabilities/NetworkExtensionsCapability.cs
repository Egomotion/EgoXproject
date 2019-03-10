// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class NetworkExtensionsCapability : BaseCapability
    {
        const string APP_PROXY_KEY = "AppProxy";
        const string CONTENT_FILTER_KEY = "ContentFilter";
        const string PACKET_TUNNEL_KEY = "PacketTunnel";
        const string DNS_PROXY_KEY = "DNSProxy";

        public NetworkExtensionsCapability()
        {
        }

        public NetworkExtensionsCapability(PListDictionary dic)
        {
            AppProxy = dic.BoolValue (APP_PROXY_KEY);
            ContentFilter = dic.BoolValue (CONTENT_FILTER_KEY);
            PacketTunnel = dic.BoolValue (PACKET_TUNNEL_KEY);
            DNSProxy = dic.BoolValue (DNS_PROXY_KEY);
        }

        public NetworkExtensionsCapability(NetworkExtensionsCapability other)
        : base (other)
        {
            AppProxy = other.AppProxy;
            ContentFilter = other.ContentFilter;
            PacketTunnel = other.PacketTunnel;
            DNSProxy = other.DNSProxy;
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.AddIfTrue (APP_PROXY_KEY, AppProxy);
            dic.AddIfTrue (CONTENT_FILTER_KEY, ContentFilter);
            dic.AddIfTrue (PACKET_TUNNEL_KEY, PacketTunnel);
            dic.AddIfTrue (DNS_PROXY_KEY, DNSProxy);
            return dic;
        }

        public override BaseCapability Clone()
        {
            return new NetworkExtensionsCapability(this);
        }

        public bool AppProxy { get; set; }
        public bool ContentFilter { get; set; }
        public bool PacketTunnel { get; set; }
        public bool DNSProxy { get; set; }

        #endregion
    }
}

