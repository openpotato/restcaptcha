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
    /// An implmentation of <see cref="IAppMetadata"/>
    /// </summary>
    public sealed class AppMetadata : IAppMetadata
    {
        /// <summary>
        /// Short name
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// The current version
        /// </summary>
        public required string Version { get; init; }

        /// <summary>
        /// A unqiue service id
        /// </summary>
        public required string ServiceId { get; init; }

        /// <summary>
        /// Start time of the service
        /// </summary>
        public DateTimeOffset StartedAt { get; init; }
    }
}
