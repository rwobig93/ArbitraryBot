using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using ArbitraryBot.Shared;
using ArbitraryBot.Extensions;
using System.IO;
using System.Runtime.InteropServices;

namespace ArbitraryBot.BackEnd
{
    public static class Core
    {
        internal static void InitializeLogger()
        {
            #if DEBUG
            Constants.LogLevelCloud.MinimumLevel = LogEventLevel.Debug;
            Constants.LogLevelLocal.MinimumLevel = LogEventLevel.Debug;
            #endif

            Log.Logger = new LoggerConfiguration()
                //.MinimumLevel.ControlledBy(Constants.LogLevelLocal)
                .WriteTo.Async(c => c.File($"{Constants.PathLogs}\\{OSDynamic.GetProductAssembly().ProductName}_.log", rollingInterval: RollingInterval.Day,
                  outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                  levelSwitch: Constants.LogLevelLocal))
                .WriteTo.Async(c => c.Seq("http://dev.wobigtech.net:5341", apiKey: "xBJXeoMOJzEwG1HBuxgN", controlLevelSwitch: Constants.LogLevelCloud))
                .WriteTo.Console(levelSwitch: Constants.LogLevelConsole,
                  outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}")
                .Enrich.WithCaller()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProperty("Application", OSDynamic.GetProductAssembly().ProductName)
                .CreateLogger();

            ChangeLoggingLevelLocal();
            ChangeLoggingLevelCloud();
            ChangeLoggingLevelConsole();

            Log.Information("==START-STOP== Application Started");
        }

        public static void SaveEverything()
        {
            Log.Verbose("Attempting to save all application data");
            Config.Save();
            SavedData.Save();
            Log.Debug("Saved all application data");
        }

        public static void CleanupAllOldFiles()
        {
            Log.Debug("Attempting to cleanup all old files");
            HouseKeeping.CleanupOldFiles(AppFile.Config);
            HouseKeeping.CleanupOldFiles(AppFile.Log);
            Log.Information("Cleaned up all old files");
        }

        internal static void BackupEverything()
        {
            Log.Debug("Attempting to backup everything");
            HouseKeeping.BackupFiles(AppFile.Config);
            HouseKeeping.BackupFiles(AppFile.SavedData);
            Log.Information("Successfully backed up everything");
        }

        public static void StartServices()
        {
            Log.Debug("Attempting to start all services");
            Jobs.InitializeJobService();
            Jobs.StartAllTimedJobs();
            //Watcher.StartWatcherThread();
            Log.Information("Finished Starting Services");
        }

        internal static void ProcessSettingsFromConfig()
        {
            HouseKeeping.ValidateAllFilePaths();
            CleanupAllOldFiles();
            // TODO: Put setting changes from config load
        }

        internal static void InitializeFirstRun()
        {
            Config.CreateNew();
            SavedData.CreateNew();
            if (Constants.CloseApp)
            {
                return;
            }
            Config.Save();
            SavedData.Save();
            Log.Information("Finished Initializing First App Run");
        }

        internal static void ChangeLoggingLevelLocal(LogEventLevel logLevel = LogEventLevel.Information)
        {
            #if DEBUG
            logLevel = LogEventLevel.Debug;
            #endif

            if (Constants.LogLevelLocal == null)
            {
                Constants.LogLevelLocal = new Serilog.Core.LoggingLevelSwitch(logLevel);
                Log.Warning("Logging Level for local was null and had to be initialized");
            }
            else
            {
                Constants.LogLevelLocal.MinimumLevel = logLevel;
            }
        }

        internal static void ChangeLoggingLevelConsole(LogEventLevel logLevel = LogEventLevel.Error)
        {

            if (Constants.LogLevelConsole == null)
            {
                Constants.LogLevelConsole = new Serilog.Core.LoggingLevelSwitch(logLevel);
                Log.Warning("Logging Level for console was null and had to be initialized");
            }
            else
            {
                Constants.LogLevelConsole.MinimumLevel = logLevel;
            }
        }

        internal static void ChangeLoggingLevelCloud(LogEventLevel logLevel = LogEventLevel.Warning)
        {
            #if DEBUG
            logLevel = LogEventLevel.Debug;
            #endif

            if (Constants.LogLevelCloud == null)
            {
                Constants.LogLevelCloud = new Serilog.Core.LoggingLevelSwitch(logLevel);
                Log.Warning("Logging Level for cloud was null and had to be initialized");
            }
            else
            {
                Constants.LogLevelCloud.MinimumLevel = logLevel;
            }
        }

        internal static void InitializeApp()
        {
            HouseKeeping.ValidateAllFilePaths(true);
        }

        internal static StatusReturn OpenDir(AppFile appFile)
        {
            try
            {
                if (OSDynamic.GetCurrentOS() == OSPlatform.Windows)
                {
                    var file = FileType.GetFileType(appFile);
                    OSDynamic.OpenPath(file.Directory);
                    return StatusReturn.Success;
                }
                else
                {
                    return StatusReturn.Failure;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to open directory");
                return StatusReturn.Failure;
            }
        }

        internal static StatusReturn LoadAllFiles()
        {
            StatusReturn confStatus = Config.Load();
            StatusReturn saveStatus = SavedData.Load();
            if (confStatus != StatusReturn.Success && saveStatus != StatusReturn.Success)
            {
                Log.Information("Neither a config or savedata file was found", confStatus, saveStatus);
                return StatusReturn.NotFound;
            }
            else if (confStatus != StatusReturn.Success && saveStatus == StatusReturn.Success)
            {
                Log.Information("A config file wasn't found but a savedata file was, generating config file", confStatus, saveStatus);
                Config.CreateNew();
                return StatusReturn.Success;
            }
            else if (saveStatus != StatusReturn.Success && confStatus == StatusReturn.Success)
            {
                Log.Information("A savedata file wasn't found but a config file was, generating savedata file", confStatus, saveStatus);
                SavedData.CreateNew();
                return StatusReturn.Success;
            }
            else
            {
                Log.Information("All files were found and loaded successfully", confStatus, saveStatus);
                return StatusReturn.Success;
            }
        }
    }
}
