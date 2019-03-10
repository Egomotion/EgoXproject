// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Egomotion.EgoXproject.Internal
{
    internal class Upgrader
    {
        #region Core
        const string VERSION_FILENAME = "egoxproject.version";
        const string VERSION_KEY = "egoXprojectVersion";

        public void Upgrade()
        {
            var path = Path.Combine(DllUtils.DllLocation(), VERSION_FILENAME);
            var versionDic = LoadVersionFile(path);

            if (!UpgradeCheckRequired(versionDic))
            {
                return;
            }

            bool upgraded = false;
            //Do Upgrading here
            upgraded |= CheckSettingsForUpgrade();
            upgraded |= CheckChangeFilesForUpgrade();
            //End of upgrading
            SaveVersionFile(path);

            if (upgraded)
            {
                Debug.LogWarning ("EgoXproject upgraded: Please check that everything is correct. The extension \".bak\" has been added to the old version of the upgraded files. They can be safely deleted.");
            }
        }

        bool UpgradeCheckRequired(Dictionary<string, string> dic)
        {
            //check to see if we have the version file
            if (dic == null)
            {
                return true;
            }

            string version;

            if (dic.TryGetValue(VERSION_KEY, out version))
            {
                return DllUtils.IsNewer(version);
            }

            //failed to verify it is the same version, so force a check
            return true;
        }

        Dictionary<string, string> LoadVersionFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var dic = new Dictionary<string, string>();
            var lines = File.ReadAllLines(path);
            var separator = new char[] {':'};

            foreach (var line in lines)
            {
                var kvp = line.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

                if (kvp.Length != 2)
                {
                    continue;
                }

                var key = kvp[0].Trim();
                var value = kvp[1].Trim();
                dic[key] = value;
            }

            return dic;
        }

        void SaveVersionFile(string path)
        {
            List<string> entries = new List<string>();
            entries.Add(VERSION_KEY + ": " + DllUtils.Version());
            File.WriteAllLines(path, entries.ToArray());
            AssetDatabase.ImportAsset(ProjectUtil.MakePathRelativeToProject(path));
        }

        void BackupFile(string fileName)
        {
            string dstFileName = fileName + ".bak";
            int counter = 1;

            while (File.Exists(dstFileName))
            {
                dstFileName = fileName + " " + counter.ToString() + ".bak";
                counter++;
            }

            var from = ProjectUtil.MakePathRelativeToProject(fileName);
            var to = ProjectUtil.MakePathRelativeToProject(dstFileName);
            bool success = AssetDatabase.CopyAsset(from, to);

            if (!success)
            {
                Debug.LogError("EgoXproject: Failed to backup file " + from + " to " + to + ".");
            }
        }

        void BackupFiles(string[] fileNames)
        {
            foreach (var fileName in fileNames)
            {
                BackupFile(fileName);
            }
        }

        #endregion

        #region Settings Upgrade

        bool CheckSettingsForUpgrade()
        {
            string[] settingsFiles = Directory.GetFiles(Application.dataPath, "settings.egoxconfig", SearchOption.AllDirectories);

            if (settingsFiles == null || settingsFiles.Length <= 0)
            {
                return false;
            }

            BackupFiles(settingsFiles);
            UpgradeSettingsFiles(settingsFiles);
            return true;
        }

        void UpgradeSettingsFiles(string[] settingsFiles)
        {
            foreach (var fileName in settingsFiles)
            {
                UpgradeSettingsFile(fileName);
            }
        }

        void UpgradeSettingsFile(string fileName)
        {
            var plist = new PList();

            if (!plist.Load(fileName))
            {
                return;
            }

            if (plist.Root.StringValue("Type") != "EgoXproject Settings")
            {
                return;
            }

            if (plist.Root.IntValue("Version") != 1 && plist.Root.IntValue("Version") != 2)
            {
                return;
            }

            var dirName = Path.GetDirectoryName(fileName);
            var configurations = new XcodeConfigurations(dirName);
            var platformConfig = configurations.Configuration(BuildPlatform.iOS);
            var configs = plist.Root.DictionaryValue("Configurations");

            if (configs != null)
            {
                foreach (var kvp in configs)
                {
                    var entries = kvp.Value as PListArray;

                    if (entries == null || entries.Count <= 0)
                    {
                        platformConfig.AddConfiguration(kvp.Key);
                        continue;
                    }

                    for (int ii = 0; ii < entries.Count; ++ii)
                    {
                        platformConfig.AddChangeFileToConfiguration(entries.StringValue(ii), kvp.Key);
                    }
                }
            }

            var active = plist.Root.StringValue("ActiveConfiguration");

            if (!string.IsNullOrEmpty(active))
            {
                platformConfig.ActiveConfiguration = active;
            }

            configurations.Save();
            plist.Root.Remove("Configurations");
            plist.Root.Remove("ActiveConfiguration");
            plist.Save();
            string oldPath = ProjectUtil.MakePathRelativeToProject(plist.SavePath);
            string newPath = ProjectUtil.MakePathRelativeToProject(Path.Combine(Path.GetDirectoryName(oldPath), "egoxproject.settings"));
            AssetDatabase.MoveAsset(oldPath, newPath);
        }

        #endregion

        #region Change Files

        bool CheckChangeFilesForUpgrade()
        {
            var changeFiles = Directory.GetFiles(Application.dataPath, "*.egoxc", SearchOption.AllDirectories);

            if (changeFiles == null || changeFiles.Length <= 0)
            {
                return false;
            }

            UpgradeChangeFiles(changeFiles);
            return true;
        }

        void UpgradeChangeFiles(string[] changeFiles)
        {
            foreach (var changeFile in changeFiles)
            {
                UpgradeChangeFile(changeFile);
            }
        }

        void UpgradeChangeFile(string fileName)
        {
            var plist = new PList();

            if (!plist.Load(fileName))
            {
                return;
            }

            if (plist.Root.StringValue("Type") != "EgoXproject Change File")
            {
                return;
            }

            if (plist.Root.IntValue("Version") != 1)
            {
                return;
            }

            BackupFile(fileName);
            var changeFile = XcodeChangeFile.Load(fileName);

            if (changeFile == null)
            {
                return;
            }

            changeFile.Platform = BuildPlatform.iOS;
            //TODO handle upgrade failure
            //move custom frameworks to files and folder section
            UpgradeCustomFrameworks(plist, changeFile);
            //move files and folders to a single entries list
            UpgradeFiles(plist, changeFile);
            UpgradeFolders(plist, changeFile);
            changeFile.Save();
        }

        void UpgradeCustomFrameworks(PList plist, XcodeChangeFile changeFile)
        {
            var frameworks = plist.Root.DictionaryValue("Frameworks");

            if (frameworks == null)
            {
                return;
            }

            var customFrameworks = frameworks.ArrayValue("Custom");

            if (customFrameworks == null)
            {
                return;
            }

            for (int ii = 0; ii < customFrameworks.Count; ii++)
            {
                var customFrameworkDic = customFrameworks.DictionaryValue(ii);

                if (customFrameworkDic == null)
                {
                    continue;
                }

                var entry = FileAndFolderEntryFactory.CreateFromObsolete(customFrameworkDic);

                if (entry == null)
                {
                    continue;
                }

                changeFile.FilesAndFolders.Upgrader_AddEntry(entry);
            }
        }

        void UpgradeFiles(PList plist, XcodeChangeFile changeFile)
        {
            var filesFolders = plist.Root.DictionaryValue("FilesAndFolders");

            if (filesFolders == null)
            {
                return;
            }

            var files = filesFolders.ArrayValue("Files");

            if (files == null)
            {
                return;
            }

            for (int ii = 0; ii < files.Count; ii++)
            {
                var fileDic = files.DictionaryValue(ii);

                if (fileDic == null)
                {
                    continue;
                }

                var entry = FileAndFolderEntryFactory.CreateFromObsolete(fileDic);

                if (entry == null)
                {
                    continue;
                }

                changeFile.FilesAndFolders.Upgrader_AddEntry(entry);
            }
        }

        void UpgradeFolders(PList plist, XcodeChangeFile changeFile)
        {
            var filesFolders = plist.Root.DictionaryValue("FilesAndFolders");

            if (filesFolders == null)
            {
                return;
            }

            var folders = filesFolders.ArrayValue("Folders");

            if (folders == null)
            {
                return;
            }

            for (int ii = 0; ii < folders.Count; ii++)
            {
                var folderDic = folders.DictionaryValue(ii);

                if (folderDic == null)
                {
                    continue;
                }

                var entry = FileAndFolderEntryFactory.CreateFromObsolete(folderDic);

                if (entry == null)
                {
                    continue;
                }

                changeFile.FilesAndFolders.Upgrader_AddEntry(entry);
            }
        }

        #endregion
    }
}
