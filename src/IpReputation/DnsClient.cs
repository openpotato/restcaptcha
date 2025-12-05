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

using System.Net;

namespace RestCaptcha
{
    /// <summary>
    /// DNS lookup client
    /// </summary>
    public sealed class DnsClient : IDnsClient
    {
        private readonly ILogger<DnsClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsClient"/> class.
        /// </summary>
        /// <param name="logger">Injected cache for IP addresses</param>
        public DnsClient(ILogger<DnsClient> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Resolves a host name or IP address to an <see cref="IPHostEntry"/> instance.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The value of the TResult parameter 
        /// contains a <see cref="IPHostEntry"/> instance.</returns>
        public async Task<IPHostEntry> GetHostEntryAsync(string hostNameOrAddress, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start DNS lookup for {HostNameOrAddress}", hostNameOrAddress);
            var result = await Dns.GetHostEntryAsync(hostNameOrAddress, cancellationToken);
            _logger.LogInformation("Recieved DNS lookup response for {HostName}", result.HostName);
            return result;
        }
    }
}