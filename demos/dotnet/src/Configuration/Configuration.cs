#region RESTCaptcha ASP.NET Core MVC Example
/*    
 *    RESTCaptcha ASP.NET Core MVC Example
 */
#endregion

namespace RestCaptcha
{
    /// <summary>
    /// RESTCaptcha Sample Configuration
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// External link for 'About' 
        /// </summary>
        public ExternalLink AboutLink { get; set; }

        /// <summary>
        /// Base URL of the RESTCaptcha API endpoint
        /// </summary>
        public Uri ApiBaseUrl { get; set; }

        /// <summary>
        /// URL of the RESTCaptcha JavaScript client library
        /// </summary>
        public Uri JsClientUrl { get; set; }

        /// <summary>
        /// External link for 'Privacy' 
        /// </summary>
        public ExternalLink PrivacyLink { get; set; }

        /// <summary>
        /// Public site key used to identify this application to the RESTCaptcha service
        /// </summary>
        public string SiteKey { get; set; }

        /// <summary>
        /// Private site secret used to authorise verification requests to the RESTCaptcha service
        /// </summary>
        public string SiteSecret { get; set; }
    }
}
