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

        internal static void StartServices()
        {
            HouseKeeping.CleanupOldFiles(AppFile.Config);
            Watcher.StartWatcherThread();
        }

        internal static void ProcessSettingsFromConfig()
        {
            HouseKeeping.ValidateAllFilePaths();
        }

        internal static void InitializeFirstRun()
        {
            Config.CreateNew();
            if (Constants.CloseApp)
            {
                return;
            }
            Log.Information("Finished Initializing First App Run");
        }

        internal static void ChangeLoggingLevel(LogEventLevel logLevel = LogEventLevel.Information)
        {
            if (Constants.LogLevel == null)
            {
                #if DEBUG
                logLevel = LogEventLevel.Debug;
                #endif
                Constants.LogLevel = new Serilog.Core.LoggingLevelSwitch(logLevel);
            }
            else
            {
                Constants.LogLevel.MinimumLevel = logLevel;
            }
        }
    }
}
