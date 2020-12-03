using System;
using System.Linq;
using Serilog;
using System.IO;
using Newtonsoft.Json;
using ArbitraryBot.BackEnd;
using System.Runtime.InteropServices;
using ArbitraryBot.FrontEnd;

namespace ArbitraryBot.Shared
{
    public class Config
    {
        public bool BetaUpdates { get; set; } = false;
        public byte[] KeyA { get; set; }
        public byte[] KeyB { get; set; }
        public byte[] KeyC { get; set; }
        public string SMTPEmailFrom { get; set; }
        public string SMTPEmailName { get; set; }
        public string SMTPUrl { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public int SMTPPort { get; set; }

        public static StatusReturn Load()
        {
            string configFile = OSDynamic.GetFilePath(Constants.PathConfigDefault, "Config.json");
            Log.Debug($"configFile = {configFile}");
            if (File.Exists(configFile))
            {
                Log.Debug("Attempting to load config file");
                var configLoaded = File.ReadAllText(configFile);
                Constants.Config = UnsecureSensitiveProperties(JsonConvert.DeserializeObject<Config>(configLoaded));
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
            Config finalConfig = SecureSensitiveProperties(Constants.Config);
            File.WriteAllText(configFile, JsonConvert.SerializeObject(finalConfig));
            Log.Information("Successfully serialized config file");
            return StatusReturn.Success;
        }

        private static Config SecureSensitiveProperties(Config config)
        {
            if (config.KeyA == null || config.KeyB == null || config.KeyC == null)
            {
                GenerateKeys();
            }

            config.SMTPUsername = AESThenHMAC.SimpleEncrypt(config.SMTPUsername, config.KeyB, config.KeyA, config.KeyC);
            config.SMTPPassword = AESThenHMAC.SimpleEncrypt(config.SMTPPassword, config.KeyA, config.KeyC, config.KeyB);
            config.SMTPUrl = AESThenHMAC.SimpleEncrypt(config.SMTPUrl, config.KeyC, config.KeyA, config.KeyB);
            config.SMTPEmailFrom = AESThenHMAC.SimpleEncrypt(config.SMTPEmailFrom, config.KeyB, config.KeyC, config.KeyA);
            config.SMTPEmailName = AESThenHMAC.SimpleEncrypt(config.SMTPEmailName, config.KeyC, config.KeyB, config.KeyA);
            return config;
        }

        private static Config UnsecureSensitiveProperties(Config config)
        {
            if (config.KeyA == null || config.KeyB == null || config.KeyC == null)
            {
                Log.Fatal("No keys are viable, something happened w/ the config file and we can't recover Email settings");
                Handler.NotifyError("[Fatal] Encryption Keys aren't viable/are missing, we can't recover Email settings, you will need to reconfigure them.");
                Console.WriteLine("[Fatal] Encryption Keys aren't viable/are missing, we can't recover Email settings, you will need to reconfigure them. Config is being backed up in case you believe you can recover the keys in the config directory/folder");
                Backup();
                UI.StopForMessage();
                return null;
            }

            config.SMTPUsername = AESThenHMAC.SimpleDecrypt(config.SMTPUsername, config.KeyB, config.KeyA, config.KeyC.Length);
            config.SMTPPassword = AESThenHMAC.SimpleDecrypt(config.SMTPPassword, config.KeyA, config.KeyC, config.KeyB.Length);
            config.SMTPUrl = AESThenHMAC.SimpleDecrypt(config.SMTPUrl, config.KeyC, config.KeyA, config.KeyB.Length);
            config.SMTPEmailFrom = AESThenHMAC.SimpleDecrypt(config.SMTPEmailFrom, config.KeyB, config.KeyC, config.KeyA.Length);
            config.SMTPEmailName = AESThenHMAC.SimpleDecrypt(config.SMTPEmailName, config.KeyC, config.KeyB, config.KeyA.Length);
            return config;
        }

        private static void GenerateKeys()
        {
            Log.Verbose("Generating new keys for config");
            Constants.Config.KeyB = AESThenHMAC.NewKey();
            Constants.Config.KeyA = AESThenHMAC.NewKey();
            Constants.Config.KeyC = AESThenHMAC.NewKey();
            Log.Information("Generated new keys for config");
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
                Handler.NotifyError(ex, "ConfigCreate");
            }
        }

        internal static void Remove()
        {
            string configFile = OSDynamic.GetFilePath(Constants.PathConfigDefault, "Config.json");
            File.Delete(configFile);
        }

        internal static void Backup()
        {
            string backupConfigFile = OSDynamic.GetFilePath(Constants.PathSavedData,  $"Config_{DateTime.Now:yy-MM-dd-H-mm}.json");
            Save(backupConfigFile);
        }
    }
}
