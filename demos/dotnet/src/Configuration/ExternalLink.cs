#region RESTCaptcha ASP.NET Core MVC Example
/*    
 *    RESTCaptcha ASP.NET Core MVC Example
 */
#endregion

using System.Text.Json.Serialization;

namespace RestCaptcha
{
    /// <summary>
    /// External link configuration
    /// </summary>
    public class ExternalLink
    {
        /// <summary>
        /// href-attribute of the link
        /// </summary>
        [JsonPropertyName("href")]
        public Uri HRef { get; set; }

        /// <summary>
        /// text value of the link
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
