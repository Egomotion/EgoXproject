// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//
using System;

namespace Egomotion.EgoXproject.Internal
{
    internal enum SystemCapability
    {
        iCloud,
        PushNotifications,
        GameCenter,
        Wallet,
        Siri,
        ApplePay,
        InAppPurchase,
        Maps,
        GameControllers,
        PersonalVPN,
        NetworkExtensions,
        HotspotConfiguration,
        Multipath,
        NFCTagReading,
        BackgroundModes,
        InterAppAudio,
        KeychainSharing,
        AssociatedDomains,
        AppGroups,
        DataProtection,
        HomeKit,
        HealthKit,
        WirelessAccessoryConfiguration,
    }

    internal static class SystemCapabilityHelper
    {
        public static SystemCapability[] AllCapabilites
        {
            get
            {
                return (SystemCapability[])System.Enum.GetValues(typeof(SystemCapability));
            }
        }

        public static SystemCapability[] IOSCapabilities
        {
            get
            {
                return new SystemCapability []
                {
                    SystemCapability.iCloud,
                    SystemCapability.PushNotifications,
                    SystemCapability.GameCenter,
                    SystemCapability.Wallet,
                    SystemCapability.Siri,
                    SystemCapability.ApplePay,
                    SystemCapability.InAppPurchase,
                    SystemCapability.Maps,
                    SystemCapability.PersonalVPN,
                    SystemCapability.NetworkExtensions,
                    SystemCapability.HotspotConfiguration,
                    SystemCapability.Multipath,
                    SystemCapability.NFCTagReading,
                    SystemCapability.BackgroundModes,
                    SystemCapability.InterAppAudio,
                    SystemCapability.KeychainSharing,
                    SystemCapability.AssociatedDomains,
                    SystemCapability.AppGroups,
                    SystemCapability.DataProtection,
                    SystemCapability.HomeKit,
                    SystemCapability.HealthKit,
                    SystemCapability.WirelessAccessoryConfiguration
                };
            }
        }

        public static SystemCapability[] TVOSCapabilities
        {
            get
            {
                return new SystemCapability []
                {
                    SystemCapability.iCloud,
                    SystemCapability.PushNotifications,
                    SystemCapability.GameCenter,
                    SystemCapability.InAppPurchase,
                    SystemCapability.Maps,
                    SystemCapability.GameControllers,
                    SystemCapability.BackgroundModes,
                    SystemCapability.KeychainSharing,
                    SystemCapability.AssociatedDomains,
                    SystemCapability.AppGroups,
                    SystemCapability.DataProtection,
                    SystemCapability.HomeKit
                };
            }
        }

        public static string Name(SystemCapability capability)
        {
            switch (capability)
            {
            case SystemCapability.iCloud:
                return "iCloud";

            case SystemCapability.PushNotifications:
                return "Push Notifications";

            case SystemCapability.GameCenter:
                return "Game Center";

            case SystemCapability.Wallet:
                return "Wallet";

            case SystemCapability.Siri:
                return "Siri";

            case SystemCapability.ApplePay:
                return "Apple Pay";

            case SystemCapability.InAppPurchase:
                return "In-App Purchase";

            case SystemCapability.Maps:
                return "Maps";

            case SystemCapability.GameControllers:
                return "Game Controllers";

            case SystemCapability.PersonalVPN:
                return "Personal VPN";

            case SystemCapability.NetworkExtensions:
                return "Network Extensions";

            case SystemCapability.HotspotConfiguration:
                return "Hotspot Configuration";

            case SystemCapability.Multipath:
                return "Multipath";

            case SystemCapability.NFCTagReading:
                return "Near Field Communication Tag Reading";

            case SystemCapability.BackgroundModes:
                return "Background Modes";

            case SystemCapability.InterAppAudio:
                return "Inter-App Audio";

            case SystemCapability.KeychainSharing:
                return "Keychain Sharing";

            case SystemCapability.AssociatedDomains:
                return "Associated Domains";

            case SystemCapability.AppGroups:
                return "App Groups";

            case SystemCapability.DataProtection:
                return "Data Protection";

            case SystemCapability.HomeKit:
                return "HomeKit";

            case SystemCapability.HealthKit:
                return "HealthKit";

            case SystemCapability.WirelessAccessoryConfiguration:
                return "Wireless Accessory Configuration";

            default:
                throw new System.ArgumentOutOfRangeException();
            }
        }

