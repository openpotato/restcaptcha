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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestCaptcha
{
    public sealed class HealthStatusJsonConverter : JsonConverter<HealthStatus>
    {
        public override HealthStatus Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
            => r.GetString()?.ToLowerInvariant() switch
            {
                "pass" => HealthStatus.Healthy,
                "warn" => HealthStatus.Degraded,
                "fail" => HealthStatus.Unhealthy,
                _ => throw new JsonException("Invalid status")
            };

        public override void Write(Utf8JsonWriter w, HealthStatus v, JsonSerializerOptions o)
            => w.WriteStringValue(v switch
            {
                HealthStatus.Healthy => "pass",
                HealthStatus.Degraded => "warn",
                HealthStatus.Unhealthy => "fail",
                _ => "fail"
            });
    }
}
