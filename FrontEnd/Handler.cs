using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArbitraryBot.BackEnd;
using ArbitraryBot.Extensions;
using ArbitraryBot.Shared;
using Serilog;

namespace ArbitraryBot.FrontEnd
{
    public static class Handler
    {
        internal static void CatchAppClose()
        {
            Core.SaveEverything();
            Jobs.StopJobService();
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
                    Log.Debug("Launch arg was -debug, changed local logging to debug", arg);
                }
                if (arg.ToLower() == "-debugconsole")
                {
                    Core.ChangeLoggingLevelConsole(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was -debugconsole, changed console logging to debug", arg);
                }
                if (arg.ToLower() == "-debugcloud")
                {
                    Core.ChangeLoggingLevelCloud(Serilog.Events.LogEventLevel.Debug);
                    Log.Debug("Launch arg was -debugcloud, changed cloud logging to debug", arg);
                }
            }
        }

        internal static void CloseApp()
        {
            var answer = true;
            #if !DEBUG
            answer = UI.PromptYesNo("Are you sure you want to close the bot?");
            #endif
            Log.Debug("Closing app called w/ an answer", answer, Constants.CloseApp);
            if (answer)
            {
                Constants.CloseApp = true;
            }
            Log.Information("Closing app", answer, Constants.CloseApp);
            Core.SaveEverything();
        }

        internal static Alert SelectAlertFromChoice(int alertAnswer)
        {
            switch (alertAnswer)
            {
                case 1:
                    Log.Debug("Set alertType to Webhook", alertAnswer);
                    return Alert.Webhook;
                case 2:
                    Log.Debug("Set alertType to Email", alertAnswer);
                    return Alert.Email;
                default:
                    Log.Warning("Default was hit on switch that shouldn't occur", alertAnswer);
                    return Alert.Email;
            }
        }

        internal static void SelectTrackerIntervalFromChoice(int intervalAnswer, TrackedProduct tracker)
        {
            switch (intervalAnswer)
            {
                case 1:
                    Log.Debug("Adding tracker to 1min queue", tracker, intervalAnswer);
                    tracker.AlertInterval = TrackInterval.OneMin;
                    Constants.SavedData.TrackedProducts1Min.Add(tracker);
                    Log.Information("Added tracker to 1min queue", tracker, intervalAnswer);
                    break;
                case 2:
                    Log.Debug("Adding tracker to 5min queue", tracker, intervalAnswer);
                    tracker.AlertInterval = TrackInterval.FiveMin;
                    Constants.SavedData.TrackedProducts5Min.Add(tracker);
                    Log.Information("Added tracker to 5min queue", tracker, intervalAnswer);
                    break;
                default:
                    Log.Warning("Default was hit on a switch that shouldn't occur", tracker, intervalAnswer);
                    Constants.SavedData.TrackedProducts5Min.Add(tracker);
                    break;
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
                    menu += tracker.FriendlyName.ConvertToMenuOption(menuNum);
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
    }
}
