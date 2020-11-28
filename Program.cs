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
            Core.InitializeApp();

            Core.InitializeLogger();

            Handler.ParseLaunchArgs(args);

            if (Core.LoadAllFiles() != StatusReturn.Success)
            {
                Core.InitializeFirstRun();
            }

            Core.ProcessSettingsFromConfig();

            Core.StartServices();

            UI.ShowMenuRoot();

            Handler.CatchAppClose();
        }
    }
}
