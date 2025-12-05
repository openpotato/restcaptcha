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
using System.Text;
using System.Text.Json;
using Xunit;

namespace RestCaptcha.Tests
{
    public class AbuseIpDbClientTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public AbuseIpDbClientTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CheckAsync_CallsApiAndCachesResult_WhenNotInCache()
        {
            // Prepare
            var cache = CreateMemoryCache();
            var configuration = CreateConfiguration(_fixture.AbuseIpDbApiKey);

            var ip = "5.6.7.8";
            var cacheKey = $"abuseipdb:{ip}";

            HttpRequestMessage capturedRequest = null;

            var expectedResult = new AbuseIpDbCheckResult
            {
                AbuseConfidenceScore = 42,
                CountryCode = "DE",
                DistinctUserCount = 3,
                Domain = "example.com",
                Hostnames = ["host1.example.com", "host2.example.com"],
                IpAddress = ip,
                IPVersion = 4,
                ISP = "Example ISP",
                IsPublic = true,
                IsTor = false,
                IsWhitelisted = false,
                LastReportedAt = DateTimeOffset.Parse("2025-01-01T12:34:56Z"),
                TotalReports = 10,
                UsageType = "Data Center"
            };

            var responseJson = JsonSerializer.Serialize(new
            {
                data = expectedResult
            });

            var handler = new DelegateHttpMessageHandler((request, token) =>
            {
                capturedRequest = request;

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                };

                return Task.FromResult(response);
            });

            var httpClient = CreateHttpClient(handler);

            var client = new AbuseIpDbClient(httpClient, cache, configuration);

            // Act
            var result = await client.CheckAsync(ip);

            // Assert: result content
            Assert.NotNull(result);
            Assert.Equal(expectedResult.AbuseConfidenceScore, result.AbuseConfidenceScore);
            Assert.Equal(expectedResult.CountryCode, result.CountryCode);
            Assert.Equal(expectedResult.DistinctUserCount, result.DistinctUserCount);
            Assert.Equal(expectedResult.Domain, result.Domain);
            Assert.Equal(expectedResult.Hostnames, result.Hostnames);
            Assert.Equal(expectedResult.IpAddress, result.IpAddress);
            Assert.Equal(expectedResult.IPVersion, result.IPVersion);
            Assert.Equal(expectedResult.ISP, result.ISP);
            Assert.Equal(expectedResult.IsPublic, result.IsPublic);
            Assert.Equal(expectedResult.IsTor, result.IsTor);
            Assert.Equal(expectedResult.IsWhitelisted, result.IsWhitelisted);
            Assert.Equal(expectedResult.TotalReports, result.TotalReports);
            Assert.Equal(expectedResult.UsageType, result.UsageType);

            // Assert: request built correctly
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Get, capturedRequest!.Method);
            Assert.Equal($"https://api.abuseipdb.com/api/v2/check?ipAddress={Uri.EscapeDataString(ip)}&maxAgeInDays=90", capturedRequest.RequestUri!.ToString());

            // Accept header contains application/json
            Assert.Contains(capturedRequest.Headers.Accept, h => h.MediaType != null && h.MediaType.Contains("json", StringComparison.OrdinalIgnoreCase));

            // API key header
            Assert.True(capturedRequest.Headers.Contains("Key"));
            Assert.Equal(_fixture.AbuseIpDbApiKey, string.Join(",", capturedRequest.Headers.GetValues("Key")));

            // Assert: result is cached
            var cached = cache.TryGetValue(cacheKey, out AbuseIpDbCheckResult cachedResult);
            Assert.True(cached);
            Assert.NotNull(cachedResult);
            Assert.Equal(expectedResult.AbuseConfidenceScore, cachedResult!.AbuseConfidenceScore);
        }

        [Fact]
        public async Task CheckAsync_PassesCancellationTokenToHttpClient()
        {
            // Prepare
            var cache = CreateMemoryCache();
            var configuration = CreateConfiguration(_fixture.AbuseIpDbApiKey);

            var ip = "10.0.0.1";
            var cts = new CancellationTokenSource();

            // This handler will delay indefinitely until cancellation is triggered
            var handler = new DelegateHttpMessageHandler(async (request, token) =>
            {
                // Delay long enough that we rely on the cancellation signal
                await Task.Delay(Timeout.InfiniteTimeSpan, token);

                // Should never reach here
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var httpClient = CreateHttpClient(handler);
            var client = new AbuseIpDbClient(httpClient, cache, configuration);

            // Act
            var task = client.CheckAsync(ip, cts.Token);

            // Trigger cancellation
            cts.Cancel();

            // Assert — the inner SendAsync must throw TaskCanceledException
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
        }

        [Fact]
        public async Task CheckAsync_Throws_WhenResponseIsNotSuccess()
        {
            // Prepare
            var cache = CreateMemoryCache();
            var configuration = CreateConfiguration(_fixture.AbuseIpDbApiKey);

            var ip = "9.9.9.9";

            var handler = new DelegateHttpMessageHandler((request, token) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.TooManyRequests));
            });

            var httpClient = CreateHttpClient(handler);

            var client = new AbuseIpDbClient(httpClient, cache, configuration);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => client.CheckAsync(ip));
        }

        [Fact]
        public async Task CheckAsync_UsesCache_WhenResultIsCached()
        {
            // Arrange
            var cache = CreateMemoryCache();
            var configuration = CreateConfiguration(_fixture.AbuseIpDbApiKey);

            var ip = "1.2.3.4";
            var cacheKey = $"abuseipdb:{ip}";

            var cachedResult = new AbuseIpDbCheckResult
            {
                IpAddress = ip,
                AbuseConfidenceScore = 99
            };

            cache.Set(cacheKey, cachedResult);

            // handler throws if used – to ensure HTTP is not called
            var handler = new ThrowingHttpMessageHandler();
            var httpClient = CreateHttpClient(handler);

            var client = new AbuseIpDbClient(httpClient, cache, configuration);

            // Act
            var result = await client.CheckAsync(ip);

            // Assert
            Assert.Same(cachedResult, result);
        }

        private static HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return new(handler, disposeHandler: false);
        }

        private static MemoryCache CreateMemoryCache()
        { 
            return new(new MemoryCacheOptions());
        }

        private static AbuseIPDBConfiguration CreateConfiguration(string apiKey)
        {
            return new()
            {
                ApiKey = apiKey
            };
        }
    }

    /// <summary>
    /// A HttpMessageHandler that delegates to a provided function.
    /// </summary>
    internal sealed class DelegateHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

        public DelegateHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        { 
            return _handler(request, cancellationToken);
        }
    }

    /// <summary>
    /// A HttpMessageHandler that always throws when called. Used to ensure no HTTP calls happen (e.g. when cache is used).
    /// </summary>
    internal sealed class ThrowingHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("HTTP call should not have been made.");
        }
    }
}
