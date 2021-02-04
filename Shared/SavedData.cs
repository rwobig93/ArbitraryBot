using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ArbitraryBot.BackEnd;
using ArbitraryBot.Dto;
using Newtonsoft.Json;
using Serilog;

namespace ArbitraryBot.Shared
{
    public class SavedData
    {
        public List<TrackedProduct> TrackedProducts1Min { get; set; } = new List<TrackedProduct>();
        public List<TrackedProduct> TrackedProducts5Min { get; set; } = new List<TrackedProduct>();

        public static StatusReturn Load()
        {
            try
            {
                string saveFile = OSDynamic.GetFilePath(Constants.PathSavedData, "SaveData.json");
                Log.Debug($"saveFile = {saveFile}");
                if (File.Exists(saveFile))
                {
                    Log.Debug("Attempting to load savedData file");
                    var saveDataLoaded = File.ReadAllText(saveFile);
                    Constants.SavedData = JsonConvert.DeserializeObject<SavedData>(saveDataLoaded);
                    Log.Debug("Successfully deserialized savedData file");
                    return StatusReturn.Success;
                }
                else
                {
                    Log.Debug("SavedData file wasn't found");
                    return StatusReturn.NotFound;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed at attempting to load save data file");
                return StatusReturn.Failure;
            }
        }

        public static StatusReturn Save(string saveFile = "")
        {
            try
            {
                if (!Directory.Exists(Constants.PathSavedData))
                {
                    Log.Debug($"SavedData path doesn't exist, attempting to create dir: {Constants.PathSavedData}");
                    Directory.CreateDirectory(Constants.PathSavedData);
                    Log.Information($"Created missing config dir: {Constants.PathSavedData}");
                }

                if (string.IsNullOrWhiteSpace(saveFile))
                {
                    saveFile = OSDynamic.GetFilePath(Constants.PathSavedData, "SaveData.json");
                }
                else
                {
                    var split = saveFile.Split('\\');
                    if (OSDynamic.GetCurrentOS() != OSPlatform.Windows)
                    {
                        split = saveFile.Split('/');
                    }
                    saveFile = OSDynamic.GetFilePath(Constants.PathSavedData, split.Last());
                    if (!saveFile.ToLower().EndsWith(".json"))
                    {
                        saveFile = saveFile += ".json";
                    }
                }
                Log.Debug($"saveFile = {saveFile}");
                if (File.Exists(saveFile))
                {
                    Log.Debug("Attempting to save over save data file");
                }
                else
                {
                    Log.Debug("Attempting to save a new save data file");
                }
                File.WriteAllText(saveFile, JsonConvert.SerializeObject(Constants.SavedData));
                Log.Debug("Successfully serialized savedData file");
                return StatusReturn.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save savedData file");
                return StatusReturn.Failure;
            }
        }

        internal static void Remove()
        {
            string saveFile = OSDynamic.GetFilePath(Constants.PathSavedData, "SaveData.json");
            File.Delete(saveFile);
        }

        public static StatusReturn Backup()
        {
            try
            {
                string backupSaveDataFile = OSDynamic.GetFilePath(Constants.PathSavedData, $"SaveData_{DateTime.Now:yy-MM-dd-H-mm}.json");
                Save(backupSaveDataFile);
                return StatusReturn.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to backup savedData file");
                return StatusReturn.Failure;
            }
        }
        internal static void CreateNew()
        {
            try
            {
                string saveFile = OSDynamic.GetFilePath(Constants.PathSavedData, "SaveData.json");
                if (File.Exists(saveFile))
                {
                    Log.Warning($"SaveData file already exists, will back it up before creating a new one: {saveFile}");
                    Backup();
                    Remove();
                }
                Constants.SavedData = new SavedData();
                Log.Debug("Created new SaveData");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create new savedata file");
            }
        }
    }
}
