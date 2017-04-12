using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Logdiver.Util
{
    internal static class Extensions
    {
        private static readonly Regex StyleRegex = new Regex(@"<style>(?:\s|.)*?</style>", RegexOptions.Compiled);

        public static string RemoveStyles(this string text)
        {
            return StyleRegex.Replace(text, string.Empty);
        }

        public static string InsertSearch(this string text, string needle, string insert)
        {
            return text.Replace(needle, needle + insert);
        }

        public static string StripTagsCharArray(this string source)
        {
            var array = new char[source.Length];
            var arrayIndex = 0;
            var inside = false;

            foreach (var let in source)
            {
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (@let == '>')
                {
                    inside = false;
                    continue;
                }
                if (inside) continue;
                array[arrayIndex] = let;
                arrayIndex++;
            }
            return new string(array, 0, arrayIndex);
        }

        public static void AddUpper(this Dictionary<string, string> dictionary, string value)
        {
            dictionary.Add(value, value.ToUpper());
        }
    }
}
