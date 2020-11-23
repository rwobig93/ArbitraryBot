using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }

        internal static void ParseLaunchArgs(string[] args)
        {
            foreach (var arg in args)
            {
                Log.Information($"Launch arg passed: {arg}");
                
                if (arg.ToLower() == "-debug")
                {
                    Core.ChangeLoggingLevel(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was -debug, changed logging to debug");
                }
                if (arg.ToLower() == "-consoledebug")
                {
                    Core.ChangeLoggingLevelConsole(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was -consoledebug, changed console logging to debug");
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
    }
}
