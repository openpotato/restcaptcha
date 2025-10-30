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
    /// Metadata about the RESTCaptcha web service
    /// </summary>
    public interface IAppMetadata
    {
        /// <summary>
        /// Short name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The current version
        /// </summary>
        string Version { get; }

        /// <summary>
        /// A unqiue service id
        /// </summary>
        string ServiceId { get; }

        /// <summary>
        /// Start time of the service
        /// </summary>
        DateTimeOffset StartedAt { get; }
    }
}
