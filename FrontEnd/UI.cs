﻿using System;
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
                try
                {
                    Console.Write(
                                "{0}" +
                                "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                                "|                                 Main Menu                                 |{0}" +
                                "|  -----------------------------------------------------------------------  |{0}" +
                                "|  Enter the corresponding menu number for the action you want to perform:  |{0}" +
                                "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                                "|  1. Add Watcher                                                           |{0}" +
                                "|  2. Modify Watcher                                                        |{0}" +
                                "|  3. Test Watcher Alert                                                    |{0}" +
                                "|  4. Open Directory                                                        |{0}" +
                                "|  5. Close Bot                                                             |{0}" +
                                "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                                "{0}Option: ", Environment.NewLine);
                    var answer = Console.ReadLine();
                    Log.Debug("Menu prompt answered: {Answer}", answer);
                    if (!int.TryParse(answer, out int intAnswer))
                    {
                        Log.Debug("Menu answer entered was an invalid response");
                        Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                        Console.ReadLine();
                    }
                    else
                    {
                        Log.Debug("Valid menu option was entered: {Answer}", intAnswer);
                        switch (intAnswer)
                        {
                            case 1:
                                ShowMenuAddWatcher();
                                break;
                            case 2:
                                ShowMenuModifyWatcher();
                                break;
                            case 3:
                                ShowMenuTestWatcherAlert();
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
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error: {ex.Message}");
                }
            }
            Log.Information("Exited menu root");
        }

        private static void ShowMenuTestWatcherAlert()
        {
            Console.Clear();
            bool menuClose = false;
            int currentPage = 1;
            Log.Debug("Presenting Menu TestWatcherAlert");
            while (!menuClose)
            {
                string menu = string.Format(
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "|                             Test Watcher Alert                            |{0}" +
                    "|  -----------------------------------------------------------------------  |{0}" +
                    "|                 Select the watcher alert you want to test:                |{0}" +
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "|  1. Back to Main Menu                                                     |{0}", Environment.NewLine);
                List<List<TrackedProduct>> trackerList = new List<List<TrackedProduct>>();
                menu = Handler.GetTrackersForMenu(menu, currentPage, out trackerList);
                if (trackerList[0].Count <= 0)
                {
                    Console.WriteLine($"There currently aren't any trackers created!{Environment.NewLine}" +
                        $"Please create one before attempting to test");
                    StopForMessage();
                    menuClose = true;
                    return;
                }
                menu += string.Format(
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "{0}Current Page: {1} " +
                    "{0}Option: ", Environment.NewLine, currentPage);
                Console.Write(menu);
                var answer = Console.ReadLine();
                Log.Debug("Menu prompt answered: {Answer}", answer);
                if (!int.TryParse(answer, out int intAnswer))
                {
                    Log.Debug("Menu answer entered was an invalid response");
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else
                {
                    Log.Debug("Valid menu option was entered {Answer}", intAnswer);
                    var trackerPage = trackerList[currentPage - 1];
                    if (intAnswer == 1)
                    {
                        menuClose = true;
                    }
                    else if (intAnswer > 0 && intAnswer <= trackerPage.Count() + 1)
                    {
                        var selectedTracker = trackerPage.ElementAt(intAnswer - 2);
                        Watcher.ProcessAlertToTest(selectedTracker);
                        Console.WriteLine($"Sent test alert for the tracker: {selectedTracker.FriendlyName}");
                        StopForMessage();
                    }
                    else if (intAnswer > trackerPage.Count() + 1 && currentPage >= 2)
                    {
                        currentPage--;
                    }
                    else if (intAnswer > trackerPage.Count() + 1 && currentPage < trackerList.Count)
                    {
                        currentPage++;
                    }
                }
                Console.Clear();
            }
            Log.Information("Exited Menu TestWatcherAlert");
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
                Log.Debug("Menu prompt answered: {Answer}", answer);
                if (!int.TryParse(answer, out int intAnswer))
                {
                    Log.Debug("Menu answer entered was an invalid response");
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else
                {
                    Log.Debug("Valid menu option was entered: {Answer}", intAnswer);
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
            Console.WriteLine($"{Environment.NewLine}Press enter to continue");
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
                    "|                                Add Tracker                                |{0}" +
                    "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{0}" +
                    "{0}Option: ", Environment.NewLine);
                var friendlyName = UI.PromptQuestion("Enter a name for this tracker");
                var pageURL = UI.PromptQuestion("Enter the page URL to monitor");
                var keyWord = UI.PromptQuestion("Enter the keyword you want to look for (case sensitive)");
                bool alertOnNotExist = UI.PromptYesNo($"Do you want the alert to trigger when this keyword doesn't exist?{Environment.NewLine} (if no then alert triggers when the keyword does exist)");
                int intervalAnswer = UI.PromptMultipleChoice("Which interval would you like this tracker to check?",
                    new string[] 
                    {
                        "1 Min",
                        "5 Min"
                    });
                int alertAnswer = UI.PromptMultipleChoice("Which alert type would you like to use?",
                    new string[]
                    {
                        "Webhook",
                        "Email"
                    });
                Alert selectedAlert = Handler.SelectAlertFromChoice(alertAnswer);
                var newTracker = new TrackedProduct()
                {
                    FriendlyName = friendlyName,
                    PageURL = pageURL,
                    Keyword = keyWord,
                    AlertOnKeywordNotExist = alertOnNotExist,
                    AlertType = selectedAlert
                };
                if (selectedAlert == Alert.Webhook)
                {
                    newTracker.WebHookURL = UI.PromptQuestion("Enter the webhook URL");
                    newTracker.MentionString = UI.PromptQuestion($"Enter an ID of a user or role you want to mention{Environment.NewLine} (leave blank if you don't want a mention with the alert");
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
                Handler.SelectTrackerIntervalFromChoice(intervalAnswer, newTracker);
                Log.Information("Created new tracker! {Tracker}", newTracker);
                Console.Write($"Successfully created tracker! {Environment.NewLine}URL: {newTracker.PageURL}");
                menuClose = true;
                UI.StopForMessage();
                Console.Clear();
            }
            Log.Information("Exited Menu AddWatcher");
        }

        private static int PromptMultipleChoice(string question, string[] choices, bool validate = false)
        {
            Log.Debug("Asking PromptMultipleChoice [q]{Question} [c]{Choices}", question, choices);
            bool answered = false;
            int retAnswer = 0;
            while (!answered)
            {
                var counter = 1;
                Console.WriteLine($"{question}: ");
                foreach (var choice in choices)
                {
                    Console.WriteLine($"   {counter}. {choice}");
                    counter++;
                }
                Console.Write($"{Environment.NewLine}Option: ");
                string answer = Console.ReadLine();
                if (!int.TryParse(answer, out int intAnswer))
                {
                    Log.Debug("Menu answer entered was an invalid response: {Answer}", answer);
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else if (intAnswer <= 0 || intAnswer > choices.Count())
                {
                    Log.Debug("Menu answer entered was an invalid response: {Answer}", intAnswer);
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else
                {
                    if (validate)
                    {
                        Console.Write($"You chose: {choices[intAnswer - 1]}{Environment.NewLine}Is this correct?  [y/n] ");
                        var bAnswer = Console.ReadLine().ToLower();
                        if (bAnswer != "y" && bAnswer != "n")
                        {
                            Log.Debug("Answer was invalid: {Answer}", answer);
                            Console.WriteLine("You entered an invalid response, please try again");
                        }
                        else if (bAnswer == "n")
                        {
                            Log.Debug("Answer was no, asking again: {Answer}", bAnswer);
                        }
                        else
                        {
                            retAnswer = intAnswer;
                            answered = true;
                        }
                    }
                    else
                    {
                        retAnswer = intAnswer;
                        answered = true;
                    }
                }
            }
            Log.Information("PromptMultipleChoice answered: {Answer}", retAnswer);
            return retAnswer;
        }

        private static string PromptQuestion(string question, bool validate = false)
        {
            Log.Debug("Asking PromptQuestion: {Question}", question);
            bool answered = false;
            string answer = "";
            while (!answered)
            {
                Console.Write($"{question}: ");
                answer = Console.ReadLine();
                if (validate)
                {
                    Console.Write($"You entered: {answer}{Environment.NewLine}Is this correct?  [y/n] ");
                    var bAnswer = Console.ReadLine().ToLower();
                    if (bAnswer != "y" && bAnswer != "n")
                    {
                        Log.Debug("Answer was invalid: {Answer}", answer);
                        Console.WriteLine("You entered an invalid response, please try again");
                    }
                    else if (bAnswer == "n")
                    {
                        Log.Debug("Answer was no, asking again: {Answer}", bAnswer);
                    }
                    else
                    {
                        answered = true;
                    }
                }
                else
                {
                    answered = true;
                }
            }
            Log.Information("PromptQuestion answered", answer);
            return answer;
        }

        internal static void DisplayHostInfo()
        {
            Log.Debug("Displaying host info");
            Console.WriteLine(
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
            Log.Debug("Asking PromptYesNo: {Question}", question);
            bool answered = false;
            string answer = "";
            while (!answered)
            {
                Console.Write($"{question} [y/n]? ");
                answer = Console.ReadLine().ToLower();
                if (answer != "y" && answer != "n")
                {
                    Log.Debug("Answer was invalid: {Answer}", answer);
                    Console.WriteLine("You entered an invalid response, please try again");
                }
                else
                {
                    answered = true;
                }
            }
            Log.Information("Prompt YesNo answered: {Answer}", answer);
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
