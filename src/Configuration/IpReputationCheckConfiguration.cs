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

namespace RestCaptcha
{
    /// <summary>
    /// IP reputation check configuration
    /// </summary>
    public class IpReputationCheckConfiguration
    {
        /// <summary>
        /// Check for abusive IPs via AbuseIPDB integration
        /// </summary>
        public AbuseIPDBConfiguration AbuseIPDB { get; set; } = new();

        /// <summary>
        /// Check for abusive IPs via Spamhaus integration
        /// </summary>
        public SpamhausConfiguration Spamhaus { get; set; } = new();
    }
}
