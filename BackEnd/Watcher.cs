using System;
using System.Collections.Generic;
using ArbitraryBot.FrontEnd;
using ArbitraryBot.Shared;
using Serilog;

namespace ArbitraryBot.BackEnd
{
    public static class Watcher
    {
        public static void CheckOnTrackers(TrackInterval interval)
        {
            Log.Debug("Running Tracker Check: {Interval}", interval);
            List<TrackedProduct> selectedList = interval switch
            {
                TrackInterval.OneMin => Constants.SavedData.TrackedProducts1Min,
                TrackInterval.FiveMin => Constants.SavedData.TrackedProducts5Min,
                _ => Constants.SavedData.TrackedProducts5Min,
            };
            foreach (TrackedProduct tracker in selectedList)
            {
                if (tracker.Enabled)
                {
                    Log.Verbose("Attempting to Run {Interval} Process: {Tracker}", interval, tracker.FriendlyName);
                    ProcessAlertNeedOnTracker(tracker);
                    Log.Verbose("Successfully Ran {Interval} Process: {Tracker}", interval, tracker.FriendlyName);
                }
                else
                {
                    Log.Verbose("Tracker {Tracker} is disabled, skipping it", tracker.FriendlyName);
                }
            }
        }

        public static async void ProcessAlertNeedOnTracker(TrackedProduct tracker)
        {
            try
            {
                Log.Verbose("Processing alert for tracker", tracker);
                WebCheck attempt1 = (await Communication.WebCheckForKeyword(tracker.PageURL, tracker.Keyword));
                if (attempt1 == null)
                {
                    Log.Verbose("Attempt1 page is empty, not alerting");
                    return;
                }
                WebCheck attempt2 = (await Communication.WebCheckForKeyword(tracker.PageURL, tracker.Keyword));
                if (attempt2 == null)
                {
                    Log.Verbose("Attempt2 page is empty, not alerting");
                    return;
                }

                if (attempt1.KeywordExists == attempt2.KeywordExists)
                {
                    if ((attempt1.KeywordExists && !tracker.AlertOnKeywordNotExist) || (!attempt1.KeywordExists && tracker.AlertOnKeywordNotExist))
                    {
                        if (!tracker.Triggered)
                        {
                            Log.Debug("Alerting on tracker as logic matches", tracker, attempt1.KeywordExists);
                            ProcessAlertToSend(tracker);
                        }
                    }
                    else
                    {
                        if (tracker.Triggered)
                        {
                            Log.Debug("Alerting on tracker as logic matches", tracker, attempt1.KeywordExists);
                            ProcessAlertToReset(tracker);
                        }
                    }
                }
                else
                {
                    Log.Verbose("Keyword found [{KWFound}] and Validation [{KWValidation}] don't match, not alerting", attempt1.KeywordExists, attempt2.KeywordExists);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error on tracker: [{Tracker}]{Error}", tracker.FriendlyName, ex.Message);
                Handler.NotifyError(ex, tracker.FriendlyName);
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
            var title = $"Alert has cleared for the {tracker.FriendlyName}, back to waiting :cry:";
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
            string title = $"Testing alert on the the {tracker.FriendlyName} tracker, Get Pumped!";
            string msg = $"Testing the tracker for the following page: {Environment.NewLine}{tracker.PageURL}";
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