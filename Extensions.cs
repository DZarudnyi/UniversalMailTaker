using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalMailTaker
{
    static class Extensions
    {
        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        public static IEnumerable<int> AllIndexesOf(this string str, string searchstring)
        {
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
            }
        }

        public static bool IsNumber(this string value)
        {
            return int.TryParse(value, out _);
        }
    }
}
