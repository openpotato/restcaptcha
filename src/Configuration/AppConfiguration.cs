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

namespace RestCaptcha
{
    /// <summary>
    /// RESTCaptcha configuration
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Challenge type
        /// </summary>
        public ChallengeType ChallengeType { get; set; } = new ProofOfWork(ProofOfWorkAlgorithm.SHA256, 4);

        /// <summary>
        /// Health check configuration
        /// </summary>
        public HealthCheckConfiguration HealthCheck { get; set; } = new();

        /// <summary>
        /// Secret key for HMAC (Hash-based Message Authentication Code)
        /// </summary>
        /// <remarks>
        /// The generated default value is not cryptographically secure, please use a
        /// Random Password Generator like https://1password.com/password-generator
        /// </remarks>
        public string HMACKey { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Maximun time-to-live of the unique nonce
        /// </summary>
        public TimeSpan NonceMaxTTL { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// List of registered sites
        /// </summary>
        public List<Site> Sites { get; set; } = [];

        /// <summary>
        /// Minimum delay between receiving the challenge and server-side verification
        /// </summary>
        public TimeSpan VerificationMinDelay { get; set; } = TimeSpan.FromSeconds(2);
    }
}
