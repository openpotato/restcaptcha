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
    /// Representation of a RESTCaptcha token
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        public Token(string nonce, string nonceSignature)
        {
            Nonce = nonce;
            NonceSignature = nonceSignature;
        }

        /// <summary>
        /// The original nonce from the challenge request
        /// </summary>
        public string Nonce { get; }

        /// <summary>
        /// The original nonce from the challenge request
        /// </summary>
        public string NonceSignature { get; }

        /// <summary>
        /// Parses a string value into a new <see cref="Token"/> instance.
        /// </summary>
        /// <param name="value">The string value</param>
        /// <returns>A new <see cref="Token"/> instance</returns>
        public static Token FromValue(string value)
        {
            var valueParts = value.Split([':'], 2);

            return new Token(
                valueParts.Length > 0 ? valueParts[0] : string.Empty,
                valueParts.Length > 1 ? valueParts[1] : string.Empty
            );
        }

        /// <summary>
        /// String representation of this instance.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{Nonce}:{NonceSignature}";
        }
    }
}
