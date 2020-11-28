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
using Nito.AsyncEx.Synchronous;
using ArbitraryBot.Extensions;

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
                    string menuName = "Main Menu";
                    string description = "Enter the corresponding menu number for the action you want to perform:";
                    string[] menuChoices = new string[]
                    {
                        "Add Watcher",
                        "Modify Watcher",
                        "Test Watcher Alert",
                        "Test Keyword On Website",
                        "Open Directory",
                        "Close Bot"
                    };
                    int answer = Prompts.PromptMenu(menuName, menuChoices, description);
                    
                    switch (answer)
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
                            ShowMenuTestKeywordOnWebpage();
                            break;
                        case 5:
                            ShowMenuOpenDirectory();
                            break;
                        case 6:
                            Handler.CloseApp();
                            break;
                        default:
                            Log.Information("Answer entered wasn't a valid presented option");
                            Console.WriteLine("Answer entered isn't one of the options, please press enter and try again");
                            Console.ReadLine();
                            break;
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

        private static void ShowMenuTestKeywordOnWebpage()
        {
            bool menuClose = false;
            while (!menuClose)
            {
                Prompts.PromptMenuAction("Test Keyword On Webpage");
                var webpage = Prompts.PromptQuestion("Enter the webpage URL");
                var keyword = Prompts.PromptQuestion("Enter the keyword");
                WebCheck webCheck = null;

                try
                {
                    webCheck = Communication.DoesKeywordExistOnWebpage(webpage, keyword).WaitAndUnwrapException();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error from webpage: {ex.Message}");
                }

                if (webCheck != null)
                {
                    if (webCheck.KeywordExists)
                    {
                        Console.WriteLine("The keyword was found!");
                    }
                    else
                    {
                        Console.WriteLine($"Webpage Response StatusCode: {webCheck.ResponseCode}");
                        Console.WriteLine("The keyword wasn't found on the webpage :(");
                    }
                }
                var tryAgain = Prompts.PromptYesNo("Would you like to do another test?");
                if (!tryAgain)
                {
                    menuClose = true;
                }
                Console.Clear();
            }
            Log.Information("Exited Menu TestKeywordOnWebpage");
        }

        private static void ShowMenuTestWatcherAlert()
        {
            bool menuClose = false;
            int currentPage = 1;
            while (!menuClose)
            {
                string menuName = "Test Watcher Alert";
                string description = "Select the watcher alert you want to test:";
                List<List<TrackedProduct>> trackerList = new List<List<TrackedProduct>>();
                var answer = Prompts.PromptMenuTrackers(menuName, currentPage, out trackerList, description);
                if (answer < 0)
                {
                    return;
                }
                
                var trackerPage = trackerList[currentPage - 1];
                if (answer == 1)
                {
                    menuClose = true;
                }
                else if (answer > 0 && answer <= trackerPage.Count() + 1)
                {
                    var selectedTracker = trackerPage.ElementAt(answer - 2);
                    Watcher.ProcessAlertToTest(selectedTracker);
                    Console.WriteLine($"Sent test alert for the tracker: {selectedTracker.FriendlyName}");
                    StopForMessage();
                }
                else if (answer > trackerPage.Count() + 1 && currentPage >= 2)
                {
                    currentPage--;
                }
                else if (answer > trackerPage.Count() + 1 && currentPage < trackerList.Count)
                {
                    currentPage++;
                }
                Console.Clear();
            }
            Log.Information("Exited Menu TestWatcherAlert");
        }

        private static void ShowMenuOpenDirectory()
        {
            bool menuClose = false;
            while (!menuClose)
            {
                string menuName = "Open Directory";
                string description = "Which directory would you like to open?";
                string[] menuChoices = new string[]
                {
                    "Open Config Directory",
                    "Open Log Directory",
                    "Open SaveData Directory",
                    "Back to Main Menu"
                };
                int answer = Prompts.PromptMenu(menuName, menuChoices, description);
                switch (answer)
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
            while (!menuClose)
            {
                Prompts.PromptMenuAction("Add a Watcher");
                var friendlyName = Prompts.PromptQuestion("Enter a name for this tracker");
                var pageURL = Prompts.PromptQuestion("Enter the page URL to monitor");
                var keyWord = Prompts.PromptQuestion("Enter the keyword you want to look for (case sensitive)");
                bool alertOnNotExist = Prompts.PromptYesNo($"Do you want the alert to trigger when this keyword doesn't exist?{Environment.NewLine} (if no then alert triggers when the keyword does exist)");
                int intervalAnswer = Prompts.PromptMultipleChoice("Which interval would you like this tracker to check?",
                    new string[]
                    {
                        "1 Min",
                        "5 Min"
                    });
                int alertAnswer = Prompts.PromptMultipleChoice("Which alert type would you like to use?",
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
                    newTracker.WebHookURL = Prompts.PromptQuestion("Enter the webhook URL");
                    newTracker.MentionString = Prompts.PromptQuestion($"Enter an ID of a user or role you want to mention{Environment.NewLine} (leave blank if you don't want a mention with the alert");
                }
                else
                {
                    var emailString = Prompts.PromptQuestion("Enter a comma seperated list of emails to send an alert to");
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
    }
}
