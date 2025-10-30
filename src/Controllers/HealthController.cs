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

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;

namespace RestCaptcha
{
    /// <summary>
    /// RESTCaptcha health check controller
    /// </summary>
    [ApiController]
    [ApiVersionNeutral]
    [AllowAnonymous]
    [SwaggerTag("RESTCaptcha health check endpoint")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _health;
        private readonly IAppMetadata _serviceMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthController"/> class.
        /// </summary>
        /// <param name="health">Injected health check service</param>
        /// <param name="serviceMetadata">Injected service metadata</param>
        public HealthController(HealthCheckService health, IAppMetadata serviceMetadata)
        {
            _health = health;
            _serviceMetadata = serviceMetadata;
        }

        /// <summary>
        /// Health endpoint
        /// </summary>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>/// <returns>A <see cref="HealthCheckDetails"/> object</returns></returns>
        [HttpGet("health")]
        [HealthCheckApiKey]
        [ProducesResponseType(typeof(HealthCheckDetails), StatusCodes.Status200OK, MediaTypeNames.Application.HealthCheckDetails)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemDetails)]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Check(CancellationToken cancellationToken = default)
        {
            var report = await _health.CheckHealthAsync(cancellationToken);
            
            return StatusCode(
                report.Status switch
                {
                    HealthStatus.Healthy => StatusCodes.Status200OK,
                    HealthStatus.Degraded => StatusCodes.Status200OK,
                    HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
                    _ => StatusCodes.Status503ServiceUnavailable
                },
                new HealthCheckDetails(report.Status)
                {
                    Version = _serviceMetadata.Version,
                    ServiceId = _serviceMetadata.ServiceId,
                    Components = [ 
                        new HealthCheckComponent(HealthCheckMeasurementName.UpTime) {
                            Instances = [
                                new HealthCheckComponentInstance() {
                                    ComponentType = HealthCheckComponentType.System,
                                    ObservedValue = (int)(DateTimeOffset.UtcNow - _serviceMetadata.StartedAt).TotalSeconds,
                                    ObservedUnit = HealthCheckUnit.Seconds
                                }
                            ]
                        }
                    ]
                });
        }
    }
}