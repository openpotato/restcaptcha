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

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestCaptcha
{
    /// <summary>
    /// Body data of a verify request
    /// </summary>
    public class VerifyRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyRequest"/> class.
        /// </summary>
        public VerifyRequest()
        {
        }

        /// <summary>
        /// Device/browser fingerprint for trust scoring
        /// </summary>
        [Required]
        [JsonPropertyOrder(5)]
        public string Fingerprint { get; set; }

        /// <summary>
        /// The original nonce from the challenge request
        /// </summary>
        [Required]
        [JsonPropertyOrder(1)]
        public string Nonce { get; set; }

        /// <summary>
        /// Signature of the nonce proving authenticity
        /// </summary>
        [Required]
        [JsonPropertyOrder(2)]
        public string NonceSignature { get; set; }

        /// <summary>
        /// The client’s proof-of-work solution
        /// </summary>
        [Required]
        [JsonPropertyOrder(3)]
        public string Solution { get; set; }

        /// <summary>
        /// Timestamp to check freshness
        /// </summary>
        [Required]
        [JsonPropertyOrder(3)]
        public long TimeStamp { get; set; }
    }
}
