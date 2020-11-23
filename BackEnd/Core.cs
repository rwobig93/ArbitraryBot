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

namespace ArbitraryBot.BackEnd
{
    public static class Core
    {
        internal static void InitializeLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(Constants.LogLevel)
                .WriteTo.Async(c => c.File($"{Constants.PathLogs}\\{OSDynamic.GetProductAssembly().ProductName}.log", rollingInterval: RollingInterval.Day,
                  outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))
                .WriteTo.Async(c => c.Seq("http://dev.wobigtech.net:5341", apiKey: "xBJXeoMOJzEwG1HBuxgN"))
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

            ChangeLoggingLevel();
            Log.Information("Logger started");
        }

        internal static void SaveEverything()
        {
            Log.Verbose("Attempting to save all application data");
            Config.Save();
            SavedData.Save();
            Log.Debug("Saved all application data");
        }

        internal static void CleanupAllOldFiles()
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

        internal static void TestMethod()
        {
            string msg = "TESTING: This should appear every min";
            Log.Information($"LOG_{msg}");
            Console.WriteLine($"CON_{msg}");
        }

        internal static void ProcessSettingsFromConfig()
        {
            HouseKeeping.ValidateAllFilePaths();
            // TODO: Put setting changes from config load
        }

        internal static void InitializeFirstRun()
        {
            Config.CreateNew();
            if (Constants.CloseApp)
            {
                return;
            }
            Config.Save();
            Log.Information("Finished Initializing First App Run");
        }

        internal static void ChangeLoggingLevel(LogEventLevel logLevel = LogEventLevel.Information)
        {
            #if DEBUG
            logLevel = LogEventLevel.Debug;
            #endif

            if (Constants.LogLevel == null)
            {
                Constants.LogLevel = new Serilog.Core.LoggingLevelSwitch(logLevel);
                Log.Warning("Logging Level was null and had to be initialized");
            }
            else
            {
                Constants.LogLevel.MinimumLevel = logLevel;
            }
        }

        internal static void ChangeLoggingLevelConsole(LogEventLevel logLevel)
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

        internal static void InitializeApp()
        {
            HouseKeeping.ValidateAllFilePaths(true);
        }
    }
}
