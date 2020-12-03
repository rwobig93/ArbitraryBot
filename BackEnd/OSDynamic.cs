using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ArbitraryBot.Shared;

namespace ArbitraryBot.BackEnd
{
    public static class OSDynamic
    {
        public static string GetStoragePath()
        {
            string basePath = "";
            bool isWindows = OperatingSystem.IsWindows();
            if (isWindows)
            {
                var userPath = Environment.GetEnvironmentVariable(
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                    "LOCALAPPDATA" : "Home");
                ProductAssembly proAss = GetProductAssembly();
                basePath = Path.Combine(Path.Combine(userPath, proAss.CompanyName), proAss.ProductName);
                if (Constants.DebugMode)
                    basePath = Path.Combine(basePath, "Test");
                else
                    basePath = Path.Combine(basePath, "Prod");
            }
            return basePath;
        }

        internal static ProductAssembly GetProductAssembly()
        {
            var assy = Assembly.GetExecutingAssembly();
            return new ProductAssembly
            {
                CompanyName = assy.GetCustomAttributes<AssemblyCompanyAttribute>()
                  .FirstOrDefault().Company,
                ProductName = assy.GetCustomAttribute<AssemblyProductAttribute>().Product
            };
        }

        internal static Version GetRunningVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        internal static string GetConfigPath()
        {
            return Path.Combine(GetStoragePath(), "Config");
        }

        internal static string GetLoggingPath()
        {
            return Path.Combine(GetStoragePath(), "Logs");
        }

        internal static string GetSavedDataPath()
        {
            return Path.Combine(GetStoragePath(), "SavedData");
        }

        internal static string GetFilePath(string directory, string fileName)
        {
            return Path.Combine(directory, fileName);
        }

        public static OSPlatform GetCurrentOS()
        {
            if (OperatingSystem.IsWindows())
            {
                return OSPlatform.Windows;
            }
            else if (OperatingSystem.IsLinux())
            {
                return OSPlatform.Linux;
            }
            else if (OperatingSystem.IsMacOS())
            {
                return OSPlatform.OSX;
            }
            else
            {
                return OSPlatform.FreeBSD;
            }
        }

        internal static void OpenPath(string directory)
        {
            if (OperatingSystem.IsWindows())
            {
                Process proc = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "C:\\Windows\\explorer.exe",
                        Arguments = directory
                    }
                };
                proc.Start();
            }
        }
    }
}
