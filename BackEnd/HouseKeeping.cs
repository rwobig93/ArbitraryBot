using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using ArbitraryBot.Shared;
using ArbitraryBot.Extensions;
using System.IO;

namespace ArbitraryBot.BackEnd
{
    public static class HouseKeeping
    {
        internal static void ValidateAllFilePaths()
        {
            List<string> directories = new List<string>();

            directories.Add(Constants.PathConfigDefault);
            directories.Add(Constants.PathLogs);

            foreach (var dir in directories)
            {
                try
                {
                    if (!Directory.Exists(dir))
                    {
                        Log.Debug($"Creating missing directory: {dir}");
                        Directory.CreateDirectory(dir);
                        Log.Information($"Created missing directory: {dir}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to create required directory");
                }
            }
            Log.Information("Finished validating required file paths");
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
                        if (fI.LastWriteTime < DateTime.Now.AddDays(-30))
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
                Log.Error(ex, $"Failed to cleanup old {appFile} files");
            }
        }
    }
}
