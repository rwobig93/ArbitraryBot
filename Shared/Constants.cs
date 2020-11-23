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
        public static LoggingLevelSwitch LogLevel { get; set; }
        public static LoggingLevelSwitch LogLevelConsole { get; set; } = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Error);
        public static Config Config { get; set; }
        // Strings
        public static string PathLogs { get; set; } = OSDynamic.GetLoggingPath();
        public static string PathConfigDefault { get; set; } = OSDynamic.GetConfigPath();
        // Bools
        public static bool CloseApp { get; set; } = false;
    }
}
