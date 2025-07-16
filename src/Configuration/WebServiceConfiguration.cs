#region RestCaptcha - Copyright (C) STÜBER SYSTEMS GmbH
/*    
 *    RestCaptcha
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
    /// RestCaptcha Web Service configuration
    /// </summary>
    public class WebServiceConfiguration
    {
        /// <summary>
        /// TTL for fingerprint cache
        /// </summary>
        public TimeSpan FingerprintTTL { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Secret key for HMAC (Hash-based Message Authentication Code)
        /// </summary>
        public string HMACKey { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Proof-of-work difficulty
        /// </summary>
        public int ProofOfWorkDifficulty { get; set; } = 4;

        /// <summary>
        /// Maximun time between receiving the challenge and checking the solution
        /// </summary>
        public TimeSpan ChallengeResponseMaxDuration { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Minimum time between receiving the challenge and checking the solution
        /// </summary>
        public TimeSpan ChallengeResponseMinDuration { get; set; } = TimeSpan.FromSeconds(2);
    }
}
