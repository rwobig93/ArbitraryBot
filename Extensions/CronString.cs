﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbitraryBot.Extensions
{
    public static class CronString
    {
        public static string Minutely()
        {
            return "*/1 * * * *";
        }
        public static string MinuteInterval(int interval)
        {
            return $"*/{interval} * * * *";
        }
        public static string Hourly()
        {
            return "0 */1 * * *";
        }
        public static string HourInterval(int interval)
        {
            return $"0 */{interval} * * *";
        }
        public static string Daily()
        {
            return "0 0 */1 * *";
        }
        public static string DayInterval(int interval)
        {
            return $"0 0 */{interval} * *";
        }
        public static string Monthly()
        {
            return "0 0 1 */1 *";
        }
        public static string MonthInterval(int interval)
        {
            return $"0 0 1 */{interval} *";
        }
    }
}
