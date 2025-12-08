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
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RestCaptcha
{
    /// <summary>
    /// Simple client for querying the Spamhaus project.
    /// </summary>
    /// <remarks>
    /// See https://docs.spamhaus.com/datasets/docs/source/70-access-methods/data-query-service/000-intro.html for more information. 
    /// For manual checking use https://check.spamhaus.org/.
    /// </remarks>
    public sealed class SpamhausClient
    {
        private const string _zone = "zen.spamhaus.org";
        private readonly IMemoryCache _cache;
        private readonly IDnsClient _dnsClient;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpamhausClient"/> class.
        /// </summary>
        /// <param name="dnsClient">Injected DNS client</param>
        /// <param name="cache">Injected cache for IP addresses</param>
        public SpamhausClient(IDnsClient dnsClient, IMemoryCache cache)
        {
            _dnsClient = dnsClient;
            _cache = cache;
            _cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
        }

        /// <summary>
        /// Checks whether the IP address is listed in the Spamhaus project.
        /// </summary>
        /// <param name="address">The IP address</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The value of the TResult parameter 
        /// contains a <see cref="SpamhausCheckResult"/> instance.</returns>
        public async Task<SpamhausCheckResult> CheckAsync(string address, CancellationToken cancellationToken = default)
        {
            var addressKey = $"spamhaus:{address}";

            if (_cache.TryGetValue(addressKey, out SpamhausCheckResult cachedResult))
            {
                return cachedResult;
            }
            else
            {
                var result = await CheckWithoutCacheAsync(address, cancellationToken);

                _cache.Set(addressKey, result, _cacheEntryOptions);

                return result;
            }
        }

        /// <summary>
        /// Checks whether the IP address is listed in the Spamhaus project.
        /// </summary>
        /// <param name="address">The IP address</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The value of the TResult parameter 
        /// contains a <see cref="SpamhausCheckResult"/> instance.</returns>
        public async Task<SpamhausCheckResult> CheckWithoutCacheAsync(string address, CancellationToken cancellationToken = default)
        {
            var baseResult = new SpamhausCheckResult
            {
                IpAddress = address,
                Database = SpamhausDatabase.NotListed
            };

            if (!TryParseIpAddress(address, out var ipAddress))
            {
                return baseResult;
            }

            try
            {
                var hostEntry = await _dnsClient.GetHostEntryAsync(CreateHostQuery(ipAddress), cancellationToken);

                if (hostEntry.AddressList.Length == 0)
                {
                    return baseResult;
                }

                var retAddr = hostEntry.AddressList[0];

                baseResult.RawReturnAddress = retAddr.ToString();
                baseResult.Database = MapReturnCode(retAddr);

                return baseResult;
            }
            catch 
            {
                return baseResult;
            }
        }

        private static string CreateHostQuery(IPAddress ipAddress)
        {
            return ipAddress.AddressFamily switch
            {
                AddressFamily.InterNetwork => CreateHostQueryForIpv4(ipAddress),
                AddressFamily.InterNetworkV6 => CreateHostQueryForIpv6(ipAddress),
                _ => throw new NotSupportedException("Only IPv4 and IPv6 addresses are supported.")
            };
        }

        private static string CreateHostQueryForIpv4(IPAddress ipAddress)
        {
            // IPv4: 1.2.3.4 -> 4.3.2.1.zen.spamhaus.org

            var octets = ipAddress.ToString().Split('.');
            Array.Reverse(octets);
            return $"{string.Join('.', octets)}.{_zone}";
        }

        private static string CreateHostQueryForIpv6(IPAddress ipAddress)
        {
            // IPv6: full 32 hex digits, reverse nibble-by-nibble, dot-separated.
            // Example: 2001:db8::1 =>
            // 1.0.0.0.0.0.0.0.0.0.0.0.0.0.0.0.8.b.d.0.1.0.0.2.zen.spamhaus.org

            var bytes = ipAddress.GetAddressBytes(); // 16 bytes in network order
            var sb = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2")); // hex, zero-padded
            }

            var hex = sb.ToString().ToCharArray(); // 32 hex chars
            Array.Reverse(hex);                   // reverse nibble order

            var reversedNibbles = string.Join('.', hex);
            return $"{reversedNibbles}.{_zone}";
        }


        private static SpamhausDatabase MapReturnCode(IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                return SpamhausDatabase.Other;
            }

            var addressBytes = ipAddress.GetAddressBytes();

            // Check for 127.0.0.X pattern
            if (addressBytes[0] != 127 || addressBytes[1] != 0 || addressBytes[2] != 0)
            { 
                return SpamhausDatabase.Other;
            }

            return addressBytes[3] switch
            {
                2 or 3    => SpamhausDatabase.SBL,
                4         => SpamhausDatabase.XBL,
                9         => SpamhausDatabase.DROP,
                10 or 11  => SpamhausDatabase.PBL,
                _         => SpamhausDatabase.Other
            };
        }

        private static bool TryParseIpAddress(string address, out IPAddress ipAddress)
        {
            if (!IPAddress.TryParse(address, out ipAddress))
            {
                return false;
            }

            // Handle IPv4-mapped IPv6 (e.g. ::ffff:1.2.3.4)
            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6 && ipAddress.IsIPv4MappedToIPv6)
            {
                ipAddress = ipAddress.MapToIPv4();
            }

            // We support only IPv4 or IPv6 
            return ipAddress.AddressFamily == AddressFamily.InterNetwork
                || ipAddress.AddressFamily == AddressFamily.InterNetworkV6;
        }
    }
}