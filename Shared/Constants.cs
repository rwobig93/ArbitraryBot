using System.Collections.Generic;
using ArbitraryBot.BackEnd;
using Serilog.Core;

namespace ArbitraryBot.Shared
{
    public static class Constants
    {
        // Classes
        public static LoggingLevelSwitch LogLevelLocal { get; set; } = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);
        public static LoggingLevelSwitch LogLevelCloud { get; set; } = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning);
        public static LoggingLevelSwitch LogLevelConsole { get; set; } = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Error);
        public static Config Config { get; set; }
        public static SavedData SavedData { get; set; }
        // Strings
        public static string PathLogs => OSDynamic.GetLoggingPath();
        public static string PathConfigDefault => OSDynamic.GetConfigPath();
        public static string PathSavedData => OSDynamic.GetSavedDataPath();
        public static string LogUri { get; set; }
        public static string WebHookAvatarURL { get; set; } = "https://wobigtech.net/wp-content/uploads/2020/10/WobigIconnoBack-50x50.png";
        public static List<string> Notifications { get; set; } = new List<string>(5);
        // Bools
        public static bool CloseApp { get; set; } = false;
        public static bool DebugMode { get; set; } = false;
        public static bool BetaMode { get; set; } = false;
        public static bool UpdateReady { get; set; } = false;
    }
}
