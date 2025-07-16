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

using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestCaptcha
{
    /// <summary>
    /// Representation of a challenge response
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public class ChallengeResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeResponse"/> class.
        /// </summary>
        /// <param name="nonce">An unique, random value</param>
        /// <param name="nonceSignature">Signature of the nonce proving authenticity</param>
        /// <param name="timeStamp">Current TimeStamp of the server response</param>
        /// <param name="proofOfWorkDifficulty">Difficulty level for solving the challenge</param>
        public ChallengeResponse(string nonce, string nonceSignature, long timeStamp, int proofOfWorkDifficulty)
        {
            TimeStamp = timeStamp;
            Nonce = nonce;
            NonceSignature = nonceSignature;
            ProofOfWorkDifficulty = proofOfWorkDifficulty;
        }

        /// <summary>
        /// An unique, random value
        /// </summary>
        [JsonPropertyOrder(1)]
        public string Nonce { get; set; }

        /// <summary>
        /// Signature of the nonce proving authenticity
        /// </summary>
        [JsonPropertyOrder(2)]
        public string NonceSignature { get; set; }

        /// <summary>
        /// Difficulty level for solving the challenge
        /// </summary>
        [Required]
        [JsonPropertyOrder(4)]
        public int ProofOfWorkDifficulty { get; set; }

        /// <summary>
        /// Current TimeStamp of the server response
        /// </summary>
        [JsonPropertyOrder(3)]
        public long TimeStamp { get; set; }
    }
}
