using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonModels.Helpers
{
    public static class StringExtensions
    {
        public static string NormalizeToMaxLength255(this string s)
        {
            if (s == null)
                return null;

            return s.Length > 255 ? s.Substring(0, 255) : s;
        }
    }

    public static class DateTimeExtensions
    {
        public static long GetTimestamp(this DateTime d)
        {
            return (long)(d.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }
    }
}
