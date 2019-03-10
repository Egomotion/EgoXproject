// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

/*
 * Should load the pbxproj file and parse that
 * Then should query it for the correct info plist
 * Then check the Info.plist exists
 */
namespace Egomotion.EgoXproject.Internal
{
    internal class XcodeProjectManipulator
    {
        const string BACKUP_FILENAME = "project.egoxproject-backup.pbxproj";

        string _projectPath;

        string _pxbProjPath;

        BuildPlatform _platform;

        PBXProj _pbxproj;

        public bool Load(string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                Debug.LogError("EgoXproject: Must have a Xcode project path");
                return false;
            }

            //make sure we have a full path and not a relative path
            projectPath = Path.GetFullPath(projectPath);

            if (!LocatePBXProjFile(projectPath, out _pxbProjPath))
            {
                Debug.LogError("EgoXproject: Path must be to a valid Xcode project");
                return false;
            }

            _projectPath = projectPath;
            var backup = CreateBackup();
            _pbxproj = new PBXProj();

            if (!_pbxproj.Load(_pxbProjPath))
            {
                RemoveBackup(backup);
                Debug.LogError("EgoXproject: Failed to load the Xcode project file - " + _pbxproj.ErrorMessage + ". Path: " + _pxbProjPath);
                return false;
            }

            return true;
        }

        string CreateBackup()
        {
            var dir = Path.GetDirectoryName(_pxbProjPath);
            var backup = Path.Combine(dir, BACKUP_FILENAME);
            RemoveBackup(backup);
            File.Copy(_pxbProjPath, backup);
            return backup;
        }

        void RemoveBackup(string backup)
        {
            if (File.Exists(backup))
            {
                File.Delete(backup);
            }
        }

        public bool ApplyChanges(XcodeChangeFile changes)
        {
            if (changes == null)
            {
                Debug.LogError("EgoXproject: Must have a valid change file");
                return false;
            }

            if (_pbxproj == null)
            {
                Debug.LogError("EgoXproject: Must load an Xcode project first");
                return false;
            }

            _platform = changes.Platform;

            if (!ApplyPBXProjChanges(changes))
            {
                Debug.LogError("EgoXproject: Unable to apply changes and save Xcode project file - " + _pbxproj.ErrorMessage + ". Path: " + _pxbProjPath);
                return false;
            }

            if (!ApplyInfoPlistChanges(changes.InfoPlistChanges))
            {
                return false;
            }

            if (!ApplyCapabilityChanges(changes.Capabilities))
            {
                return false;
            }

            return true;
        }

        #region PBXProj

        bool ApplyPBXProjChanges(XcodeChangeFile changes)
        {
            AddFrameworks(changes.Frameworks);
            AddFilesAndFolders(changes.FilesAndFolders);
            AddBuildSettings(changes.BuildSettings);
            AddScripts(changes.Scripts);
            ApplySigningChanges(changes.Signing);
            return _pbxproj.Save();
        }

        bool LocatePBXProjFile(string projectDir, out string pbxProjFilePath)
        {
            pbxProjFilePath = "";

            if (string.IsNullOrEmpty(projectDir))
            {
                return false;
            }

            var xcodeprojDirs = Directory.GetDirectories(projectDir, "*.xcodeproj");

            if (xcodeprojDirs.Length <= 0)
            {
                return false;
            }

            var path = Path.Combine(xcodeprojDirs[0], "project.pbxproj");

            if (File.Exists(path))
            {
                pbxProjFilePath = path;
                return true;
            }

            return false;
        }

        void AddFrameworks(FrameworkChanges changes)
        {
            for (int ii = 0; ii < changes.Count; ++ii)
            {
                _pbxproj.AddSystemFramework(changes.FileNameAt(ii), changes.LinkTypeAt(ii));
            }
        }

        void AddScripts(ScriptChanges changes)
        {
            for (int ii = 0; ii < changes.Count; ++ii)
            {
                _pbxproj.AddScript(changes.NameAt(ii), changes.ShellAt(ii), changes.ScriptAt(ii));
            }
        }

        void AddFilesAndFolders(FilesAndFolderChanges changes)
        {
            foreach (var entry in changes.Entries)
            {
                _pbxproj.AddFileOrFolder(entry);
            }
        }

