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

using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RestCaptcha
{
    /// <summary>
    /// Simple client for querying AbuseIPDB.
    /// </summary>
    /// <remarks>
    /// See https://www.abuseipdb.com/ for more information.
    /// </remarks>
    public sealed class AbuseIpDbClient
    {
        private const string _baseUrl = "https://api.abuseipdb.com/api/v2/check";
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;
        private readonly AbuseIPDBConfiguration _configuration;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbuseIpDbClient"/> class.
        /// </summary>
        /// <param name="httpClient">Injected HTTP client</param>
        /// <param name="cache">Injected cache for IP addresses</param>
        /// <param name="configuration">Configuration data</param>
        public AbuseIpDbClient(HttpClient httpClient, IMemoryCache cache, AbuseIPDBConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cache = cache;
            _cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
        }

        /// <summary>
        /// Checks the reputation for a given IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The value of the TResult parameter 
        /// contains a <see cref="AbuseIpDbCheckResult"/> instance.</returns>
        public async Task<AbuseIpDbCheckResult> CheckAsync(string ipAddress, CancellationToken cancellationToken = default)
        {
            var ipAddressKey = $"abuseipdb:{ipAddress}";

            if (_cache.TryGetValue(ipAddressKey, out AbuseIpDbCheckResult cachedResult))
            {
                return cachedResult;
            }
            else
            {
                var result = await CheckWithoutCacheAsync(ipAddress, cancellationToken);

                _cache.Set(ipAddressKey, result, _cacheEntryOptions);

                return result;
            }
        }

        /// <summary>
        /// Checks the reputation for a given IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The value of the TResult parameter 
        /// contains a <see cref="AbuseIpDbCheckResult"/> instance.</returns>
        private async Task<AbuseIpDbCheckResult> CheckWithoutCacheAsync(string ipAddress, CancellationToken cancellationToken = default)
        {
            var ipAddressEncoded = Uri.EscapeDataString(ipAddress);

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}?ipAddress={ipAddressEncoded}&maxAgeInDays=90");

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            request.Headers.Add("Key", _configuration.ApiKey);

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            using var reponseBody = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(cancellationToken));

            return JsonSerializer.Deserialize<AbuseIpDbCheckResult>(reponseBody.RootElement.GetProperty("data"));
        }
    }
}