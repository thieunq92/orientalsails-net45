using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CMS.Core.Util
{
    /// <summary>
    /// The Text class contains helper methods for text manipulation.
    /// </summary>
    public class Text
    {
        private Text()
        {
        }

        /// <summary>
        /// Truncate a given text to the given number of characters. 
        /// Also any embedded html is stripped.
        /// </summary>
        /// <param name="fullText"></param>
        /// <param name="numberOfCharacters"></param>
        /// <returns></returns>
        public static string TruncateText(string fullText, int numberOfCharacters)
        {
            string text;
            Regex regexStripHTML = new Regex("<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            fullText = regexStripHTML.Replace(fullText, " ");
            if (fullText.Length > numberOfCharacters)
            {
                int spacePos = fullText.IndexOf(" ", numberOfCharacters);
                if (spacePos > -1)
                {
                    text = string.Format("{0}...", fullText.Substring(0, spacePos));
                }
                else
                {
                    text = fullText;
                }
            }
            else
            {
                text = fullText;
            }
            return text;
        }

        public static string HTMLRemoveTag(string text)
        {
            Regex regexStripHTML = new Regex("<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = regexStripHTML.Replace(text, " ");
            return text;
        }

        /// <summary>
        /// Ensure that the given string has a trailing slash.
        /// </summary>
        /// <param name="stringThatNeedsTrailingSlash"></param>
        /// <returns></returns>
        public static string EnsureTrailingSlash(string stringThatNeedsTrailingSlash)
        {
            if (! stringThatNeedsTrailingSlash.EndsWith("/"))
            {
                return string.Format("{0}/", stringThatNeedsTrailingSlash);
            }
            return stringThatNeedsTrailingSlash;
        }

        /// <summary>
        /// Ensure that the given string has a trailing slash.
        /// </summary>
        /// <param name="stringThatNeedsSlash"></param>
        /// <returns></returns>
        public static string EnsureSlash(string stringThatNeedsSlash)
        {
            if (!stringThatNeedsSlash.EndsWith(@"\"))
            {
                return string.Format(@"{0}\", stringThatNeedsSlash);
            }
            return stringThatNeedsSlash;
        }

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        /// <summary>
        /// Hợp từ NameValueCollection thành QueryString
        /// </summary>
        /// <param name="qs">QueryString ở dạnh NameValueCollection</param>
        /// <returns></returns>
        public static string JoinNvcToQs(NameValueCollection qs)
        {
            return string.Join("&", Array.ConvertAll<string, string>(qs.AllKeys, delegate(string key)
            {
                return string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(qs[key]));
            }));
        }

        public static string Padding(int count)
        {
            if (count == 0)
            {
                return string.Empty;
            }
            string[] s = new string[count];
            for (int i = 0; i < count; ++i)
            {
                s[i] = "&nbsp;";
            }
            return HttpUtility.HtmlDecode(string.Join("", s));
        }

        public static string RomanNumeralize(int number)
        {
            // Validate
            if (number < 0 || number > 3999)
            {
                throw new ArgumentException("Value must be in the range 0 - 3,999.");
            }
            if (number == 0) return "N";
            // Set up key numerals and numeral pairs
            int[] values = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            string[] numerals = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            // Initialise the string builder
            StringBuilder result = new StringBuilder();
            // Loop through each of the values to diminish the number
            for (int i = 0; i < 13; i++)
            {
                // If the number being converted is less than the test value, append
                // the corresponding numeral or numeral pair to the resultant string
                while (number >= values[i])
                {
                    number -= values[i];
                    result.Append(numerals[i]);
                }
            }
            // Done
            return result.ToString();
        }
    }
}