        void AddBuildSettings(BuildSettingsChanges changes)
        {
            for (int ii = 0; ii < changes.Count; ++ii)
            {
                var entry = changes.EntryAt(ii);

                if (entry is BoolBuildSettingEntry)
                {
                    var boolEntry = entry as BoolBuildSettingEntry;
                    _pbxproj.AddBoolBuildSetting(boolEntry.Name, boolEntry.Value);
                }
                else if (entry is EnumBuildSettingEntry)
                {
                    var enumEntry = entry as EnumBuildSettingEntry;
                    _pbxproj.AddEnumBuildSetting(enumEntry.Name, enumEntry.Value);
                }
                else if (entry is CustomStringBuildSettingEntry)
                {
                    var strEntry = entry as CustomStringBuildSettingEntry;
                    _pbxproj.AddCustomStringBuildSetting(strEntry.Name, strEntry.Value);
                }
                else if (entry is StringBuildSettingEntry)
                {
                    var strEntry = entry as StringBuildSettingEntry;
                    _pbxproj.AddStringBuildSetting(strEntry.Name, strEntry.Value);
                }
                else if (entry is CollectionBuildSettingEntry)
                {
                    var arrayEntry = entry as CollectionBuildSettingEntry;
                    _pbxproj.AddCollectionBuildSetting(arrayEntry.Name, arrayEntry.Values.ToArray(), arrayEntry.Merge);
                }
            }
        }

        void ApplySigningChanges(SigningChanges changes)
        {
            _pbxproj.SetTeamId(changes.TeamId);
            _pbxproj.EnableAutomaticProvisioning(changes.AutomaticProvisioning);
        }

        #endregion

        #region Info Plist

        bool ApplyInfoPlistChanges(PListDictionary changes, bool forceReplace = false)
        {
            bool plistDone = false;

            if (_pbxproj.InfoPlistPaths.Length > 0)
            {
                plistDone = true;

                foreach (var infoPlistPath in _pbxproj.InfoPlistPaths)
                {
                    if (!ModifyInfoPlist(infoPlistPath, changes, forceReplace))
                    {
                        plistDone = false;
                    }
                }
            }

            //TODO is this sensible?
            //if we fail to modify the provided info plists, 
            //try modifying the one that should be in the default location
            if (!plistDone)
            {
                string defaultPlistPath = Path.Combine(_projectPath, "Info.plist");

                if (ModifyInfoPlist(defaultPlistPath, changes, forceReplace))
                {
                    return false;
                }
            }

            return true;
        }

        bool ModifyInfoPlist(string pathToInfoPlist, PListDictionary changes, bool forceReplace)
        {
            PList infoPlist = new PList();

            if (infoPlist.Load(pathToInfoPlist))
            {
                MergeDictionaries(infoPlist.Root, changes, false, forceReplace);
                return infoPlist.Save();
            }
            else
            {
                Debug.LogError("EgoXproject: Failed to open Info.plist file: " + pathToInfoPlist);
                return false;
            }
        }

        void MergeDictionaries(PListDictionary original, PListDictionary toMerge, bool keepEmpty, bool forceReplace)
        {
            foreach (var kvp in toMerge)
            {
                IPListElement toMergeElement = kvp.Value;

                if (toMergeElement is PListDictionary)
                {
                    var toMergeDic = toMergeElement as PListDictionary;
                    var originalDic = original.Element<PListDictionary>(kvp.Key);

                    if (originalDic == null)
                    {
                        originalDic = new PListDictionary();
                        original[kvp.Key] = originalDic;
                    }

                    if (forceReplace)
                    {
                        original [kvp.Key] = toMergeDic.Copy ();
                        originalDic = original.Element<PListDictionary> (kvp.Key);
                    }
                    else
                    {
                        MergeDictionaries(originalDic, toMergeDic, keepEmpty, forceReplace);
                    }

                    if (!keepEmpty && originalDic.Count <= 0)
                    {
                        original.Remove(kvp.Key);
                    }
                }
                else if (toMergeElement is PListArray)
                {
                    var toMergeArray = toMergeElement as PListArray;
                    var originalArray = original.Element<PListArray>(kvp.Key);

                    if (originalArray == null)
                    {
                        originalArray = new PListArray();
                        original[kvp.Key] = originalArray;
                    }

                    if (forceReplace)
                    {
                        original [kvp.Key] = toMergeArray.Copy ();
                        originalArray = original.Element<PListArray> (kvp.Key);
                    }
                    else
                    {
                        MergeArrays (originalArray, toMergeArray, keepEmpty);
                    }

                    if (!keepEmpty && originalArray.Count <= 0)
                    {
                        original.Remove(kvp.Key);
                    }
                }
                else
                {
                    //skip empty entries
                    //TODO should add empty string?
                    if (string.IsNullOrEmpty(toMergeElement.ToString()))
                    {
                        continue;
                    }

                    //add or overwrite existing value
                    original[kvp.Key] = kvp.Value.Copy();
                }
            }
        }

