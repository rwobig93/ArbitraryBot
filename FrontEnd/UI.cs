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
    public static class UI
    {
        internal static void ShowMenuRoot()
        {
            Log.Debug("Presenting Menu Root");
            while (!Constants.CloseApp)
            {
                Console.WriteLine(
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "|  Enter the corresponding menu number for the action you want to perform:  |{0}" +
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "|  1. Add Watcher                                                           |{0}" +
                    "|  2. Update Watcher                                                        |{0}" +
                    "|  3. Remove Watcher                                                        |{0}" +
                    "|  4. Open Directory                                                        |{0}" +
                    "|  5. Close Bot                                                             |{0}" +
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "{0}Option: ", Environment.NewLine);
                var answer = Console.ReadLine();
                Log.Debug($"Menu answer was: {answer}");
                if (!int.TryParse(answer, out int intAnswer))
                {
                    Log.Debug("Menu answer entered was an invalid response");
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else
                {
                    Log.Debug($"Valid menu option {intAnswer} was entered");
                    switch (intAnswer)
                    {
                        case 1:
                            ShowMenuAddWatcher();
                            break;
                        case 2:
                            ShowMenuUpdateWatcher();
                            break;
                        case 3:
                            ShowMenuRemoveWatcher();
                            break;
                        case 4:
                            ShowMenuOpenDirectory();
                            break;
                        case 5:
                            Handler.CloseApp();
                            break;
                        default:
                            Log.Information("Answer entered wasn't a valid presented option");
                            Console.WriteLine("Answer entered isn't one of the options, please press enter and try again");
                            Console.ReadLine();
                            break;
                    }
                }
                Console.Clear();
            }
            Log.Information("Exited menu root");
        }

        private static void ShowMenuOpenDirectory()
        {
            bool menuClose = false;
            Log.Debug("Presenting Menu OpenDirectory");
            while (!menuClose)
            {
                Console.WriteLine(
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "|  Enter the corresponding menu number for the action you want to perform:  |{0}" +
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "|  1. Open Config Directory                                                 |{0}" +
                    "|  2. Open Log Directory                                                    |{0}" +
                    "|  3. Open SaveData Directory                                               |{0}" +
                    "|  4. Back to Main Menu                                                     |{0}" +
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "{0}Option: ", Environment.NewLine);
                var answer = Console.ReadLine();
                Log.Debug($"Menu answer was: {answer}");
                if (!int.TryParse(answer, out int intAnswer))
                {
                    Log.Debug("Menu answer entered was an invalid response");
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else
                {
                    Log.Debug($"Valid menu option {intAnswer} was entered");
                    switch (intAnswer)
                    {
                        case 1:
                            Core.OpenDir(AppFile.Config);
                            break;
                        case 2:
                            Core.OpenDir(AppFile.Log);
                            break;
                        case 3:
                            Core.OpenDir(AppFile.SavedData);
                            break;
                        case 4:
                            menuClose = true;
                            break;
                        default:
                            Log.Information("Answer entered wasn't a valid presented option");
                            Console.WriteLine("Answer entered isn't one of the options, please press enter and try again");
                            Console.ReadLine();
                            break;
                    }
                }
                Console.Clear();
            }
            Log.Information("Exited Menu OpenDirectory");
        }

        private static void ShowMenuRemoveWatcher()
        {
            throw new NotImplementedException();
        }

        private static void ShowMenuUpdateWatcher()
        {
            throw new NotImplementedException();
        }

        private static void ShowMenuAddWatcher()
        {
            throw new NotImplementedException();
        }

        internal static void DisplayHostInfo()
        {
            Log.Debug("Displaying host info");
            Console.WriteLine(
                $"{Environment.NewLine}" +
                $"Hostname:                {Environment.MachineName}{Environment.NewLine}" +
                $"Current OS Platform:     {OSDynamic.GetCurrentOS()}{Environment.NewLine}" +
                $"Current OS Architecture: {RuntimeInformation.OSArchitecture}{Environment.NewLine}" +
                $"Current OS Description:  {RuntimeInformation.OSDescription}{Environment.NewLine}" +
                $"Current Process Arch:    {RuntimeInformation.ProcessArchitecture}{Environment.NewLine}" +
                $"Current Framework:       {RuntimeInformation.FrameworkDescription}{Environment.NewLine}" +
                $"Logging Path:            {Constants.PathLogs}{Environment.NewLine}" +
                $"Config Path:             {Constants.PathConfigDefault}{Environment.NewLine}");
            Log.Information("Host info Displayed");
        }
        internal static bool PromptYesNo(string question)
        {
            Log.Debug($"Asking PromptYesNo({question})");
            bool answered = false;
            string answer = "";
            while (!answered)
            {
                Console.Write($"{question} [y/n]? ");
                answer = Console.ReadLine().ToLower();
                if (answer != "y" && answer != "n")
                {
                    Log.Debug($"Answer was invalid: {answer}");
                    Console.WriteLine("You entered an invalid response, please try again");
                }
                else
                {
                    answered = true;
                }
            }
            Log.Information($"Prompt YesNo answer was: {answer}");
            if (answer == "y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
