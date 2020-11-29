using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbitraryBot.Shared;
using Serilog;

namespace ArbitraryBot.Extensions
{
    public static class CustomClassExtensions
    {
        public static void Save(this TrackedProduct tracker)
        {
            TrackedProduct foundTracker;
            switch (tracker.AlertInterval)
            {
                case TrackInterval.OneMin:
                    Log.Debug("Adding or updating tracker on 1min queue: {Tracker}", tracker.FriendlyName);
                    foundTracker = Constants.SavedData.TrackedProducts1Min.Find(x => x.TrackerID == tracker.TrackerID);
                    if (foundTracker != null)
                    {
                        foundTracker = tracker;
                        Log.Debug("Updated tracker on 1min queue: {Tracker}", tracker.FriendlyName);
                    }
                    else
                    {
                        Constants.SavedData.TrackedProducts1Min.Add(tracker);
                        Log.Debug("Added tracker on 1min queue: {Tracker}", tracker.FriendlyName);
                    }
                    break;
                case TrackInterval.FiveMin:
                    Log.Debug("Adding or updating tracker on 5min queue: {Tracker}", tracker.FriendlyName);
                    foundTracker = Constants.SavedData.TrackedProducts5Min.Find(x => x.TrackerID == tracker.TrackerID);
                    if (foundTracker != null)
                    {
                        foundTracker = tracker;
                        Log.Debug("Updated tracker on 5min queue: {Tracker}", tracker.FriendlyName);
                    }
                    else
                    {
                        Constants.SavedData.TrackedProducts5Min.Add(tracker);
                        Log.Debug("Added tracker on 5min queue: {Tracker}", tracker.FriendlyName);
                    }
                    break;
                default:
                    Log.Warning("Default was hit on a switch that shouldn't occur", tracker);
                    Constants.SavedData.TrackedProducts5Min.Add(tracker);
                    break;
            }
        }
    }
}
