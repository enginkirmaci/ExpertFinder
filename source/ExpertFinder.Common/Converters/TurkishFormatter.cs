using System;
using System.Globalization;

namespace ExpertFinder.Common.Converters
{
    static public class TurkishFormatter
    {
        private static CultureInfo turkish = new CultureInfo("tr-TR");

        static public string ToDayMonthDayname(this DateTime value)
        {
            return value.ToString("dd MMMM dddd", turkish);
        }

        static public string ToShortTrDate(this DateTime value)
        {
            return value.ToString("dd MMMM yyyy", turkish);
        }

        static public string ToHourMinute(this DateTime value)
        {
            return value.ToString("HH:mm", turkish);
        }
    }
}