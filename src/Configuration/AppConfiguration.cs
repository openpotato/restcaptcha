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

namespace RestCaptcha
{
    /// <summary>
    /// RESTCaptcha configuration
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Map of behaviors by score ranges
        /// </summary>
        public List<Behavior> BehaviorMap { get; set; } = [
            new Behavior() { 
                MinRiskScore = 0, 
                MaxRiskScore = 75, 
                Action = ActionType.Challenge, 
                ChallengeType = new Dictionary<string, object>
                {
                    [PropertyNames.Type] = TypeConsts.ProofOfWork,
                    [PropertyNames.Algorithm] = TypeConsts.SHA256,
                    [PropertyNames.Difficulty] = 4
                } },
            new Behavior() {
                MinRiskScore = 75,
                MaxRiskScore = 100,
                Action = ActionType.Block }
        ];

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
        /// Check for abusive IPs
        /// </summary>
        public IpReputationCheckConfiguration IpReputationCheck { get; set; } = new();

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
