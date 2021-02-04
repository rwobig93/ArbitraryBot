using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ArbitraryBot.BackEnd;
using ArbitraryBot.Shared;
using ArbitraryBot.Dto;
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
                        "Settings",
                        "Close Bot"
                    };
                    int answer = Prompts.PromptMenu(menuName, menuChoices, description, true);
                    if (Constants.DebugMode && answer == 99)
                    {
                        Handler.TestAction();
                    }
                    
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
                            ShowMenuSettings();
                            break;
                        case 7:
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
                    Handler.NotifyError(ex);
                }
            }
            Log.Information("Exited menu root");
        }

        private static void ShowMenuSettings()
        {
            bool menuClose = false;
            while (!menuClose)
            {
                var menuTitle = "Settings Menu";
                var description = "Select a setting to modify or an action:";
                string[] choices = new string[]
                {
                    "Back to the Main Menu",
                    "Show Running Environment & Settings Details",
                    "Check for an Update",
                    "Email Settings",
                    "Enable Beta Updates"
                };
                int answer = Prompts.PromptMenu(menuTitle, choices, description);
                switch (answer)
                {
                    case 1:
                        menuClose = true;
                        break;
                    case 2:
                        ShowEnvironmentVariables();
                        break;
                    case 3:
                        bool updateAvailable = Core.CheckForUpdate();
                        if (updateAvailable)
                            Console.WriteLine("An update is available and is downloading now, we'll alert you when it's ready");
                        else
                            Console.WriteLine("You are on the latest version!");
                        StopForMessage();
                        break;
                    case 4:
                        Prompts.PromptEmailSettings();
                        break;
                    case 5:
                        Constants.Config.BetaUpdates = Prompts.PromptYesNo("Would you like to get Beta versions of this app? (Some updates may be unstable)");
                        break;
                    default:
                        Log.Information("Answer entered wasn't a valid presented option");
                        Console.WriteLine("Answer entered isn't one of the options, please press enter and try again");
                        Console.ReadLine();
                        break;
                }
                Console.Clear();
            }
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
                    webCheck = Communication.WebCheckForKeyword(webpage, keyword).WaitAndUnwrapException();
                    Console.WriteLine("The webpage that was returned was empty/blank");
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
                var answer = Prompts.PromptMenuTrackers(menuName, currentPage, out List<List<TrackedProduct>> trackerList, description);
                if (answer < 0)
                {
                    return;
                }
                
                var trackerPage = trackerList[currentPage - 1];
                if (answer == 1)
                {
                    menuClose = true;
                }
                else if (answer > 0 && answer <= trackerPage.Count + 1)
                {
                    var selectedTracker = trackerPage.ElementAt(answer - 2);
                    Watcher.ProcessAlertToTest(selectedTracker);
                    Console.WriteLine($"Sent test alert for the tracker: {selectedTracker.FriendlyName}");
                    StopForMessage();
                }
                else if (answer > trackerPage.Count + 1 && currentPage >= 2)
                {
                    currentPage--;
                }
                else if (answer > trackerPage.Count + 1 && currentPage < trackerList.Count)
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

        internal static void StopForMessage()
        {
            Log.Debug("Stopping for a message that is being displayed");
            Console.WriteLine($"{Environment.NewLine}Press enter to continue");
            Console.ReadLine();
        }

        private static void ShowMenuModifyWatcher()
        {
            bool menuClose = false;
            int currentPage = 1;
            while (!menuClose)
            {
                string menuName = "Modify Watcher";
                string description = "Select the watcher you wish to modify:";
                var answer = Prompts.PromptMenuTrackers(menuName, currentPage, out List<List<TrackedProduct>> trackerList, description);
                if (answer < 0)
                {
                    return;
                }

                var trackerPage = trackerList[currentPage - 1];
                if (answer == 1)
                {
                    menuClose = true;
                }
                else if (answer > 0 && answer <= trackerPage.Count + 1)
                {
                    var selectedTracker = trackerPage.ElementAt(answer - 2);
                    ShowMenuModifySingleWatcher(selectedTracker);
                }
                else if (answer > trackerPage.Count + 1 && currentPage >= 2)
                {
                    currentPage--;
                }
                else if (answer > trackerPage.Count + 1 && currentPage < trackerList.Count)
                {
                    currentPage++;
                }
                Console.Clear();
            }
            Log.Information("Exited Menu ModifyWatcher");
        }

        private static void ShowMenuModifySingleWatcher(TrackedProduct selectedTracker)
        {
            Console.Clear();
            bool menuClose = false;
            bool removeWatcher = false;
            while (!menuClose)
            {
                string menuName = $"Modify: {selectedTracker.FriendlyName}";
                string description = "Select the property you wish to modify:";
                var answer = Prompts.PromptMenuTrackerProperties(menuName, description);
                
                switch (answer)
                {
                    case 1:
                        menuClose = true;
                        break;
                    case 2:
                        selectedTracker.FriendlyName = Prompts.PromptQuestion("Enter a new Friendly Name");
                        break;
                    case 3:
                        selectedTracker.PageURL = Prompts.PromptQuestion("Enter a new Page URL");
                        break;
                    case 4:
                        selectedTracker.Keyword = Prompts.PromptQuestion("Enter a new keyword");
                        break;
                    case 5:
                        selectedTracker.AlertOnKeywordNotExist = Prompts.PromptYesNo("Alert when keyword doesn't exist?");
                        break;
                    case 6:
                        selectedTracker.Enabled = Prompts.PromptYesNo("Do you want this watcher enabled?");
                        break;
                    case 7:
                        Prompts.PromptWatcherAlertType(selectedTracker);
                        break;
                    case 8:
                        ShowWatcherProperties(selectedTracker);
                        break;
                    case 9:
                        removeWatcher = Prompts.PromptYesNo("Are you sure you want to delete this watcher?");
                        menuClose = true;
                        break;
                    default:
                        Log.Information("Answer entered wasn't a valid presented option");
                        Console.WriteLine("Answer entered isn't one of the options, please press enter and try again");
                        Console.ReadLine();
                        break;
                }
                if (removeWatcher)
                {
                    selectedTracker.Delete();
                }
                else
                {
                    selectedTracker.Save();
                }
            }
            Log.Information("Exited Menu ModifySingleWatcher");
        }

        private static void ShowWatcherProperties(TrackedProduct selectedTracker)
        {
            Console.Clear();
            var menu = Prompts.PromptMenuAction("Watcher Properties", displayMenu: false);
            menu += "FriendlyName".ConvertToMenuProperty(selectedTracker.FriendlyName);
            menu += "PageURL".ConvertToMenuProperty(selectedTracker.PageURL);
            menu += "Keyword".ConvertToMenuProperty(selectedTracker.Keyword);
            menu += "AlertNoKeyword".ConvertToMenuProperty(selectedTracker.AlertOnKeywordNotExist.ToString());
            menu += "Enabled".ConvertToMenuProperty(selectedTracker.Enabled.ToString());
            menu += "AlertInterval".ConvertToMenuProperty(selectedTracker.AlertInterval.ToString());
            menu += "AlertType".ConvertToMenuProperty(selectedTracker.AlertType.ToString());
            if (selectedTracker.AlertType == AlertType.Email)
            {
                menu += "Emails".ConvertToMenuProperty(selectedTracker.Emails.ToString());
            }
            else if (selectedTracker.AlertType == AlertType.Webhook)
            {
                menu += "WebHookURL".ConvertToMenuProperty(selectedTracker.WebHookURL);
                menu += "MentionString".ConvertToMenuProperty(selectedTracker.MentionString);
            }
            menu = menu.AddSeperatorTilde();
            Console.Write(menu);
            StopForMessage();
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
                var newTracker = new TrackedProduct()
                {
                    FriendlyName = friendlyName,
                    PageURL = pageURL,
                    Keyword = keyWord,
                    AlertOnKeywordNotExist = alertOnNotExist
                };
                Prompts.PromptWatcherAlertType(newTracker);
                newTracker.Save();
                Log.Information("Created new tracker! {Tracker}", newTracker);
                Console.Write($"Successfully created tracker! {Environment.NewLine}URL: {newTracker.PageURL}");
                menuClose = true;
                StopForMessage();
                Console.Clear();
            }
            Log.Information("Exited Menu AddWatcher");
        }

        internal static void DisplayLatestNotification()
        {
            Log.Verbose("Updating Notification");
            var (Left, Top) = Console.GetCursorPosition();
            Console.SetCursorPosition(0, 0);
            Console.Write(Constants.Notifications.Last().ConvertToNotification());
            Log.Debug("Updating Notification: {Notification}", Constants.Notifications[0]);
            Console.SetCursorPosition(Left, Top);
            Log.Verbose("Set cursor position back");
        }

        internal static void AddNewNotification(string notification)
        {
            Log.Verbose("Adding new notification: {Notification}", notification);
            Constants.Notifications.Add(notification);
            if (Constants.Notifications.Count >= 5)
            {
                Log.Verbose("Removed oldest notification");
                Constants.Notifications.RemoveAt(0);
            }
            DisplayLatestNotification();
        }

        internal static void ShowEnvironmentVariables()
        {
            Log.Debug("Displaying Environment & Setting Details");
            string info = "".AddSeperatorTilde();
            int envLength = 25;
            int setLength = 18;
            info += "App Version".ConvertToMenuProperty(OSDynamic.GetRunningVersion().ToString(), envLength);
            info += "Hostname".ConvertToMenuProperty(Environment.MachineName, envLength);
            info += "Current OS Platform".ConvertToMenuProperty(OSDynamic.GetCurrentOS().ToString(), envLength);
            info += "Current OS Architecture".ConvertToMenuProperty(RuntimeInformation.OSArchitecture.ToString(), envLength);
            info += "Current OS Description".ConvertToMenuProperty(RuntimeInformation.OSDescription, envLength);
            info += "Current Process Arch".ConvertToMenuProperty(RuntimeInformation.ProcessArchitecture.ToString(), envLength);
            info += "Current Framework".ConvertToMenuProperty(RuntimeInformation.FrameworkDescription, envLength);
            info += "Logging Path".ConvertToMenuProperty(Constants.PathLogs, envLength);
            info += "Config Path".ConvertToMenuProperty(Constants.PathConfigDefault, envLength);
            info = info.AddSeperatorDashed();
            info += "Email Name".ConvertToMenuProperty(Constants.Config.SMTPEmailName, setLength);
            info += "Email From Address".ConvertToMenuProperty(Constants.Config.SMTPEmailFrom, setLength);
            info += "Email URL".ConvertToMenuProperty(Constants.Config.SMTPUrl, setLength);
            info += "Email Port".ConvertToMenuProperty(Constants.Config.SMTPPort.ToString(), setLength);
            info = info.AddSeperatorTilde();
            Console.WriteLine(info);
            Log.Information("Environment & Setting Details Displayed");
            StopForMessage();
        }
    }
}
