using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static string PathLogs { get; set; } = OSDynamic.GetLoggingPath();
        public static string PathConfigDefault { get; set; } = OSDynamic.GetConfigPath();
        public static string PathSavedData { get; set; } = OSDynamic.GetSavedDataPath();
        // Bools
        public static bool CloseApp { get; set; } = false;
        public static string WebHookAvatarURL { get; set; } = "https://wobigtech.net/wp-content/uploads/2020/10/WobigIconnoBack-50x50.png";
    }
}
