using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArbitraryBot.Shared;
using Nito.AsyncEx.Synchronous;
using Serilog;

namespace ArbitraryBot.BackEnd
{
    public static class Watcher
    {
        public static void CheckOnTrackers(TrackInterval interval)
        {
            Log.Information("Running Tracker Check: {Interval}", interval);
            switch (interval)
            {
                case TrackInterval.OneMin:
                    foreach (TrackedProduct tracker in Constants.SavedData.TrackedProducts1Min)
                    {
                        var task = ProcessAlertNeedOnTracker(tracker);
                        var result = task.WaitAndUnwrapException();
                        Log.Verbose("Tracker Response: {TrackerResponse}", result);
                    }
                    break;
                case TrackInterval.FiveMin:
                    foreach (TrackedProduct tracker in Constants.SavedData.TrackedProducts5Min)
                    {
                        var task = ProcessAlertNeedOnTracker(tracker);
                        var result = task.WaitAndUnwrapException();
                        Log.Verbose("Tracker Response: {TrackerResponse}", result);
                    }
                    break;
            }
        }

        public static async Task<string> ProcessAlertNeedOnTracker(TrackedProduct tracker)
        {
            try
            {
                Log.Verbose("Processing alert for tracker", tracker);
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(tracker.PageURL);
                    var contents = await response.Content.ReadAsStringAsync();
                    bool keywordFound = contents.Contains(tracker.Keyword);
                    if ((keywordFound && !tracker.AlertOnKeywordNotExist) || (!keywordFound && tracker.AlertOnKeywordNotExist))
                    {
                        Log.Debug("Alerting on tracker as logic matches", tracker, keywordFound);
                        ProcessAlertToSend(tracker);
                    }
                    return contents;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured when attempting to process alerting for tracker");
                return "";
            }
        }

        public static void ProcessAlertToSend(TrackedProduct tracker)
        {
            Log.Debug("Processing Alert Type", tracker.AlertType);
            switch (tracker.AlertType)
            {
                case Alert.Email:
                    Communication.SendAlertEmail(tracker);
                    break;
                case Alert.Webhook:
                    Communication.SendAlertWebhookDiscord(tracker);
                    break;
                case Alert.Email_Webhook:
                    Log.Warning("Processed Alert Type Webhook + Email when it isn't implemented yet", tracker);
                    break;
            }
        }

        public static void ProcessAlertToTest(TrackedProduct tracker)
        {
            Log.Debug("Processing Alert Type For Testing", tracker.AlertType);
            string title = "Testing alert on the following tracker, Get Pumped!";
            string msg = $"Testing tracker for the following page: {Environment.NewLine}{tracker.PageURL}";
            string color = "16445954";
            switch (tracker.AlertType)
            {
                case Alert.Email:
                    Communication.SendAlertEmail(tracker);
                    break;
                case Alert.Webhook:
                    Communication.SendAlertWebhookDiscord(tracker, title, msg, color);
                    break;
                case Alert.Email_Webhook:
                    Log.Warning("Processed Alert Type Webhook + Email when it isn't implemented yet", tracker);
                    break;
            }
        }
    }
}