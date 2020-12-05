using System;

namespace ArbitraryBot.Extensions
{
    public static class StringExtensions
    {
        public static string ConvertToMenuOption(this string _string, int menuNumber)
        {
            int maxLength = 70;
            if (menuNumber >= 10)
            {
                maxLength--;
            }
            if (menuNumber >= 100)
            {
                maxLength--;
            }
            if (_string.Length > maxLength)
            {
                _string = _string.Substring(0, maxLength);
            }
            else
            {
                while (_string.Length < maxLength)
                {
                    _string += " ";
                }
            }
            return $"|  {menuNumber}. {_string}|{Environment.NewLine}";
        }

        public static string ConvertToMenuTitle(this string _string)
        {
            int lengthTotal = 75;
            int padLeft = 75;
            if (_string.Length > 75)
            {
                _string = _string.Substring(0, padLeft);
            }
            padLeft -= _string.Length;
            padLeft = (int)Math.Round((double)padLeft / 2) + _string.Length;
            return $"|{_string.PadLeft(padLeft, ' ').PadRight(lengthTotal, ' ')}|{Environment.NewLine}";
        }

        public static string ConvertToMenuProperty(this string _string, string value, int propNameMax = 14)
        {
            if (_string.Length > propNameMax)
            {
                _string = _string.Substring(0, propNameMax);
            }
            return $"|{_string.PadLeft(1 + _string.Length, ' ').PadRight(propNameMax + 2)}: {value}{Environment.NewLine}";
        }

        public static string ConvertToNotification(this string _string)
        {
            if (_string.Length > 65)
            {
                _string = _string.Substring(0, 65);
            }
            return $"| Notifiy: {_string,-65}|{Environment.NewLine}";
        }

        public static string AddSeperatorDashed(this string _string)
        {
            return _string += $"|  -----------------------------------------------------------------------  |{Environment.NewLine}";
        }

        public static string AddSeperatorTilde(this string _string)
        {
            return _string += $"|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{Environment.NewLine}";
        }

        public static bool IsCompressed(this string _string)
        {
            return _string.Contains("�");
        }
    }
}
