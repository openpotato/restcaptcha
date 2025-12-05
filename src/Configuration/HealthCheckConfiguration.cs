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
    /// Configuration for the health check endpoint
    /// </summary>
    public class HealthCheckConfiguration
    {
        /// <summary>
        /// List of trusted network ranges in CIDR (Classless Inter-Domain Routing) notation.
        /// Requests from these ranges are allowed without an API-key when <see cref="PrivateOnly"/> is false.
        /// In <see cref="PrivateOnly"/> mode, these ranges (plus loopback if enabled) are the only ones allowed.
        /// </summary>
        /// <example>
        /// <code language="json">
        /// "AllowCidrs": [ "10.0.0.0/8", "192.168.0.0/16", "fd00::/8" ]
        /// </code>
        /// </example>        
        [JsonPropertyOrder(3)]
        public string[] AllowCidrs { get; set; } = [];

        /// <summary>
        /// Whether to allow requests from local loopback addresses.
        /// </summary>
        /// <remarks>
        /// Behind a proxy, ensure forwarded headers are configured so the effective client IP is correct.
        /// </remarks>
        [JsonPropertyOrder(2)]
        public bool AllowLocal { get; set; } = true;

        /// <summary>
        /// Should the health check endpoint be exposed?
        /// </summary>
        [JsonPropertyOrder(0)]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// A list of expected API-key values to validate when the request is not allowed by
        /// <see cref="AllowLocal"/> or <see cref="AllowCidrs"/>, and <see cref="PrivateOnly"/> is false.
        /// Leave empty only if public access or using private-only access.
        /// </summary>
        [JsonPropertyOrder(4)]
        public string[] ApiKeys { get; set; } = [];

        /// <summary>
        /// If true, the endpoint is private: only loopback and <see cref="AllowCidrs"/> sources are permitted;
        /// <see cref="ApiKeys"/> is ignored and public access is denied.
        /// </summary>
        [JsonPropertyOrder(1)]
        public bool PrivateOnly { get; set; } = false;
    }
}
