using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static string AddSeperatorDashed(this string _string)
        {
            return _string += $"|  -----------------------------------------------------------------------  |{Environment.NewLine}";
        }

        public static string AddSeperatorTilde(this string _string)
        {
            return _string += $"|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|{Environment.NewLine}";
        }
    }
}
