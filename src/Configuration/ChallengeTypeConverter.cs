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
using System.Text.Json.Serialization;

namespace RestCaptcha
{
    /// <summary>
    /// JSON Converter for <see cref="ChallengeType"/> instances
    /// </summary>
    public class ChallengeTypeConverter : JsonConverter<ChallengeType>
    {
        /// <summary>
        /// Read and convert the JSON to <see cref="ChallengeType"/>.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">The <see cref="System.Type"/> being converted.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
        /// <returns>The value that was converted.</returns>
        public override ChallengeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument jsonDoc = JsonDocument.ParseValue(ref reader);
            
            if (jsonDoc.RootElement.TryGetProperty(PropertyNames.Type, out JsonElement typeElement))
            {
                var elementType = typeElement.GetString();

                ChallengeType result = elementType switch
                {
                    TypeConsts.ProofOfWork => JsonSerializer.Deserialize<ProofOfWork>(jsonDoc.RootElement.GetRawText(), options),
                    _ => throw new JsonException($"Unknown challenge type: {elementType}")
                };

                return result;
            }
            else
            {
                throw new JsonException("Missing type discriminator in challenge type.");
            }
        }

        /// <summary>
        /// Write the value as JSON.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The value to convert.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
        public override void Write(Utf8JsonWriter writer, ChallengeType value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
}
