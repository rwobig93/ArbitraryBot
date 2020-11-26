using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArbitraryBot.Shared;
using Serilog;

namespace ArbitraryBot.BackEnd
{
    public static class Watcher
    {
        internal static void CheckOnTrackers(TrackInterval interval)
        {
            switch (interval)
            {
                case TrackInterval.OneMin:
                    foreach (TrackedProduct tracker in Constants.SavedData.TrackedProducts1Min)
                    {
                        ProcessAlertNeedOnTracker(tracker);
                    }
                    break;
                case TrackInterval.FiveMin:
                    break;
            }
        }

        private static async void ProcessAlertNeedOnTracker(TrackedProduct tracker)
        {
            try
            {
                Log.Verbose("Processing alert for tracker", tracker);
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(tracker.PageURL);
                var contents = await response.Content.ReadAsStringAsync();
                bool keywordFound = contents.Contains(tracker.Keyword);
                if ((keywordFound && !tracker.AlertOnKeywordNotExist) || (!keywordFound && tracker.AlertOnKeywordNotExist))
                {
                    Log.Debug("Alerting on tracker as logic matches", tracker, keywordFound);
                    ProcessAlertToSend(tracker);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured when attempting to process alerting for tracker");
            }
        }

        private static void ProcessAlertToSend(TrackedProduct tracker)
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

        internal static void ProcessAlertToTest(TrackedProduct tracker)
        {
            Log.Debug("Processing Alert Type For Testing", tracker.AlertType);
            string title = "Testing alert on the following tracker, Get Pumped!";
            string msg = $"Testing tracker for the following page: {Environment.NewLine}{tracker.PageURL}";
            string color = "faf202";
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