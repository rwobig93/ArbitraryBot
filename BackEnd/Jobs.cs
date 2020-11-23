using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbitraryBot.Extensions;
using ArbitraryBot.Shared;
using Hangfire;
using Hangfire.MemoryStorage;
using Serilog;

namespace ArbitraryBot.BackEnd
{
    public static class Jobs
    {
        public static BackgroundJobServer BackgroundJobServer { private set; get; }
        public static StatusReturn InitializeJobService()
        {
            try
            {
                if (BackgroundJobServer != null)
                {
                    Log.Warning("Background Job Server is already initialized and was attempted to be started");
                }
                else
                {
                    GlobalConfiguration.Configuration.UseSerilogLogProvider();
                    GlobalConfiguration.Configuration.UseMemoryStorage();
                    BackgroundJobServer = new BackgroundJobServer();
                }
                return StatusReturn.Success;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Background Job Service Failed to Start");
                return StatusReturn.Failure;
            }
        }

        public static StatusReturn StopJobService()
        {
            try
            {
                Log.Debug("Attempting to stop the background job server");
                BackgroundJobServer.Dispose();
                Log.Information("Successfully stopped the background job server");
                return StatusReturn.Success;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to stop the Background Job Server");
                return StatusReturn.Failure;
            }
        }

        public static StatusReturn StartAllTimedJobs()
        {
            try
            {
                StartJobDataSaver();
                StartJobCleanup();
                //StartJobWatcherOneMin();
                //StartJobWatcherFiveMin();
                StartJobTestJob();
                return StatusReturn.Success;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to start all timed jobs");
                return StatusReturn.Failure;
            }
        }

        private static void StartJobTestJob()
        {
            RecurringJob.AddOrUpdate(() => Core.TestMethod(), CronString.Minutely());
        }

        private static void StartJobWatcherFiveMin()
        {
            throw new NotImplementedException();
        }

        private static void StartJobWatcherOneMin()
        {
            throw new NotImplementedException();
        }

        private static void StartJobCleanup()
        {
            RecurringJob.AddOrUpdate(() => Core.CleanupAllOldFiles(), CronString.Daily());
        }

        private static void StartJobDataSaver()
        {
            RecurringJob.AddOrUpdate(() => Core.SaveEverything(), CronString.MinuteInterval(5));
        }
    }
}
