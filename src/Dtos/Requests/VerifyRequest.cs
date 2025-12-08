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

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestCaptcha
{
    /// <summary>
    /// Representation of a verify request
    /// </summary>
    public class VerifyRequest
    {
        /// <summary>
        /// The IP address of the client on which the solution was calculated
        /// </summary>
        [JsonPropertyOrder(4)]
        public string CallerIp { get; set; }

        /// <summary>
        /// Site secret
        /// </summary>
        [Required(ErrorMessage = "missingSiteSecret")]
        [JsonPropertyOrder(1)]
        public string SiteSecret { get; set; }

        /// <summary>
        /// The challenge solution to be verified
        /// </summary>
        [Required(ErrorMessage = "missingSolution")]
        [JsonPropertyOrder(3)]
        public string Solution { get; set; }

        /// <summary>
        /// The challenge token
        /// </summary>
        [Required(ErrorMessage = "missingToken")]
        [JsonPropertyOrder(2)]
        public string Token { get; set; }

        /// <summary>
        /// Compact string representation of the instance
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(CallerIp))
            {
                return $"\"{Token}\" + \"{Solution}\" + \"{SiteSecret}\"";
            }
            else
            {
                return $"\"{Token}\" + \"{Solution}\" + \"{SiteSecret}\" + \"{CallerIp}\"";
            }
        }
    }
}
