using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonModels.Helpers
{
    public static class StringExtensions
    {
        public static string NormalizeToMaxLength1024(this string s)
        {
            if (s == null)
                return null;

            return s.Length > 1024 ? s.Substring(0, 1024) : s;
        }
    }

    public static class DateTimeExtensions
    {
        public static long GetTimestamp(this DateTime d)
        {
            return (long)(d.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
