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

using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json.Serialization;

namespace RestCaptcha
{
    /// <summary>
    /// Representation of a Health Check Response for HTTP APIs
    /// </summary>
    /// <param name="status">The service status</param>
    public sealed class HealthCheckDetails(HealthStatus status)
    {
        /// <summary>
        /// Provides detailed health statuses of additional downstream systems and endpoints which 
        /// can affect the overall health of the main API.
        /// </summary>
        [JsonIgnore]
        public IList<HealthCheckComponent> Components { get; set; } = [];

        /// <summary>
        /// A unique identifier of the service, in the application scope.
        /// </summary>
        [JsonPropertyOrder(3)]
        [JsonPropertyName("serviceId")]
        public string ServiceId { get; set; }

        /// <summary>
        /// Indicates whether the service status is acceptable or not.
        /// </summary>
        [JsonPropertyOrder(1)]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(HealthStatusJsonConverter))]
        public HealthStatus Status { get; } = status;

        /// <summary>
        /// Public version of the service
        /// </summary>
        [JsonPropertyOrder(2)]
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// This calculated property is needed for JSON serialisation.
        /// </summary>
        [JsonPropertyOrder(4)]
        [JsonPropertyName("checks")]
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        private Dictionary<string, List<HealthCheckComponentInstance>> Checks
        { 
            get
            {
                return Components is null || Components.Count == 0
                   ? null
                   : Components
                        .GroupBy(c => StringHelper.Combine(":", c.ComponentName, c.MeasurementName.GetJsonStringEnumMemberName()))
                        .ToDictionary(g => g.Key, g => g.Where(x => x.Instances != null)
                            .SelectMany(x => x.Instances!)
                            .ToList());
            }
        }
    }
}
