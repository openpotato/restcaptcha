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

using Enbrea.ApiKey;
using Microsoft.Extensions.Options;

namespace RestCaptcha
{
    /// <summary>
    /// Provides the effective API-key policy for the health endpoint.
    /// </summary>
    public sealed class HealthCheckApiKeyPolicyProvider : IApiKeyPolicyProvider
    {
        private readonly AppConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckApiKeyPolicyProvider"/> class.
        /// </summary>
        /// <param name="configuration">Injected app configuration</param>
        public HealthCheckApiKeyPolicyProvider(IOptionsMonitor<AppConfiguration> configuration) 
        {
            _configuration = configuration.CurrentValue;
        }

        /// <summary>
        /// Returns the current API-key policy for the health endpoint.
        /// </summary>
        /// <returns>An API-key policy instance</returns>
        public IApiKeyPolicy Get()
        {
            if (_configuration.HealthCheck.Enabled)
            {
                return new ApiKeyPolicy()
                {
                    Keys = _configuration.HealthCheck.ApiKeys,
                    PrivateOnly = _configuration.HealthCheck.PrivateOnly,
                    AllowLocal = _configuration.HealthCheck.AllowLocal,
                    AllowCidrs = _configuration.HealthCheck.AllowCidrs
                };
            }
            else
            {
                return new ApiKeyPolicy()
                {
                    Keys = [],
                    PrivateOnly = true,
                    AllowLocal = false,
                    AllowCidrs = []
                };
            }
        }
    }
}
