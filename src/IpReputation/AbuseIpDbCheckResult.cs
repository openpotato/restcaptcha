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

using System.Text.Json.Serialization;

namespace RestCaptcha
{
    /// <summary>
    /// Represents the result of a AbuseIPDB service IP check
    /// </summary>
    public sealed class AbuseIpDbCheckResult
    {
        [JsonPropertyName("abuseConfidenceScore")]
        public int AbuseConfidenceScore { get; set; }

        /// <summary>
        /// The ISO 3166 country code where this IP address is located.
        /// </summary>
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }

        /// <summary>
        /// The amount of distinct users that have ever reported this IP address.
        /// </summary>
        [JsonPropertyName("numDistinctUsers")]
        public int DistinctUserCount { get; set; }

        /// <summary>
        /// The domain name associated with this IP address.
        /// </summary>
        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// The hostnames that are pointing to this IP address.
        /// </summary>
        [JsonPropertyName("hostnames")]
        public string[] Hostnames { get; set; }

        /// <summary>
        /// The IP address that's being checked.
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// The version of this IP address (4/6).
        /// </summary>
        [JsonPropertyName("ipVersion")]
        public int IPVersion { get; set; }

        /// <summary>
        /// The Internet Service Provider of this IP address.
        /// </summary>
        [JsonPropertyName("isp")]
        public string ISP { get; set; }

        /// <summary>
        /// Whether this is a public or private IP address.
        /// </summary>
        [JsonPropertyName("isPublic")]
        public bool? IsPublic { get; set; }

        /// <summary>
        /// Whether this is a tor exit node.
        /// </summary>
        [JsonPropertyName("isTor")]
        public bool? IsTor { get; set; }

        /// <summary>
        /// Whether this is a whitelisted IP address.
        /// </summary>
        [JsonPropertyName("isWhitelisted")]
        public bool? IsWhitelisted { get; set; }

        /// <summary>
        /// Timepoint of the newest report for this IP address.
        /// </summary>
        [JsonPropertyName("lastReportedAt")]
        public DateTimeOffset? LastReportedAt { get; set; }

        /// <summary>
        /// The total amount of reports that have been submitted for this IP address in the time period.
        /// </summary>
        [JsonPropertyName("totalReports")]
        public int? TotalReports { get; set; }

        /// <summary>
        /// The usage type of this IP address (e.g. Data Center).
        /// </summary>
        [JsonPropertyName("usageType")]
        public string UsageType { get; set; }

        /// <summary>
        /// Compact string representation of the challenge type
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{IpAddress}:{AbuseConfidenceScore}:{TotalReports}:{LastReportedAt}";
        }
    }
}