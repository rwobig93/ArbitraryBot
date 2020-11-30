using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbitraryBot.Extensions;
using ArbitraryBot.Shared;
using Serilog;

namespace ArbitraryBot.FrontEnd
{
    public static class Prompts
    {
        internal static int PromptMenuTrackers(string menuTitle, int currentPage, out List<List<TrackedProduct>> trackerList, string description = "")
        {
            Console.Clear();
            Log.Debug("Displaying MenuTrackers: {MenuTitle}", menuTitle);
            var menuClose = false;
            int intAnswer = 0;
            trackerList = new List<List<TrackedProduct>>();
            while (!menuClose)
            {
                var menu = Prompts.PromptMenuAction(menuTitle, description, false);
                menu += "Back to Main Menu".ConvertToMenuOption(1);

                menu = Handler.GetTrackersForMenu(menu, currentPage, out trackerList);
                if (trackerList[0].Count <= 0)
                {
                    Console.WriteLine($"There currently aren't any trackers created!{Environment.NewLine}" +
                        $"Please create one before attempting to test, press enter to continue");
                    Console.ReadLine();
                    return -1;
                }
                menu = menu.AddSeperatorTilde();
                menu += $"{Environment.NewLine}Current Page: {currentPage}{Environment.NewLine}Option: ";
                Console.Write(menu);
                var answer = Console.ReadLine();
                Log.Debug("Menu prompt answered: {Answer}", answer);
                if (!int.TryParse(answer, out intAnswer))
                {
                    Log.Debug("Menu answer entered was an invalid response");
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else
                {
                    menuClose = true;
                }
            }
            Log.Debug("Valid menu option was entered: {Answer}", intAnswer);
            return intAnswer;
        }
        internal static string PromptMenuAction(string menuTitle, string description = "", bool displayMenu = true)
        {
            Console.Clear();
            Log.Debug("Working on MenuAction: {MenuTitle}", menuTitle);
            string menu = "";
            menu = menu.AddSeperatorTilde();
            menu += menuTitle.ConvertToMenuTitle();
            if (!string.IsNullOrWhiteSpace(description))
            {
                menu = menu.AddSeperatorDashed();
                menu += description.ConvertToMenuTitle();
            }
            menu = menu.AddSeperatorTilde();
            if (displayMenu)
            {
                Console.Write(menu);
                Log.Debug("Displayed MenuAction: {MenuTitle}", menuTitle);
            }
            else
            {
                Log.Debug("Not Displaying MenuAction: {MenuTitle}", menuTitle);
            }
            return menu;
        }

        internal static int PromptMenu(string menuTitle, string[] choices, string description = "")
        {
            Console.Clear();
            Log.Debug("Displaying Menu: {MenuTitle}", menuTitle);
            var validAnswer = false;
            int intAnswer = 0;
            while (!validAnswer)
            {
                string menu = "";
                menu = menu.AddSeperatorTilde();
                menu += menuTitle.ConvertToMenuTitle();
                if (!string.IsNullOrWhiteSpace(description))
                {
                    menu = menu.AddSeperatorDashed();
                    menu += description.ConvertToMenuTitle();
                }
                menu = menu.AddSeperatorTilde();
                int choiceNum = 1;
                foreach (var choice in choices)
                {
                    menu += choice.ConvertToMenuOption(choiceNum);
                    choiceNum++;
                }
                menu = menu.AddSeperatorTilde();
                menu += $"{Environment.NewLine}Option: ";
                Console.Write(menu);
                var answer = Console.ReadLine();
                Log.Debug("Menu prompt answered: {Answer}", answer);
                if (!int.TryParse(answer, out intAnswer))
                {
                    Log.Debug("Menu answer entered was an invalid response");
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else
                {
                    validAnswer = true;
                }
            }
            Log.Debug("Valid menu option was entered: {Answer}", intAnswer);
            return intAnswer;
        }

        internal static int PromptMultipleChoice(string question, string[] choices, bool validate = false)
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
                else if (intAnswer <= 0 || intAnswer > choices.Length)
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

        internal static string PromptQuestion(string question, bool validate = false)
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

        internal static int PromptMenuTrackerProperties(string menuName, string description)
        {
            Console.Clear();
            Log.Debug("Displaying MenuTrackerProps: {MenuTitle}", menuName);
            var validAnswer = false;
            int intAnswer = 0;
            while (!validAnswer)
            {
                var menu = PromptMenuAction(menuName, description, false);
                string[] choices = new string[]
                {
                    "[<-] Back to Previous Menu",
                    "[P]  Friendly Name",
                    "[P]  Page URL",
                    "[P]  Keyword",
                    "[P]  Alert when keyword doesn't exist",
                    "[P]  Enabled",
                    "[P]  Alert Settings",
                    "[D]  Display Current Property Values",
                    "[X]  Delete Watcher"
                };

                int choiceNum = 1;
                foreach (var choice in choices)
                {
                    menu += choice.ConvertToMenuOption(choiceNum);
                    choiceNum++;
                }
                menu = menu.AddSeperatorTilde();
                menu += $"{Environment.NewLine}Option: ";
                Console.Write(menu);
                var answer = Console.ReadLine();
                Log.Debug("Menu prompt answered: {Answer}", answer);
                if (!int.TryParse(answer, out intAnswer))
                {
                    Log.Debug("Menu answer entered was an invalid response");
                    Console.WriteLine("Answer wasn't invalid, please press enter and try again");
                    Console.ReadLine();
                }
                else
                {
                    validAnswer = true;
                }
            }
            Log.Debug("Valid menu option was entered: {Answer}", intAnswer);
            return intAnswer;
        }

        internal static void PromptWatcherAlertType(TrackedProduct tracker)
        {
            int intervalAnswer = Prompts.PromptMultipleChoice("Which interval would you like this tracker to check?",
                new string[]
                {
                        "1 Min",
                        "5 Min"
                });
            tracker.AlertInterval = Handler.SelectAlertIntervalFromChoice(intervalAnswer);
            int alertAnswer = Prompts.PromptMultipleChoice("Which alert type would you like to use?",
                new string[]
                {
                        "Webhook",
                        "Email"
                });
            tracker.AlertType = Handler.SelectAlertFromChoice(alertAnswer);
            if (tracker.AlertType == Alert.Webhook)
            {
                tracker.WebHookURL = Prompts.PromptQuestion("Enter the webhook URL");
                tracker.MentionString = Prompts.PromptQuestion($"Enter an ID of a user or role you want to mention{Environment.NewLine} (leave blank if you don't want a mention with the alert");
            }
            else
            {
                var emailString = Prompts.PromptQuestion("Enter a comma seperated list of emails to send an alert to");
                tracker.Emails = new List<string>();
                foreach (var email in emailString)
                {
                    tracker.Emails.Add(email.ToString().Replace("\"", "").Trim());
                }
            }
        }
    }
}
