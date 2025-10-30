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
    /// An instance of a specific sub-component/dependency
    /// </summary>
    public sealed class HealthCheckComponentInstance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckComponentInstance"/> class.
        /// </summary>
        public HealthCheckComponentInstance()
        {
        }

        /// <summary>
        /// A unique identifier of an instance of a specific sub-component/dependency of 
        /// a service
        /// </summary>
        [JsonPropertyOrder(1)]
        [JsonPropertyName("componentId")]
        public string ComponentId { get; set; }

        /// <summary>
        /// The type of the component. Pre-defined value are: component, datastore, system
        /// </summary>
        [JsonPropertyOrder(2)]
        [JsonPropertyName("componentType")]
        public HealthCheckComponentType? ComponentType { get; set; }

        /// <summary>
        /// A common and standard term from a well-known source such as schema.org, IANA, 
        /// microformats, or a standards document such as RFC3339.
        /// </summary>
        [JsonPropertyOrder(5)]
        [JsonPropertyName("observedUnit")]
        public HealthCheckUnit? ObservedUnit { get; set; }

        /// <summary>
        /// Could be any valid JSON value, such as: string, number, object, array 
        /// or literal.
        /// </summary>
        [JsonPropertyOrder(4)]
        [JsonPropertyName("observedValue")]
        public object ObservedValue { get; set; }

        /// <summary>
        /// Status of the sub-component/dependency.
        /// </summary>
        [JsonPropertyOrder(3)]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(HealthStatusJsonConverter))]
        public HealthStatus? Status { get; set; }

        /// <summary>
        /// The date-time at which the reading of the observed value was recorded.
        /// </summary>
        [JsonPropertyOrder(6)]
        [JsonPropertyName("time")]
        public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;
    }
}
