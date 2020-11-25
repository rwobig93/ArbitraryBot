using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArbitraryBot.BackEnd;
using ArbitraryBot.Shared;
using Serilog;

namespace ArbitraryBot.FrontEnd
{
    public static class Handler
    {
        internal static void CatchAppClose()
        {
            Core.SaveEverything();
            Jobs.StopJobService();
            Log.Information("==START-STOP== Application Stopped");
            Log.CloseAndFlush();
        }

        internal static void ParseLaunchArgs(string[] args)
        {
            foreach (var arg in args)
            {
                Log.Information($"Launch arg passed: {arg}");
                
                if (arg.ToLower() == "-debug")
                {
                    Core.ChangeLoggingLevelLocal(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was -debug, changed local logging to debug");
                }
                if (arg.ToLower() == "-debugconsole")
                {
                    Core.ChangeLoggingLevelConsole(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was -debugconsole, changed console logging to debug");
                }
                if (arg.ToLower() == "-debugcloud")
                {
                    Core.ChangeLoggingLevelCloud(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was -debugcloud, changed cloud logging to debug");
                }
            }
        }

        internal static void CloseApp()
        {
            var answer = UI.PromptYesNo("Are you sure you want to close the bot?");
            if (answer)
            {
                Constants.CloseApp = true;
            }
            Core.SaveEverything();
        }

        internal static Alert SelectAlertFromChoice(int alertAnswer)
        {
            switch (alertAnswer)
            {
                case 1:
                    return Alert.Webhook;
                case 2:
                    return Alert.Email;
                default:
                    Log.Warning("Default was hit on switch that shouldn't occur");
                    return Alert.Email;
            }
        }
    }
}