        IPListElement[] ArrayContainsElement(PListArray original, IPListElement element)
        {
            if (element is PListString)
            {
                var strings = original.Where(o => o is PListString).ToArray();

                if (strings != null)
                {
                    var e = element as PListString;
                    var selected = strings.Where(o => o.Equals(e));

                    if (selected != null && selected.Count() > 0)
                    {
                        return selected.ToArray();
                    }
                }
            }
            else if (element is PListInteger)
            {
                var ints = original.Where(o => o is PListInteger).ToArray();

                if (ints != null)
                {
                    var e = element as PListInteger;
                    var selected = ints.Where(o => o.Equals(e));

                    if (selected != null && selected.Count() > 0)
                    {
                        return selected.ToArray();
                    }
                }
            }
            else if (element is PListBoolean)
            {
                var bools = original.Where(o => o is PListBoolean).ToArray();

                if (bools != null)
                {
                    var e = element as PListBoolean;
                    var selected = bools.Where(o => o.Equals(e));

                    if (selected != null && selected.Count() > 0)
                    {
                        return selected.ToArray();
                    }
                }
            }
            else if (element is PListReal)
            {
                var reals = original.Where(o => o is PListReal).ToArray();

                if (reals != null)
                {
                    var e = element as PListReal;
                    var selected = reals.Where(o => o.Equals(e));

                    if (selected != null && selected.Count() > 0)
                    {
                        return selected.ToArray();
                    }
                }
            }
            else if (element is PListData)
            {
                var datas = original.Where(o => o is PListData).ToArray();

                if (datas != null)
                {
                    var e = element as PListData;
                    var selected = datas.Where(o => o.Equals(e));

                    if (selected != null && selected.Count() > 0)
                    {
                        return selected.ToArray();
                    }
                }
            }
            else if (element is PListDate)
            {
                var dates = original.Where(o => o is PListDate).ToArray();

                if (dates != null)
                {
                    var e = element as PListDate;
                    var selected = dates.Where(o => o.Equals(e));

                    if (selected != null && selected.Count() > 0)
                    {
                        return selected.ToArray();
                    }
                }
            }

            return null;
        }

        void MergeArrays(PListArray original, PListArray toMerge, bool keepEmpty)
        {
            foreach (var element in toMerge)
            {
                if (element is PListDictionary)
                {
                    var originalDic = new PListDictionary();
                    MergeDictionaries(originalDic, element as PListDictionary, keepEmpty, false);

                    if (!keepEmpty && originalDic.Count > 0)
                    {
                        original.Add(originalDic);
                    }
                }
                else if (element is PListArray)
                {
                    var originalArray = new PListArray();
                    MergeArrays(originalArray, element as PListArray, keepEmpty);

                    if (!keepEmpty && originalArray.Count > 0)
                    {
                        original.Add(originalArray);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(element.ToString()))
                    {
                        continue;
                    }

                    var matchedElements = ArrayContainsElement(original, element);

                    if (matchedElements == null)
                    {
                        original.Add(element);
                    }
                }
            }
        }

        #endregion

        #region Capabilities

