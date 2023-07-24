using System.Collections.Generic;

namespace UniversalMailTaker
{
    static class Extensions
    {
        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        public static int CountAllOccurances(this string value, char itemToSearch)
        {
            return value.Split(itemToSearch).Length - 1;
        }

        public static int CountAllOccurances(this string value, string itemToSearch)
        {
            return (value.Length - value.Replace(itemToSearch, "").Length) / itemToSearch.Length;
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
