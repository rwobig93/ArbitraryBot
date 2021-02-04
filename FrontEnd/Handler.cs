using System;
using System.Collections.Generic;
using ArbitraryBot.BackEnd;
using ArbitraryBot.Extensions;
using ArbitraryBot.Shared;
using ArbitraryBot.Dto;
using Serilog;

namespace ArbitraryBot.FrontEnd
{
    public static class Handler
    {
        internal static void CatchAppClose()
        {
            Core.SaveEverything();
            Jobs.StopJobService();
            Communication.Dispose();
            Log.Information("==START-STOP== Application Stopped");
            Log.CloseAndFlush();
        }

        internal static void ParseLaunchArgs(string[] args)
        {
            foreach (var arg in args)
            {
                Log.Information($"Launch arg passed: {arg}", args);
                
                if (arg.ToLower() == "-debug")
                {
                    Core.ChangeLoggingLevelLocal(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was {Arg}, changed local logging to debug", arg);
                }
                if (arg.ToLower() == "-debugconsole")
                {
                    Core.ChangeLoggingLevelConsole(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was {Arg}, changed console logging to debug", arg);
                }
                if (arg.ToLower() == "-debugcloud")
                {
                    Core.ChangeLoggingLevelCloud(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was {Arg}, changed cloud logging to debug", arg);
                }
                if (arg.ToLower() == "-verbose")
                {
                    Core.ChangeLoggingLevelLocal(Serilog.Events.LogEventLevel.Verbose);
                    Core.ChangeLoggingLevelCloud(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was {Arg}, changed cloud & local logging to verbose", arg);
                }
            }
        }

        internal static void TestAction()
        {
            if (!Constants.DebugMode)
            {
                UI.AddNewNotification("You shouldn't be here... how'd you do that?");
                UI.StopForMessage();
                return;
            }
            for (int i = 0; i < 5; i++)
            {
                UI.AddNewNotification($"Test notification: {i} | Notify Count: {Constants.Notifications.Count}");
                UI.StopForMessage();
            }
        }

        internal static void CloseApp()
        {
            var answer = true;
            #if !DEBUG
            answer = Prompts.PromptYesNo("Are you sure you want to close the bot?");
            #endif
            Log.Debug("Closing app called w/ an answer", answer, Constants.CloseApp);
            if (answer)
            {
                Constants.CloseApp = true;
            }
            Log.Information("Closing app", answer, Constants.CloseApp);
            Core.SaveEverything();
        }

        internal static AlertType SelectAlertFromChoice(int alertAnswer)
        {
            switch (alertAnswer)
            {
                case 1:
                    Log.Debug("Set alertType to Webhook", alertAnswer);
                    return AlertType.Webhook;
                case 2:
                    Log.Debug("Set alertType to Email", alertAnswer);
                    return AlertType.Email;
                default:
                    Log.Warning("Default was hit on switch that shouldn't occur", alertAnswer);
                    return AlertType.Email;
            }
        }

        /// <summary>
        /// Return a string menu of enumerated tracker lists along w/ the combined paged list
        /// </summary>
        /// <param name="menu">The menu string we're adding to</param>
        /// <param name="currentPage">The current page we're on</param>
        /// <param name="splitList">The returned combined paged list of enumerated trackers</param>
        /// <returns></returns>
        internal static string GetTrackersForMenu(string menu, int currentPage, out List<List<TrackedProduct>> splitList)
        {
            splitList = new List<List<TrackedProduct>>();
            int currentList = 0;

            // Add all tracker lists to one list for enumeration
            List<List<TrackedProduct>> trackedLists = new List<List<TrackedProduct>>
            {
                Constants.SavedData.TrackedProducts1Min,
                Constants.SavedData.TrackedProducts5Min
            };

            // Enumerate tracked product lists and combine into a single list of paged trackers
            splitList.Add(new List<TrackedProduct>());
            foreach (var trackerList in trackedLists)
            {
                foreach (var tracker in trackerList)
                {
                    if (splitList[currentList].Count < 7)
                    {
                        splitList[currentList].Add(tracker);
                    }
                    else
                    {
                        currentList++;
                        splitList.Add(new List<TrackedProduct>());
                        splitList[currentList].Add(tracker);
                    }
                }
            }

            // Build menu from the single tracked product list
            var menuNum = 2;
            if (splitList.Count > 0)
            {
                foreach (TrackedProduct tracker in splitList[currentPage - 1])
                {
                    string enabled = tracker.Enabled ? "E" : "D";
                    string interval = tracker.AlertInterval == TrackInterval.OneMin ? "1Min" : "5Min";
                    menu += $"[{interval}] [{enabled}] {tracker.FriendlyName}".ConvertToMenuOption(menuNum);
                    menuNum++;
                }
                if (currentPage >= 2)
                {
                    menu += "Go to previous page".ConvertToMenuOption(menuNum);
                    menuNum++;
                }
                if (splitList.Count > 0 && currentPage < splitList.Count)
                {
                    menu += "Go to next page".ConvertToMenuOption(menuNum);
                }
            }

            return menu;
        }

        internal static TrackInterval SelectAlertIntervalFromChoice(int intervalAnswer)
        {
            return intervalAnswer switch
            {
                1 => TrackInterval.OneMin,
                2 => TrackInterval.FiveMin,
                _ => TrackInterval.FiveMin,
            };
        }

        internal static void NotifyError(string error)
        {
            Log.Information($"Nofiying Error: [Error] {error}");
            UI.AddNewNotification($"[Error] {error}");
        }

        internal static void NotifyError(Exception ex, string error = "")
        {
            string message;
            if (string.IsNullOrWhiteSpace(error))
                message = $"[Error]: {ex.Message}";
            else
                message = $"[{error}] {ex.Message}";
            Log.Information("Nofiying Error: {Notification}", message);
            UI.AddNewNotification(message);
        }
    }
}
