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
                    "|                                 Main Menu                                 |{0}" +
                    "|  -----------------------------------------------------------------------  |{0}" +
                    "|  Enter the corresponding menu number for the action you want to perform:  |{0}" +
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "|  1. Add Watcher                                                           |{0}" +
                    "|  2. Modify Watcher                                                        |{0}" +
                    "|  3. Open Directory                                                        |{0}" +
                    "|  4. Close Bot                                                             |{0}" +
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
                            ShowMenuModifyWatcher();
                            break;
                        case 3:
                            ShowMenuOpenDirectory();
                            break;
                        case 4:
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
                    "|                               Open Directory                              |{0}" +
                    "|  -----------------------------------------------------------------------  |{0}" +
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
                            if (Core.OpenDir(AppFile.Config) != StatusReturn.Success)
                            {
                                UI.StopForMessage();
                            }
                            break;
                        case 2:
                            if (Core.OpenDir(AppFile.Log) != StatusReturn.Success)
                            {
                                UI.StopForMessage();
                            }
                            break;
                        case 3:
                            if (Core.OpenDir(AppFile.SavedData) != StatusReturn.Success)
                            {
                                UI.StopForMessage();
                            }
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

        private static void StopForMessage()
        {
            Log.Debug("Stopping for a message that is being displayed");
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }

        private static void ShowMenuModifyWatcher()
        {
            throw new NotImplementedException();
        }

        private static void ShowMenuAddWatcher()
        {
            bool menuClose = false;
            Log.Debug("Presenting Menu AddWatcher");
            while (!menuClose)
            {
                Console.WriteLine(
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "|                                Add Watcher                                |{0}" +
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "{0}Option: ", Environment.NewLine);
                var pageURL = UI.PromptQuestion("Enter the page URL to monitor");
                var keyWord = UI.PromptQuestion("Enter the keyword you want to look for (case sensitive)");
                bool alertOnNotExist = UI.PromptYesNo("Do you want the alert to trigger when this keyword doesn't exist? (if no then alert triggers when the keyword doesn't exist)");
                int alertAnswer = UI.PromptMultipleChoice("Which alert type would you like to use?",
                    new string[]
                    {
                        "Webhook",
                        "Email"
                    });
                Alert selectedAlert = Handler.SelectAlertFromChoice(alertAnswer);
                var newTracker = new TrackedProduct()
                {
                    PageURL = pageURL,
                    Keyword = keyWord,
                    AlertOnKeywordNotExist = alertOnNotExist,
                    AlertType = selectedAlert
                };
                if (selectedAlert == Alert.Webhook)
                {
                    newTracker.WebHookURL = UI.PromptQuestion("Enter the webhook URL");
                    newTracker.MentionString = UI.PromptQuestion("Enter a mention string or message prefix (leave blank if you don't want anything at the beginning of the webhook message");
                }
                else
                {
                    var emailString = UI.PromptQuestion("Enter a comma seperated list of emails to send an alert to");
                    newTracker.Emails = new List<string>();
                    foreach (var email in emailString)
                    {
                        newTracker.Emails.Add(email.ToString().Replace("\"", "").Trim());
                    }
                }
                Constants.SavedData.TrackedProducts.Add(newTracker);
                Log.Information($"Created new tracker! [URL]{newTracker.PageURL} | [KWD]{newTracker.Keyword} | [ANE]{newTracker.AlertOnKeywordNotExist} | [SLA]{newTracker.AlertType}");
                Console.Write($"Successfully created tracker! {Environment.NewLine}URL: {newTracker.PageURL}");
                UI.StopForMessage();
                Console.Clear();
            }
            Log.Information("Exited Menu AddWatcher");
        }

        private static int PromptMultipleChoice(string question, string[] choices)
        {
            Log.Debug($"Asking PromptMultipleChoice({question})");
            Log.Debug($"Choices: {choices}");
            bool answered = false;
            int retAnswer = 0;
            while (!answered)
            {
                Console.WriteLine($"{question}: ");
                var counter = 1;
                foreach (var choice in choices)
                {
                    Console.WriteLine($"   {counter}. {choice}");
                    counter++;
                }
                string answer = Console.ReadLine();
                if (!int.TryParse(answer, out int intAnswer))
                {
                    Log.Debug("Menu answer entered was an invalid response");
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else if (intAnswer <= 0 || intAnswer > choices.Count())
                {
                    Log.Debug("Menu answer entered was an invalid response");
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine($"You entered: {answer}{Environment.NewLine}Is this correct?  [y/n] ");
                    var bAnswer = Console.ReadLine().ToLower();
                    if (bAnswer != "y" && bAnswer != "n")
                    {
                        Log.Debug($"Answer was invalid: {answer}");
                        Console.WriteLine("You entered an invalid response, please try again");
                    }
                    else if (bAnswer != "n")
                    {
                        Log.Debug($"Answer was: {answer}, asking again");
                    }
                    else
                    {
                        retAnswer = intAnswer;
                        answered = true;
                    }
                }
            }
            Log.Information($"PromptMultipleChoice answer was: {retAnswer}");
            return retAnswer;
        }

        private static string PromptQuestion(string question)
        {
            Log.Debug($"Asking PromptQuestion({question})");
            bool answered = false;
            string answer = "";
            while (!answered)
            {
                Console.Write($"{question}: ");
                answer = Console.ReadLine();
                Console.WriteLine($"You entered: {answer}{Environment.NewLine}Is this correct?  [y/n] ");
                var bAnswer = Console.ReadLine().ToLower();
                if (bAnswer != "y" && bAnswer != "n")
                {
                    Log.Debug($"Answer was invalid: {answer}");
                    Console.WriteLine("You entered an invalid response, please try again");
                }
                else if (bAnswer != "n")
                {
                    Log.Debug($"Answer was: {answer}, asking again");
                }
                else
                {
                    answered = true;
                }
            }
            Log.Information($"PromptQuestion answer was: {answer}");
            return answer;
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
