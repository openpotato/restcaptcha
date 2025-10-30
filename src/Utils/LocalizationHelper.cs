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

using System.Globalization;

namespace RestCaptcha
{
    /// <summary>
    /// Static localization helper class
    /// </summary>
    public static class LocalizationHelper
    {
        private const string _defaultCultureName = "en";

        readonly private static CultureInfo[] _supportedCultures = [
            new CultureInfo(_defaultCultureName),
            new CultureInfo("de"),
            new CultureInfo("es"),
            new CultureInfo("fr"),
            new CultureInfo("it"),
            new CultureInfo("pt")
        ];

        public static string GetDefaultCultureName()
        {
            return _defaultCultureName;
        }

        public static CultureInfo[] GetSupportedCultures()
        {
            return _supportedCultures;
        }
    }
}
