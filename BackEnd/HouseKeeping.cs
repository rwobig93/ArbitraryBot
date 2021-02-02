using System;
using System.Collections.Generic;
using Serilog;
using ArbitraryBot.Shared;
using System.IO;
using Nito.AsyncEx.Synchronous;
using ArbitraryBot.FrontEnd;

namespace ArbitraryBot.BackEnd
{
    public static class HouseKeeping
    {
        internal static void ValidateAllFilePaths(bool initial = false)
        {
            try
            {
                List<string> directories = new List<string>
                {
                    Constants.PathConfigDefault,
                    Constants.PathLogs,
                    Constants.PathSavedData
                };

                foreach (var dir in directories)
                {
                    try
                    {
                        if (!Directory.Exists(dir))
                        {
                            if (!initial)
                            {
                                Log.Debug($"Creating missing directory: {dir}");
                            }
                            Directory.CreateDirectory(dir);
                            if (!initial)
                            {
                                Log.Information($"Created missing directory: {dir}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!initial)
                        {
                            Log.Error(ex, "Unable to create required directory");
                            Handler.NotifyError(ex, "FolderCreate");
                        }
                    }
                }
                if (!initial)
                {
                    Log.Information("Finished validating required file paths");
                }
            }
            catch (Exception ex)
            {
                if (!initial)
                {
                    Log.Error(ex, "Failed to validate all file paths");
                    Handler.NotifyError(ex, "FolderCreate");
                }
            }
        }

        internal static void CleanupOldFiles(AppFile appFile)
        {
            try
            {
                FileType fileType = FileType.GetFileType(appFile);
                foreach (var file in Directory.EnumerateFiles(fileType.Directory, $"{fileType.FilePrefix}*"))
                {
                    try
                    {
                        FileInfo fI = new FileInfo(file);
                        if (fI.LastWriteTime < DateTime.Now.AddDays(-fileType.RetentionDays))
                        {
                            Log.Debug($"Deleting old {fileType.AppFile} file: {fI.Name}");
                            fI.Delete();
                            Log.Information($"Deleted old {fileType.AppFile} file: {fI.Name}");
                        }
                        else
                        {
                            Log.Debug($"Skipping file that doesn't fall within dateTime constraints: {fI.LastWriteTime} | {fI.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to process old file");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to cleanup old {FileName} files", appFile);
                Handler.NotifyError(ex, "FileCleanup");
            }
        }

        internal static void BackupFiles(AppFile appFile)
        {
            switch (appFile)
            {
                case AppFile.Config:
                    try
                    {
                        Log.Debug("Attempting to backup the current configuration");
                        Config.Backup();
                        Log.Information("Successfully backed up the current configuration");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to backup the current configuration");
                        Handler.NotifyError(ex, "ConfigBackup");
                    }
                    break;
                case AppFile.Log:
                    Log.Warning("Log backups isn't implemented yet but was called anyway");
                    // Need to setup Log backups via aggregation and compression
                    break;
                case AppFile.SavedData:
                    try
                    {
                        Log.Debug("Attempting to backup the current saved data set");
                        SavedData.Backup();
                        Log.Information("Successfully backed up the current saved data set");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to backup the current saved data set");
                        Handler.NotifyError(ex, "DataBackup");
                    }
                    break;
            }
        }

        internal static void ValidateLoggingReqs()
        {
            try
            {
                var url = "https://wobigtech.net/public/public2.txt";
                if (Constants.DebugMode)
                    url = "https://wobigtech.net/public/public1.txt";

                var webReq = Communication.GetWebFileContentsUncompressed(url).WaitAndUnwrapException();
                if (!string.IsNullOrWhiteSpace(webReq.WebpageContents))
                {
                    Constants.LogUri = webReq.WebpageContents.Replace("\n", "");
                }
            }
            catch (Exception ex)
            {
                if (Log.Logger != null)
                {
                    Log.Error(ex, "Failure occured during logging req acquisition: {Error}", ex.Message);
                    Handler.NotifyError(ex, "Logging");
                }
            }
        }

        internal static void ValidateRunningMode()
        {
            #if DEBUG
            Constants.DebugMode = true;
            #endif
            #if BETA
            Constants.BetaMode = true;
            #endif
        }
    }
}