using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using System.IO;
using Newtonsoft.Json;
using ArbitraryBot.BackEnd;
using System.Runtime.InteropServices;

namespace ArbitraryBot.Shared
{
    public class Config
    {
        // Config goes here

        public static StatusReturn Load()
        {
            string configFile = OSDynamic.GetFilePath(Constants.PathConfigDefault, "Config.json");
            Log.Debug($"configFile = {configFile}");
            if (File.Exists(configFile))
            {
                Log.Debug("Attempting to load config file");
                var configLoaded = File.ReadAllText(configFile);
                Constants.Config = JsonConvert.DeserializeObject<Config>(configLoaded);
                Log.Information("Successfully deserialized config file");
                return StatusReturn.Success;
            }
            else
            {
                Log.Debug("Config file wasn't found");
                return StatusReturn.NotFound;
            }
        }

        public static StatusReturn Save(string configFile = "")
        {
            if (!Directory.Exists(Constants.PathConfigDefault))
            {
                Log.Debug($"Config path doesn't exist, attempting to create dir: {Constants.PathConfigDefault}");
                Directory.CreateDirectory(Constants.PathConfigDefault);
                Log.Information($"Created missing config dir: {Constants.PathConfigDefault}");
            }
            
            if (string.IsNullOrWhiteSpace(configFile))
            {
                configFile = OSDynamic.GetFilePath(Constants.PathConfigDefault, "Config.json");
            }
            else
            {
                var split = configFile.Split('\\');
                if (OSDynamic.GetCurrentOS() != OSPlatform.Windows)
                {
                    split = configFile.Split('/');
                }
                configFile = OSDynamic.GetFilePath(Constants.PathConfigDefault, split.Last());
                if (!configFile.ToLower().EndsWith(".json"))
                {
                    configFile = configFile += ".json";
                }
            }
            Log.Debug($"configFile = {configFile}");
            if (File.Exists(configFile))
            {
                Log.Debug("Attempting to save over current config file");
            }
            else
            {
                Log.Debug("Attempting to save a new config file");
            }
            File.WriteAllText(configFile, JsonConvert.SerializeObject(Constants.Config));
            Log.Information("Successfully serialized config file");
            return StatusReturn.Success;
        }

        internal static void CreateNew()
        {
            try
            {
                string configFile = OSDynamic.GetFilePath(Constants.PathConfigDefault, "Config.json");
                if (File.Exists(configFile))
                {
                    Log.Warning($"Config file already exists, will back it up before creating a new one: {configFile}");
                    Backup();
                    Remove();
                }
                Constants.Config = new Config();
                Log.Information("Created new config");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create new config file");
            }
        }

        internal static void Remove()
        {
            string configFile = OSDynamic.GetFilePath(Constants.PathConfigDefault, "Config.json");
            File.Delete(configFile);
        }

        internal static void Backup()
        {
            string backupConfigFile = OSDynamic.GetFilePath(Constants.PathSavedData,  $"Config_{DateTime.Now.ToString("yy-MM-dd-H-mm")}.json");
            Save(backupConfigFile);
        }
    }
}
