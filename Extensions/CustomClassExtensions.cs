using System.Collections.Generic;
using ArbitraryBot.Shared;
using Serilog;

namespace ArbitraryBot.Extensions
{
    public static class CustomClassExtensions
    {
        public static void Save(this TrackedProduct tracker)
        {
            List<TrackedProduct> selectedList;
            TrackedProduct foundTracker;
            switch (tracker.AlertInterval)
            {
                case TrackInterval.OneMin:
                    selectedList = Constants.SavedData.TrackedProducts1Min;
                    break;
                case TrackInterval.FiveMin:
                    selectedList = Constants.SavedData.TrackedProducts5Min;
                    break;
                default:
                    Log.Warning("Default was hit on a switch that shouldn't occur", tracker);
                    selectedList = Constants.SavedData.TrackedProducts5Min;
                    break;
            }

            Log.Debug("Adding or updating tracker on the {Interval} queue: {Tracker}", tracker.AlertInterval, tracker.FriendlyName);
            foundTracker = selectedList.Find(x => x.TrackerID == tracker.TrackerID);
            if (foundTracker != null)
            {
                foundTracker = tracker;
                Log.Debug("Updated tracker on the {Interval} queue: {Tracker}", tracker.AlertInterval, tracker.FriendlyName);
            }
            else
            {
                selectedList.Add(tracker);
                Log.Debug("Added tracker on the {Interval} queue: {Tracker}", tracker.AlertInterval, tracker.FriendlyName);
            }
        }

        public static void Delete(this TrackedProduct tracker)
        {
            List<TrackedProduct> selectedList;
            TrackedProduct foundTracker;
            switch (tracker.AlertInterval)
            {
                case TrackInterval.OneMin:
                    selectedList = Constants.SavedData.TrackedProducts1Min;
                    break;
                case TrackInterval.FiveMin:
                    selectedList = Constants.SavedData.TrackedProducts5Min;
                    break;
                default:
                    Log.Warning("Default was hit on a switch that shouldn't occur", tracker);
                    selectedList = Constants.SavedData.TrackedProducts5Min;
                    break;
            }

            Log.Debug("Removing tracker on {Interval} queue: {Tracker}", tracker.AlertInterval, tracker.FriendlyName);
            foundTracker = selectedList.Find(x => x.TrackerID == tracker.TrackerID);
            if (foundTracker != null)
            {
                Log.Debug("Removed tracker on {Interval} queue: {Tracker}", tracker.AlertInterval, tracker.FriendlyName);
                selectedList.Remove(foundTracker);
            }
            else
            {
                Log.Warning("Attempted to remove tracker on {Interval} queue: {Tracker} | Couldn't find the tracker", tracker.AlertInterval, tracker.FriendlyName);
            }
        }
    }
}
