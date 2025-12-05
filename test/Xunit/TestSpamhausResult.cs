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

using Xunit;

namespace RestCaptcha.Tests
{
    public class SpamhausResultTests
    {
        [Fact]
        public void SpamhausResult_Defaults_AndProperties_Work()
        {
            // Prepare
            var result = new SpamhausCheckResult();

            // Assert defaults
            Assert.Equal(SpamhausDatabase.NotListed, result.Database);
            Assert.Equal(string.Empty, result.IpAddress);
            Assert.Null(result.RawReturnAddress);

            // Set and re-check
            result.Database = SpamhausDatabase.SBL;
            result.IpAddress = "1.2.3.4";
            result.RawReturnAddress = "127.0.0.2";

            Assert.Equal(SpamhausDatabase.SBL, result.Database);
            Assert.Equal("1.2.3.4", result.IpAddress);
            Assert.Equal("127.0.0.2", result.RawReturnAddress);
        }
    }
}
