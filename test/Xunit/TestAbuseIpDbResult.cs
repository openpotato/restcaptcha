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

using System.Text.Json;
using Xunit;

namespace RestCaptcha.Tests
{
    public class AbuseIpDbCheckResultTests
    {
        [Fact]
        public void JsonSerialization_BindsToExpectedProperties()
        {
            // Prepare
            var json = @"
            {
                ""abuseConfidenceScore"": 15,
                ""countryCode"": ""US"",
                ""numDistinctUsers"": 7,
                ""domain"": ""example.org"",
                ""hostnames"": [""h1.example.org"", ""h2.example.org""],
                ""ipAddress"": ""123.45.67.89"",
                ""ipVersion"": 4,
                ""isp"": ""Some ISP"",
                ""isPublic"": true,
                ""isTor"": true,
                ""isWhitelisted"": false,
                ""lastReportedAt"": ""2025-02-02T12:00:00Z"",
                ""totalReports"": 25,
                ""usageType"": ""Hosting""
            }";

            // Act
            var result = JsonSerializer.Deserialize<AbuseIpDbCheckResult>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result!.AbuseConfidenceScore);
            Assert.Equal("US", result.CountryCode);
            Assert.Equal(7, result.DistinctUserCount);
            Assert.Equal("example.org", result.Domain);
            Assert.Equal(new[] { "h1.example.org", "h2.example.org" }, result.Hostnames);
            Assert.Equal("123.45.67.89", result.IpAddress);
            Assert.Equal(4, result.IPVersion);
            Assert.Equal("Some ISP", result.ISP);
            Assert.True(result.IsPublic);
            Assert.True(result.IsTor);
            Assert.False(result.IsWhitelisted);
            Assert.Equal(25, result.TotalReports);
            Assert.Equal("Hosting", result.UsageType);
            Assert.True(result.LastReportedAt.HasValue);
        }
    }
}
