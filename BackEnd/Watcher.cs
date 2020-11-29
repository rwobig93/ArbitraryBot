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
        public static void CheckOnTrackers(TrackInterval interval)
        {
            Log.Information("Running Tracker Check: {Interval}", interval);
            switch (interval)
            {
                case TrackInterval.OneMin:
                    foreach (TrackedProduct tracker in Constants.SavedData.TrackedProducts1Min)
                    {
                        if (tracker.Enabled)
                        {
                            Log.Information("Attempting to Run 1min Process");
                            ProcessAlertNeedOnTracker(tracker);
                            Log.Information("Successfully Ran 1min Process");
                        }
                    }
                    break;
                case TrackInterval.FiveMin:
                    foreach (TrackedProduct tracker in Constants.SavedData.TrackedProducts5Min)
                    {
                        if (tracker.Enabled)
                        {
                            ProcessAlertNeedOnTracker(tracker);
                        }
                    }
                    break;
            }
        }

        public static async void ProcessAlertNeedOnTracker(TrackedProduct tracker)
        {
            try
            {
                Log.Verbose("Processing alert for tracker", tracker);
                bool keywordFound = (await Communication.DoesKeywordExistOnWebpage(tracker.PageURL, tracker.Keyword)).KeywordExists;

                if ((keywordFound && !tracker.AlertOnKeywordNotExist) || (!keywordFound && tracker.AlertOnKeywordNotExist))
                {
                    if (!tracker.Triggered)
                    {
                        Log.Debug("Alerting on tracker as logic matches", tracker, keywordFound);
                        ProcessAlertToSend(tracker);
                    }
                }
                else
                {
                    if (tracker.Triggered)
                    {
                        Log.Debug("Alerting on tracker as logic matches", tracker, keywordFound);
                        ProcessAlertToReset(tracker);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error on tracker: [{Tracker}]{Error}", tracker.FriendlyName, ex.Message);
            }
        }

        public static void ProcessAlertToSend(TrackedProduct tracker)
        {
            Log.Debug("Processing Alert Type", tracker.AlertType);
            tracker.Triggered = true;
            switch (tracker.AlertType)
            {
                case Alert.Email:
                    Communication.SendAlertEmail(tracker);
                    break;
                case Alert.Webhook:
                    Communication.SendAlertWebhookDiscord(tracker, _color: "2813191");
                    break;
                case Alert.Email_Webhook:
                    Log.Warning("Processed Alert Type Webhook + Email when it isn't implemented yet", tracker);
                    break;
            }
        }

        public static void ProcessAlertToReset(TrackedProduct tracker)
        {
            Log.Debug("Processing Alert Type", tracker.AlertType);
            tracker.Triggered = false;
            var msg = $"Alert has cleared for the following page:{Environment.NewLine}{tracker.PageURL}";
            var title = "Alert has been reset on the following tracker, back to waiting :cry:";
            var color = "15730439";
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