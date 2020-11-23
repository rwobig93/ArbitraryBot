using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using System.IO;
using Newtonsoft.Json;

namespace ArbitraryBot.Shared
{
    public class Config
    {
        // Config goes here

        public static StatusReturn LoadConfig()
        {
            Log.Debug("Starting LoadConfig()");
            string configFile = $@"{Constants.PathConfigDefault}\config.json";
            Log.Debug($"configFile = {configFile}");
            if (File.Exists(configFile))
            {
                Log.Debug("Attempting to load config file");
                var configLoaded = File.ReadAllText(configFile);
                Constants.Config = JsonConvert.DeserializeObject<Shared.Config>(configLoaded);
                Log.Information("Successfully deserialized config file");
                return StatusReturn.Success;
            }
            else
            {
                Log.Information("Config file wasn't found");
                return StatusReturn.NotFound;
            }
        }

        public static StatusReturn SaveConfig(string configFile = "")
        {
            Log.Debug("Starting SaveConfig()");
            if (!Directory.Exists(Constants.PathConfigDefault))
            {
                Log.Debug($"Config path doesn't exist, attempting to create dir: {Constants.PathConfigDefault}");
                Directory.CreateDirectory(Constants.PathConfigDefault);
                Log.Information($"Created missing config dir: {Constants.PathConfigDefault}");
            }
            
            if (string.IsNullOrWhiteSpace(configFile))
            {
                configFile = $@"{Constants.PathConfigDefault}\config.json";
            }
            else
            {
                var split = configFile.Split('\\');
                configFile = $@"{Constants.PathConfigDefault}\{split.Last()}";
                if (!configFile.ToLower().EndsWith(".json"))
                {
                    configFile = configFile += ".json";
                }
            }
            Log.Debug($"configFile = {configFile}");
            if (File.Exists(configFile))
            {
                Log.Information("Attempting to save over current config file");
            }
            else
            {
                Log.Information("Attempting to save a new config file");
            }
            File.WriteAllText(configFile, JsonConvert.SerializeObject(Constants.Config));
            Log.Information("Successfully serialized config file");
            return StatusReturn.Success;
        }

        internal static void CreateNew()
        {
            try
            {
                string configFile = $@"{Constants.PathConfigDefault}\config.json";
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

        private static void Remove()
        {
            string configFile = $@"{Constants.PathConfigDefault}\config.json";
            File.Delete(configFile);
        }

        private static void Backup()
        {
            string configFile = $@"{Constants.PathConfigDefault}\config.json";
            string backupConfigFile = $@"{Constants.PathConfigDefault}\config_{DateTime.Now.ToString("yy-MM-dd-H-mm")}.json";
            SaveConfig(backupConfigFile);
        }
    }
}