        bool ApplyCapabilityChanges(CapabilitiesChanges changes)
        {
            var active = changes.ActiveCapabilities();

            try
            {
                foreach (var systemCapability in active)
                {
                    BaseCapability capability = changes.Capability(systemCapability);

                    switch (systemCapability)
                    {
                    case SystemCapability.PushNotifications:
                        ApplyPushNotificationsCapability(capability as PushNotificationsCapability);
                        break;

                    case SystemCapability.AssociatedDomains:
                        ApplyAssociatedDomainsCapability(capability as AssociatedDomainsCapability);
                        break;

                    case SystemCapability.DataProtection:
                        ApplyDataProtectionapability(capability as DataProtectionapability);
                        break;

                    case SystemCapability.HealthKit:
                        ApplyHealthKitCapability(capability as HealthKitCapability);
                        break;

                    case SystemCapability.HomeKit:
                        ApplyHomeKitCapability(capability as HomeKitCapability);
                        break;

                    case SystemCapability.iCloud:
                        ApplyICloudCapability(capability as ICloudCapability);
                        break;

                    case SystemCapability.ApplePay:
                        ApplyApplePayCapability(capability as ApplePayCapability);
                        break;

                    case SystemCapability.NetworkExtensions:
                        ApplyNetworkExtensionsCapability(capability as NetworkExtensionsCapability);
                        break;

                    case SystemCapability.PersonalVPN:
                        ApplyPersonalVPNCapability(capability as PersonalVPNCapability);
                        break;

                    case SystemCapability.Wallet:
                        ApplyWalletCapability(capability as WalletCapability);
                        break;

                    case SystemCapability.Siri:
                        ApplySiriCapability(capability as SiriCapability);
                        break;

                    case SystemCapability.WirelessAccessoryConfiguration:
                        ApplyWirelessAccessoryConfigurationCapability(capability as WirelessAccessoryConfigurationCapability);
                        break;

                    case SystemCapability.AppGroups:
                        ApplyAppGroupsCapability(capability as AppGroupsCapability);
                        break;

                    case SystemCapability.InterAppAudio:
                        ApplyInterAppAudioCapability(capability as InterAppAudioCapability);
                        break;

                    case SystemCapability.KeychainSharing:
                        ApplyKeychainSharingCapability(capability as KeychainSharingCapability);
                        break;

                    case SystemCapability.GameCenter:
                        ApplyGameCenterCapability(capability as GameCenterCapability);
                        break;

                    case SystemCapability.InAppPurchase:
                        ApplyInAppPurchaseCapability(capability as InAppPurchaseCapability);
                        break;

                    case SystemCapability.Maps:
                        ApplyMapsCapability(capability as MapsCapability);
                        break;

                    case SystemCapability.BackgroundModes:
                        ApplyBackgroundModesCapability(capability as BackgroundModesCapability);
                        break;

                    case SystemCapability.HotspotConfiguration:
                        ApplyHotspotConfigurationCapability (capability as HotspotConfigurationCapability);
                        break;

                    case SystemCapability.Multipath:
                        ApplyMultipathCapability (capability as MultipathCapability);
                        break;

                    case SystemCapability.NFCTagReading:
                        ApplyNFCTagReadingCapability (capability as NFCTagReadingCapability);
                        break;

                    case SystemCapability.GameControllers:
                        ApplyGameControllersCapability (capability as GameControllersCapability);
                        break;

                    default:
                        throw new System.ArgumentOutOfRangeException();
                    }
                }

                if (!_pbxproj.Save())
                {
                    Debug.LogError("EgoXproject: Failed to save PBXProject file");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("EgoXproject: Error loading capabilities: " + e.Message);
                return false;
            }
        }

        //iCloud
        void ApplyICloudCapability(ICloudCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.iCloud", true);

            if (capability.CloudKit)
            {
                _pbxproj.AddSystemFramework("CloudKit.framework", LinkType.Required);
                //ensure push is enabled with cloudkit
                ApplyPushNotificationsCapability(new PushNotificationsCapability());
            }

            //update entitlements file
            var entitlementChanges = new PListDictionary();
            var services = new PListArray();
            var ubiquityContainerIds = new PListArray();
            var iCloudContainerIds = new PListArray();

            if (capability.iCloudDocuments)
            {
                services.Add("CloudDocuments");
            }

            if (capability.CloudKit)
            {
                services.Add("CloudKit");
            }

            if (capability.iCloudDocuments || capability.CloudKit)
            {
                if (capability.UseCustomContainers)
                {
                    foreach (var item in capability.CustomContainers)
                    {
                        iCloudContainerIds.Add(item);

                        if (capability.iCloudDocuments)
                        {
                            ubiquityContainerIds.Add(item);
                        }
                    }
                }
                else
                {
                    iCloudContainerIds.Add("iCloud.$(CFBundleIdentifier)");

                    if (capability.iCloudDocuments)
                    {
                        ubiquityContainerIds.Add("iCloud.$(CFBundleIdentifier)");
                    }
                }
            }

            //always add, even if empty
            entitlementChanges.Add("com.apple.developer.icloud-container-identifiers", iCloudContainerIds);

            if (services.Count > 0)
            {
                entitlementChanges.Add("com.apple.developer.icloud-services", services);
            }

            if (ubiquityContainerIds.Count > 0)
            {
                entitlementChanges.Add("com.apple.developer.ubiquity-container-identifiers", ubiquityContainerIds);
            }

            if (capability.KeyValueStorage)
            {
                entitlementChanges.Add("com.apple.developer.ubiquity-kvstore-identifier", "$(TeamIdentifierPrefix)$(CFBundleIdentifier)");
            }

            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //Push Notifications
        void ApplyPushNotificationsCapability(PushNotificationsCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.Push", true);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            entitlementChanges.Add("aps-environment", "development");
            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //Game Center
        void ApplyGameCenterCapability(GameCenterCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.GameCenter.iOS", true);
            _pbxproj.AddSystemFramework("GameKit.framework", LinkType.Required);
            //update info.plist
            var changes = new PListDictionary();
            changes.Add("UIRequiredDeviceCapabilities", new PListArray("gamekit"));
            ApplyInfoPlistChanges(changes);
        }

        //Wallet
        void ApplyWalletCapability(WalletCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.Wallet", true);
            _pbxproj.AddSystemFramework("PassKit.framework", LinkType.Required);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            entitlementChanges.Add("com.apple.developer.pass-type-identifiers", new PListArray("$(TeamIdentifierPrefix)*"));
            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //Siri
        void ApplySiriCapability(SiriCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.Siri", true);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            entitlementChanges.Add("com.apple.developer.siri", true);
            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //Apple Pay (In App Payments)
        void ApplyApplePayCapability(ApplePayCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.ApplePay", true);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            var groups = new PListArray();
            entitlementChanges.Add("com.apple.developer.in-app-payments", groups);

            foreach (var item in capability.MerchantIds)
            {
                groups.Add(item);
            }

            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //In-App Purchase
        void ApplyInAppPurchaseCapability(InAppPurchaseCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.InAppPurchase", true);
            _pbxproj.AddSystemFramework("StoreKit.framework", LinkType.Required);
        }

        //Maps
        void ApplyMapsCapability(MapsCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.Maps.iOS", true);
            _pbxproj.AddSystemFramework("MapKit.framework", LinkType.Required);
            //update info.plist
            var changes = new PListDictionary();
            var modes = new PListArray();

            if (capability.Airplane)
            {
                modes.Add("MKDirectionsModePlane");
            }

            if (capability.Bike)
            {
                modes.Add("MKDirectionsModeBike");
            }

            if (capability.Bus)
            {
                modes.Add("MKDirectionsModeBus");
            }

            if (capability.Car)
            {
                modes.Add("MKDirectionsModeCar");
            }

            if (capability.Ferry)
            {
                modes.Add("MKDirectionsModeFerry");
            }

            if (capability.Other)
            {
                modes.Add("MKDirectionsModeOther");
            }

            if (capability.Pedestrian)
            {
                modes.Add("MKDirectionsModePedestrian");
            }

            if (capability.RideSharing)
            {
                modes.Add("MKDirectionsModeRideShare");
            }

            if (capability.Streetcar)
            {
                modes.Add("MKDirectionsModeStreetCar");
            }

            if (capability.Subway)
            {
                modes.Add("MKDirectionsModeSubway");
            }

            if (capability.Taxi)
            {
                modes.Add("MKDirectionsModeTaxi");
            }

            if (capability.Train)
            {
                modes.Add("MKDirectionsModeTrain");
            }

            if (modes.Count > 0)
            {
                changes.Add("MKDirectionsApplicationSupportedModes", modes);
                var docType = new PListDictionary();
                docType.Add("CFBundleTypeName", "MKDirectionsRequest");
                docType.Add("LSItemContentTypes", new PListArray("com.apple.maps.directionsrequest"));
                var docTypes = new PListArray(docType);
                changes.Add("CFBundleDocumentTypes", docTypes);
                ApplyInfoPlistChanges(changes);
            }
        }

        //Personal VPN
        void ApplyPersonalVPNCapability(PersonalVPNCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.VPNLite", true);
            _pbxproj.AddSystemFramework("NetworkExtension.framework", LinkType.Required);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            var groups = new PListArray();
            groups.Add("allow-vpn");
            entitlementChanges.Add("com.apple.developer.networking.vpn.api", groups);
            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //Network Extensions
        void ApplyNetworkExtensionsCapability(NetworkExtensionsCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.NetworkExtensions.iOS", true);
            _pbxproj.AddSystemFramework("NetworkExtension.framework", LinkType.Required);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            var groups = new PListArray();

            if (capability.AppProxy)
            {
                groups.Add ("app-proxy-provider");
            }

            if (capability.ContentFilter)
            {
                groups.Add ("content-filter-provider");
            }

            if (capability.PacketTunnel)
            {
                groups.Add ("packet-tunnel-provider");
            }

            if (capability.DNSProxy)
            {
                groups.Add ("dns-proxy");
            }

            entitlementChanges.Add("com.apple.developer.networking.networkextension", groups);
            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        void ApplyHotspotConfigurationCapability (HotspotConfigurationCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability ("com.apple.HotspotConfiguration", true);
            _pbxproj.AddSystemFramework ("NetworkExtension.framework", LinkType.Required);
            //update entitlements file
            var entitlementChanges = new PListDictionary ();
            entitlementChanges.Add ("com.apple.developer.networking.HotspotConfiguration", true);
            ApplyEntitlementsChanges (entitlementChanges, true);
        }

        void ApplyMultipathCapability (MultipathCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability ("com.apple.Multipath", true);
            //update entitlements file
            var entitlementChanges = new PListDictionary ();
            entitlementChanges.Add ("com.apple.developer.networking.multipath", true);
            ApplyEntitlementsChanges (entitlementChanges, true);
        }

        void ApplyNFCTagReadingCapability (NFCTagReadingCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability ("com.apple.NearFieldCommunicationTagReading", true);
            //update entitlements file
            var entitlementChanges = new PListDictionary ();
            var formats = new PListArray ();
            formats.Add ("NDEF");
            entitlementChanges.Add ("com.apple.developer.nfc.readersession.formats", formats);
            ApplyEntitlementsChanges (entitlementChanges, true);
        }

        //Background Modes
        void ApplyBackgroundModesCapability(BackgroundModesCapability capability)
        {
            //update pbxproject
            switch (_platform)
            {
            case BuildPlatform.tvOS:
                _pbxproj.EnableSystemCapability ("com.apple.BackgroundModes.appletvos", true);
                break;

            case BuildPlatform.iOS:
            default:
                _pbxproj.EnableSystemCapability ("com.apple.BackgroundModes", true);
                break;
            }

            //update info.plist
            var modes = new PListArray();

            if (capability.UsesBTLEAcc)
            {
                modes.Add("bluetooth-central");
            }

            if (capability.AudioAirplayPIP)
            {
                modes.Add("audio");
            }

            if (capability.ActsAsBTLEAcc)
            {
                modes.Add("bluetooth-peripheral");
            }

            if (capability.ExternalAccComms)
            {
                modes.Add("external-accessory");
            }

            if (capability.BackgroundFetch)
            {
                modes.Add("fetch");
            }

            if (capability.LocationUpdates)
            {
                modes.Add("location");
            }

            if (capability.NewsstandDownloads)
            {
                modes.Add("newsstand-content");
            }

            if (capability.RemoteNotifications)
            {
                modes.Add("remote-notification");
            }

            if (capability.VOIP)
            {
                modes.Add("voip");
            }

            if (modes.Count > 0)
            {
                var changes = new PListDictionary();
                changes.Add("UIBackgroundModes", modes);
                ApplyInfoPlistChanges(changes);
            }

            //update entitlements file
        }

        //Inter-App Audio
        void ApplyInterAppAudioCapability(InterAppAudioCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.InterAppAudio", true);
            _pbxproj.AddSystemFramework("AudioToolbox.framework", LinkType.Required);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            entitlementChanges.Add("inter-app-audio", true);
            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //Keychain Sharing
        void ApplyKeychainSharingCapability(KeychainSharingCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.Keychain", true);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            var groups = new PListArray();
            entitlementChanges.Add("keychain-access-groups", groups);

            foreach (var item in capability.KeychainGroups)
            {
                groups.Add("$(AppIdentifierPrefix)" + item);
            }

            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //Associated Domains
        void ApplyAssociatedDomainsCapability(AssociatedDomainsCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.SafariKeychain", true);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            var domains = new PListArray();
            entitlementChanges.Add("com.apple.developer.associated-domains", domains);

            foreach (var item in capability.AssociatedDomains)
            {
                domains.Add(item);
            }

            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //App Groups
        void ApplyAppGroupsCapability(AppGroupsCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.ApplicationGroups.iOS", true);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            var groups = new PListArray();
            entitlementChanges.Add("com.apple.security.application-groups", groups);

            foreach (var item in capability.AppGroups)
            {
                groups.Add(item);
            }

            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //Data Protection
        void ApplyDataProtectionapability(DataProtectionapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.DataProtection", true);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            entitlementChanges.Add("com.apple.developer.default-data-protection", "NSFileProtectionComplete");
            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //HomeKit
        void ApplyHomeKitCapability(HomeKitCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.HomeKit", true);
            _pbxproj.AddSystemFramework("HomeKit.framework", LinkType.Required);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            entitlementChanges.Add("com.apple.developer.homekit", true);
            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //HealthKit
        void ApplyHealthKitCapability(HealthKitCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.HealthKit", true);
            _pbxproj.AddSystemFramework("HealthKit.framework", LinkType.Required);
            //update info.plist
            var changes = new PListDictionary();
            changes.Add("UIRequiredDeviceCapabilities", new PListArray("healthkit"));
            ApplyInfoPlistChanges(changes);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            entitlementChanges.Add("com.apple.developer.healthkit", true);
            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //Wireless Accessory Configuration
        void ApplyWirelessAccessoryConfigurationCapability(WirelessAccessoryConfigurationCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability("com.apple.WAC", true);
            _pbxproj.AddSystemFramework("ExternalAccessory.framework", LinkType.Required);
            //update entitlements file
            var entitlementChanges = new PListDictionary();
            entitlementChanges.Add("com.apple.external-accessory.wireless-configuration", true);
            ApplyEntitlementsChanges(entitlementChanges, true);
        }

        //Game Controllers
        void ApplyGameControllersCapability (GameControllersCapability capability)
        {
            //update pbxproject
            _pbxproj.EnableSystemCapability ("com.apple.GameControllers.appletvos", true);
            _pbxproj.AddSystemFramework ("GameController.framework", LinkType.Required);
            //update info.plist
            var changes = new PListDictionary();
            changes.Add ("GCSupportsControllerUserInteraction", true);
            ApplyInfoPlistChanges (changes);

            if (capability.GameControllers != null && capability.GameControllers.Length > 0)
            {
                var controllerChanges = new PListDictionary ();
                var controllers = new PListArray ();

                foreach (var c in capability.GameControllers)
                {
                    var dic = new PListDictionary ();
                    dic.Add ("ProfileName", c.ToString ());
                    controllers.Add (dic);
                }

                controllerChanges.Add("GCSupportedGameControllers", controllers);
                ApplyInfoPlistChanges (controllerChanges, true);
            }
        }

        #endregion

        #region Entitlements

        bool ApplyEntitlementsChanges(PListDictionary changes, bool keepEmpty = false)
        {
            if (_pbxproj.EntitlementsFilePaths.Length <= 0)
            {
                string path = Path.Combine(_pbxproj.PathToXcodeProject, _pbxproj.ProductName + ".entitlements");
                _pbxproj.SetEntitlementsFile(path);
            }

            bool success = true;

            foreach (var path in _pbxproj.EntitlementsFilePaths)
            {
                success &= ModifyEntitlementsFile(path, changes, keepEmpty);
            }

            return success;
        }

        bool ModifyEntitlementsFile(string pathToEntitlementsFile, PListDictionary changes, bool keepEmpty = false)
        {
            PList entitlementsFile = new PList();

            if (!File.Exists(pathToEntitlementsFile))
            {
                if (!entitlementsFile.Save(pathToEntitlementsFile, true))
                {
                    Debug.LogError("EgoXproject: Failed to create entitlements file: " + pathToEntitlementsFile);
                    return false;
                }
            }

            if (entitlementsFile.Load(pathToEntitlementsFile))
            {
                MergeDictionaries(entitlementsFile.Root, changes, keepEmpty, false);

                if (entitlementsFile.Save())
                {
                    return true;
                }
                else
                {
                    Debug.LogError("EgoXproject: Failed to save entitlements file: " + pathToEntitlementsFile);
                    return false;
                }
            }
            else
            {
                Debug.LogError("EgoXproject: Failed to open entitlements file: " + pathToEntitlementsFile);
                return false;
            }
        }

        #endregion

        #region Xcode project

        #endregion
    }
}

