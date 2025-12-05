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
    /// List of possible block lists managed by Spamhaus
    /// </summary>
    public enum SpamhausDatabase
    {
        /// <summary>
        /// Not listed
        /// </summary>
        NotListed = 0,

        /// <summary>
        /// Spamhaus Block List
        /// </summary>
        SBL,

        /// <summary>
        /// Exploits Block List
        /// </summary>
        XBL,

        /// <summary>
        /// Don’t Route Or Peer
        /// </summary>
        DROP,

        /// <summary>
        /// Policy Block List
        /// </summary>
        PBL,  

        /// <summary>
        /// Unknown 
        /// </summary>
        Other
    }
}