        public static BaseCapability Create(SystemCapability systemCapability, PListDictionary dic)
        {
            switch (systemCapability)
            {
            case SystemCapability.iCloud:
                return new ICloudCapability(dic);

            case SystemCapability.PushNotifications:
                return new PushNotificationsCapability(dic);

            case SystemCapability.GameCenter:
                return new GameCenterCapability(dic);

            case SystemCapability.Wallet:
                return new WalletCapability(dic);

            case SystemCapability.Siri:
                return new SiriCapability(dic);

            case SystemCapability.ApplePay:
                return new ApplePayCapability(dic);

            case SystemCapability.InAppPurchase:
                return new InAppPurchaseCapability(dic);

            case SystemCapability.Maps:
                return new MapsCapability(dic);

            case SystemCapability.GameControllers:
                return new GameControllersCapability (dic);

            case SystemCapability.PersonalVPN:
                return new PersonalVPNCapability(dic);

            case SystemCapability.NetworkExtensions:
                return new NetworkExtensionsCapability(dic);

            case SystemCapability.HotspotConfiguration:
                return new HotspotConfigurationCapability (dic);

            case SystemCapability.Multipath:
                return new MultipathCapability (dic);

            case SystemCapability.NFCTagReading:
                return new NFCTagReadingCapability (dic);

            case SystemCapability.BackgroundModes:
                return new BackgroundModesCapability(dic);

            case SystemCapability.InterAppAudio:
                return new InterAppAudioCapability(dic);

            case SystemCapability.KeychainSharing:
                return new KeychainSharingCapability(dic);

            case SystemCapability.AssociatedDomains:
                return new AssociatedDomainsCapability(dic);

            case SystemCapability.AppGroups:
                return new AppGroupsCapability(dic);

            case SystemCapability.DataProtection:
                return new DataProtectionapability(dic);

            case SystemCapability.HomeKit:
                return new HomeKitCapability(dic);

            case SystemCapability.HealthKit:
                return new HealthKitCapability(dic);

            case SystemCapability.WirelessAccessoryConfiguration:
                return new WirelessAccessoryConfigurationCapability(dic);

            default:
                throw new System.ArgumentOutOfRangeException();
            }
        }

        public static BaseCapability Create(SystemCapability systemCapability)
        {
            switch (systemCapability)
            {
            case SystemCapability.iCloud:
                return new ICloudCapability();

            case SystemCapability.PushNotifications:
                return new PushNotificationsCapability();

            case SystemCapability.GameCenter:
                return new GameCenterCapability();

            case SystemCapability.Wallet:
                return new WalletCapability();

            case SystemCapability.Siri:
                return new SiriCapability();

            case SystemCapability.ApplePay:
                return new ApplePayCapability();

            case SystemCapability.InAppPurchase:
                return new InAppPurchaseCapability();

            case SystemCapability.Maps:
                return new MapsCapability();

            case SystemCapability.GameControllers:
                return new GameControllersCapability ();

            case SystemCapability.PersonalVPN:
                return new PersonalVPNCapability();

            case SystemCapability.NetworkExtensions:
                return new NetworkExtensionsCapability();

            case SystemCapability.HotspotConfiguration:
                return new HotspotConfigurationCapability ();

            case SystemCapability.Multipath:
                return new MultipathCapability ();

            case SystemCapability.NFCTagReading:
                return new NFCTagReadingCapability ();

            case SystemCapability.BackgroundModes:
                return new BackgroundModesCapability();

            case SystemCapability.InterAppAudio:
                return new InterAppAudioCapability();

            case SystemCapability.KeychainSharing:
                return new KeychainSharingCapability();

            case SystemCapability.AssociatedDomains:
                return new AssociatedDomainsCapability();

            case SystemCapability.AppGroups:
                return new AppGroupsCapability();

            case SystemCapability.DataProtection:
                return new DataProtectionapability();

            case SystemCapability.HomeKit:
                return new HomeKitCapability();

            case SystemCapability.HealthKit:
                return new HealthKitCapability();

            case SystemCapability.WirelessAccessoryConfiguration:
                return new WirelessAccessoryConfigurationCapability();

            default:
                throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}
