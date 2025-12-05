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
    /// Deserializer for polymorphic <see cref="ChallengeType"/> type objects
    /// </summary>
    public static class ChallengeTypeDeserializer
    {
        public static ChallengeType CreateChallengeTypeObject(this Dictionary<string, object> dict)
        {
            if (dict.TryGetValue(PropertyNames.Type, out var typeValue))
            {
                if (typeValue is string typeStrValue)
                {
                    return typeStrValue.Trim() switch
                    {
                        TypeConsts.ProofOfWork => CreateProofOfWorkObject(dict),
                        _ => throw new ArgumentException($"Invalid status value: {typeStrValue}"),
                    };
                }
                else
                {
                    throw new ArgumentException($"Invalid status value: {typeValue}");
                }
            }
            else
            {
                throw new Exception("Error");
            }
        }

        private static ProofOfWork CreateProofOfWorkObject(Dictionary<string, object> dict)
        {
            var result = new ProofOfWork();

            if (dict.TryGetValue(PropertyNames.Algorithm, out var algorithmValue))
            {
                if (algorithmValue is string algorithmStrValue)
                {
                    result.Algorithm = algorithmStrValue.Trim() switch
                    {
                        "hash-sha-256" => ProofOfWorkAlgorithm.SHA256,
                        "hash-sha-384" => ProofOfWorkAlgorithm.SHA384,
                        "hash-sha-512" => ProofOfWorkAlgorithm.SHA512,
                        _ => throw new ArgumentException($"Invalid status value: {algorithmValue}"),
                    };
                }
                else 
                {
                    throw new ArgumentException($"Invalid status value: {algorithmValue}");
                }
            }

            if (dict.TryGetValue(PropertyNames.Difficulty, out var difficultyValue))
            {
                if (difficultyValue is string difficultyStrValue)
                {
                    result.Difficulty = int.Parse(difficultyStrValue);
                }
                else
                {
                    throw new ArgumentException($"Invalid status value: {difficultyValue}");
                }
            }

            return result;
        }
    }
}
