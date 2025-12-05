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

using System.Security.Cryptography;

namespace RestCaptcha
{
    /// <summary>
    /// Represents the result of a Spamhaus service IP check
    /// </summary>
    public sealed class SpamhausCheckResult
    {
        /// <summary>
        /// Which list the IP hit?
        /// </summary>
        public SpamhausDatabase Database { get; set; }

        /// <summary>
        /// The IP address that's being checked.
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// The raw 127.x.x.x address returned by Spamhaus, if any.
        /// </summary>
        public string RawReturnAddress { get; set; }

        /// <summary>
        /// Compact string representation of the challenge type
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{IpAddress}:{Database}";
        }
    }
}