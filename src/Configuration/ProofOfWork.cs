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

using System.Text.Json.Serialization;

namespace RestCaptcha
{
    /// <summary>
    /// A proof-of-work challenge type
    /// </summary>
    public class ProofOfWork : ChallengeType
    {
        /// <summary>
        /// Used hash algorithm
        /// </summary>
        [JsonPropertyOrder(1)]
        public ProofOfWorkAlgorithm Algorithm { get; set; } = ProofOfWorkAlgorithm.SHA256;

        /// <summary>
        /// Difficulty 
        /// </summary>
        [JsonPropertyOrder(2)]
        public int Difficulty { get; set; } = 4;

        /// <summary>
        /// Compact string representation of the challenge type
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{TypeConsts.ProofOfWork}:{Algorithm}:{Difficulty}";
        }
    }
}

