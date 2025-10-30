#region RESTCaptcha - Copyright (C) STÜBER SYSTEMS GmbH
/*    
 *    RESTCaptcha
 *    
 *    Copyright (C) STÜBER SYSTEMS GmbH
 *
 *    This program is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU Affero General Public License, version 3,
 *    as published by the Free Software Foundation.
 *
 *    This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *    GNU Affero General Public License for more details.
 *
 *    You should have received a copy of the GNU Affero General Public License
 *    along with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 */
#endregion

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace RestCaptcha
{
    /// <summary>
    /// Static string helper class
    /// </summary>
    public static class StringHelper
    {
        private static readonly ConcurrentDictionary<(string Pattern, RegexOptions Options), Regex> _regexCache = new();

        /// <summary>
        /// Joins multiple strings with the given separator, skipping null or empty strings.
        /// </summary>
        /// <param name="separator">The separator value</param>
        /// <param name="values">List of values</param>
        /// <returns>Joined string</returns>
        public static string Combine(string separator, params string[] values)
        {
            return string.Join(separator, values.Where(x => !string.IsNullOrEmpty(x)));
        }

        /// <summary>
        /// Compares two strings in constant time to mitigate timing attacks.
        /// </summary>
        /// <param name="left">First value</param>
        /// <param name="right">Second value</param>
        /// <returns>TRUE if equal, otherwise FALSE</returns>
        public static bool FixedTimeEquals(string left, string right)
        {
            var lBytes = System.Text.Encoding.UTF8.GetBytes(left);
            var rBytes = System.Text.Encoding.UTF8.GetBytes(right);
            return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(lBytes, rBytes);
        }

        /// <summary>
        /// Tries to match a value against a list of regex patterns
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="patterns">The list of regex patterns</param>
        /// <returns>TRUE if matching, otherwise FALSE</returns>
        public static bool Match(string value, IEnumerable<string> patterns)
        {
            return Match(value, patterns, TimeSpan.FromMilliseconds(100));
        }

        /// <summary>
        /// Tries to match a value against a list of regex patterns
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="patterns">The list of regex patterns</param>
        /// <param name="timeout">A timeout to prevent the search from taking too long.</param>
        /// <returns>TRUE if matching, otherwise FALSE</returns>
        public static bool Match(string value, IEnumerable<string> patterns, TimeSpan timeout)
        {
            if (string.IsNullOrEmpty(value) || patterns is null)
            {
                return false;
            }

            foreach (var pattern in patterns)
            {
                if (string.IsNullOrWhiteSpace(pattern)) continue;
                try
                {
                    var regex = _regexCache.GetOrAdd(
                        ($@"\A(?:{pattern})\z", RegexOptions.CultureInvariant | RegexOptions.Compiled),
                        key => new Regex(key.Pattern, key.Options, timeout)
                    );

                    if (regex.IsMatch(value))
                    {
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                    // Ignore invalid regex
                }
            }

            return false;
        }
    }
}
