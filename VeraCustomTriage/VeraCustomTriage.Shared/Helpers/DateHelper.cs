using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VeraCustomTriage.Shared.Helpers
{
    public static class DateHelper
    {
        public static DateTime GetDate(string date)
        {
            //2020-03-23T10:49:30-04:00
            var substring = date.Substring(0, 19);
            var pattern = "yyyy-MM-dd HH:mm:ss";
            DateTime parsedDate;
            DateTime.TryParseExact(substring, pattern, null, DateTimeStyles.None, out parsedDate);
            return parsedDate;
        }
    }
}
