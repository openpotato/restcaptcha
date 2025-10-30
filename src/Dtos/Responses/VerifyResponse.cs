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

using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace RestCaptcha
{
    /// <summary>
    /// Representation of a verify response
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public class VerifyResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyResponse"/> class.
        /// </summary>
        /// <param name="status">An unique, random value</param>
        public VerifyResponse(VerifyStatus status)
            : this(status, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyResponse"/> class.
        /// </summary>
        /// <param name="status">An unique, random value</param>
        /// <param name="hostName">An unique, random value</param>
        public VerifyResponse(VerifyStatus status, string hostName)
        {
            Status = status;
            HostName = hostName;
        }

        /// <summary>
        /// Host names bound to this response
        /// </summary>
        [JsonPropertyOrder(2)]
        public string HostName { get; set; }

        /// <summary>
        /// Verification status
        /// </summary>
        [JsonPropertyOrder(1)]
        public VerifyStatus Status { get; set; }

        /// <summary>
        /// Compact string representation of the instance
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"\"{Status}\"";
        }
    }
}
