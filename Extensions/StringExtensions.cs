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
            _string = _string.Substring(0, maxLength);
            while (_string.Length < maxLength)
            {
                _string += " ";
            }
            return $"  {menuNumber}. {_string}|{Environment.NewLine}";
        }
    }
}
