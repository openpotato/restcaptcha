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

namespace RestCaptcha
{
    /// <summary>
    /// IP reputation check via Spamhaus and/or AbuseDBIP
    /// </summary>
    public sealed class IpReputationCheck
    {
        private readonly AbuseIpDbClient _abuseIpDbClient;
        private readonly IpReputationCheckConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly SpamhausClient _spamhausClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpReputationCheck"/> class.
        /// </summary>
        /// <param name="httpClient">Injected HTTP client</param>
        /// <param name="dnsClient">Injected DNS client</param>
        /// <param name="cache">Injected cache for IP addresses</param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public IpReputationCheck(HttpClient httpClient, IDnsClient dnsClient, IMemoryCache cache, IpReputationCheckConfiguration configuration, ILogger logger) 
        { 
            _configuration = configuration;
            _abuseIpDbClient = new AbuseIpDbClient(httpClient, cache, _configuration.AbuseIPDB);
            _spamhausClient = new SpamhausClient(dnsClient, cache);
            _logger = logger;
        }

        /// <summary>
        /// Checks the reputation for a given IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The value of the TResult parameter 
        /// contains a <see cref="IpReputationCheckResult"/> instance.</returns>
        public async Task<IpReputationCheckResult> CheckAsync(string ipAddress, CancellationToken cancellationToken = default)
        {
            SpamhausCheckResult spamhausCheckResult = null;
            AbuseIpDbCheckResult abuseIpDbCheckResult = null;

            if (_configuration.AbuseIPDB.Enabled)
            {
                abuseIpDbCheckResult = await _abuseIpDbClient.CheckAsync(ipAddress, cancellationToken);

                if (abuseIpDbCheckResult.TotalReports >= 0 && abuseIpDbCheckResult.LastReportedAt is not null)
                {
                    _logger.LogWarning("Found something on AbuseIPDB: {Result}", abuseIpDbCheckResult);
                }
            }

            if (_configuration.Spamhaus.Enabled)
            {
                spamhausCheckResult = await _spamhausClient.CheckAsync(ipAddress, cancellationToken);

                if (spamhausCheckResult.Database != SpamhausDatabase.NotListed)
                {
                    _logger.LogWarning("Found something on Spamhaus: {Result}", spamhausCheckResult);
                }
            }

            var riskScore = CalculateRiskScore(spamhausCheckResult, abuseIpDbCheckResult);

            _logger.LogInformation("IP risk score: {Score}", riskScore);

            return new IpReputationCheckResult() { RiskScore = riskScore };
        }

        private static int CalculateAbuseIpdbScore(AbuseIpDbCheckResult abuseIpDbCheckResult)
        {
            if (abuseIpDbCheckResult is null)
            {
                return 0;
            }

            if (abuseIpDbCheckResult.TotalReports <= 0 || abuseIpDbCheckResult.LastReportedAt is null)
            {
                return 0;
            }

            var days = (DateTimeOffset.Now - abuseIpDbCheckResult.LastReportedAt.Value).TotalDays;

            double recencyFactor =
                days <= 1 ? 1.0 :
                days <= 7 ? 0.8 :
                days <= 30 ? 0.6 :
                days <= 180 ? 0.4 : 0.2;

            var volumeBonus = Math.Min(20, Math.Log10((double)abuseIpDbCheckResult.TotalReports + 1) * 10);
            var score = abuseIpDbCheckResult.AbuseConfidenceScore * recencyFactor + volumeBonus;

            return (int)Math.Min(100, Math.Round(score));
        }

        private static int CalculateRiskScore(SpamhausCheckResult spamhausCheckResult, AbuseIpDbCheckResult abuseIpDbCheckResult)
        {
            var spamhausScore = CalculateSpamhausScore(spamhausCheckResult);
            var abuseIpDbScore = CalculateAbuseIpdbScore(abuseIpDbCheckResult);

            return Math.Max(spamhausScore, abuseIpDbScore);
        }

        private static int CalculateSpamhausScore(SpamhausCheckResult spamhausCheckResult)
        {
            if (spamhausCheckResult is null)
            {
                return 0;
            }

            if (spamhausCheckResult.Database == SpamhausDatabase.SBL || spamhausCheckResult.Database == SpamhausDatabase.XBL)
            {
                return 95;
            }

            if (spamhausCheckResult.Database == SpamhausDatabase.PBL)
            {
                return 70;
            }

            return 0;
        }
    }
}