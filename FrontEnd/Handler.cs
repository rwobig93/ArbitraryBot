using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbitraryBot.BackEnd;
using Serilog;

namespace ArbitraryBot.FrontEnd
{
    public static class Handler
    {
        internal static void CatchAppClose()
        {
            Console.WriteLine("App is closing! Press any key to close");
            Console.ReadLine();
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
            }
        }
    }
}
