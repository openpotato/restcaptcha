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
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Reflection;
using Xunit;

namespace RestCaptcha.Tests
{
    public class SpamhausClientTests
    {
        [Fact]
        public async Task CheckAsync_CallsCheckWithoutCacheAndCachesResult_WhenNotInCache_InvalidIp()
        {
            // Prepare
            var cache = CreateMemoryCache();
            var client = new SpamhausClient(CreateDnsClient(), cache);

            var ip = "not.a.valid.ip";
            var cacheKey = $"spamhaus:{ip}";

            // Act
            var result = await client.CheckAsync(ip);

            // Assert: result is "base result" with Database = None
            Assert.NotNull(result);
            Assert.Equal(ip, result.IpAddress);
            Assert.Equal(SpamhausDatabase.NotListed, result.Database);
            Assert.Null(result.RawReturnAddress);

            // Assert: result is cached
            var cached = cache.TryGetValue(cacheKey, out SpamhausCheckResult cachedResult);
            Assert.True(cached);
            Assert.Same(result, cachedResult);
        }


        [Fact]
        public async Task CheckAsync_UsesCache_WhenResultIsCached()
        {
            // Prepare
            var cache = CreateMemoryCache();
            var client = new SpamhausClient(CreateDnsClient(), cache);

            var ip = "1.2.3.4";
            var cacheKey = $"spamhaus:{ip}";

            var cachedResult = new SpamhausCheckResult
            {
                IpAddress = ip,
                Database = SpamhausDatabase.SBL,
                RawReturnAddress = "127.0.0.2"
            };

            cache.Set(cacheKey, cachedResult);

            // Act
            var result = await client.CheckAsync(ip);

            // Assert
            Assert.Same(cachedResult, result);
        }

        [Fact]
        public async Task CheckWithoutCacheAsync_InvalidIp_ReturnsNoneBaseResult()
        {
            // Prepare
            var cache = CreateMemoryCache();
            var client = new SpamhausClient(CreateDnsClient(), cache);
            var ip = "this-is-not-an-ip";

            // Act
            var result = await client.CheckWithoutCacheAsync(ip);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ip, result.IpAddress);
            Assert.Equal(SpamhausDatabase.NotListed, result.Database);
            Assert.Null(result.RawReturnAddress);
        }

        [Fact]
        public async Task CheckWithoutCacheAsync_Ipv6_ReturnsNoneBaseResult()
        {
            // Prepare
            var cache = CreateMemoryCache();
            var client = new SpamhausClient(CreateDnsClient(), cache);
            var ip = "2001:db8::1"; // Valid IPv6, but we only support IPv4

            // Act
            var result = await client.CheckWithoutCacheAsync(ip);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ip, result.IpAddress);
            Assert.Equal(SpamhausDatabase.NotListed, result.Database);
            Assert.Null(result.RawReturnAddress);
        }

        [Fact]
        public void CreateHostQuery_BuildsCorrectReversedHost()
        {
            // Prepare
            var method = typeof(SpamhausClient)
                .GetMethod("CreateHostQuery", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            var ip = IPAddress.Parse("1.2.3.4");

            // Act
            var result = (string)method!.Invoke(null, [ip]);

            // Assert
            Assert.Equal("4.3.2.1.zen.spamhaus.org", result);
        }

        [Theory]
        [InlineData("127.0.0.2", SpamhausDatabase.SBL)]
        [InlineData("127.0.0.3", SpamhausDatabase.SBL)]
        [InlineData("127.0.0.4", SpamhausDatabase.XBL)]
        [InlineData("127.0.0.9", SpamhausDatabase.DROP)]
        [InlineData("127.0.0.10", SpamhausDatabase.PBL)]
        [InlineData("127.0.0.11", SpamhausDatabase.PBL)]
        [InlineData("127.0.0.255", SpamhausDatabase.Other)]
        [InlineData("127.0.1.2", SpamhausDatabase.Other)]
        [InlineData("10.0.0.1", SpamhausDatabase.Other)]
        public void MapReturnCode_ReturnsExpected_ForIPv4(string ip, SpamhausDatabase expected)
        {
            // Prepare
            var method = typeof(SpamhausClient)
                .GetMethod("MapReturnCode", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            var addr = IPAddress.Parse(ip);

            // Act
            var result = (SpamhausDatabase)method!.Invoke(null, new object[] { addr })!;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MapReturnCode_ReturnsOther_ForNonIPv4()
        {
            // Prepare
            var method = typeof(SpamhausClient)
                .GetMethod("MapReturnCode", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            var ipv6 = IPAddress.IPv6Loopback;

            // Act
            var result = (SpamhausDatabase)method!.Invoke(null, new object[] { ipv6 })!;

            // Assert
            Assert.Equal(SpamhausDatabase.Other, result);
        }

        private static DnsClient CreateDnsClient()
        {
            return new(NullLogger<DnsClient>.Instance);
        }

        private static MemoryCache CreateMemoryCache()
        {
            return new(new MemoryCacheOptions());
        }
    }
}
