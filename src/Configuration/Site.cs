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
    /// RESTCaptcha site registration
    /// </summary>
    public class Site
    {
        /// <summary>
        /// Additional human-readable description of the site
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Human-readable name of the site
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unique public site key
        /// </summary>
        public string SiteKey { get; set; }

        /// <summary>
        /// Site secret
        /// </summary>
        public string SiteSecret { get; set; }

        /// <summary>
        /// List of valid domain names bound to this site
        /// </summary>
        public string[] ValidHostNames { get; set; }
    }
}
