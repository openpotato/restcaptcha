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
    /// A specific sub-component/dependency of a service
    /// </summary>
    public sealed class HealthCheckComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckComponent"/> class.
        /// </summary>
        /// <param name="measurementName">A measurement name</param>
        public HealthCheckComponent(HealthCheckMeasurementName measurementName)
            : this(null, measurementName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckComponent"/> class.
        /// </summary>
        public HealthCheckComponent(string componentName, HealthCheckMeasurementName measurementName)
        {
            ComponentName = componentName;
            MeasurementName = measurementName;
        }

        /// <summary>
        /// A human-readable name for the component. MUST not contain a colon, in the name, 
        /// since colon is used as a separator.
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// Name of a measurement type (a data point type) that the status is reported for.
        /// </summary>
        public HealthCheckMeasurementName MeasurementName { get; }

        /// <summary>
        /// List of component instance details
        /// </summary>
        public IList<HealthCheckComponentInstance> Instances { get; set; } = [];
    }
}
