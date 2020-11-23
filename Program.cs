using System;
using ArbitraryBot.FrontEnd;
using ArbitraryBot.BackEnd;
using ArbitraryBot.Shared;

namespace ArbitraryBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.InitializeLogger();

            Handler.ParseLaunchArgs(args);

            if (Config.LoadConfig() == StatusReturn.NotFound)
            {
                Core.InitializeFirstRun();
            }

            Core.ProcessSettingsFromConfig();

            Core.StartServices();

            UI.DisplayHostInfo();

            UI.ShowMenuRoot();

            Handler.CatchAppClose();
        }
    }
}
