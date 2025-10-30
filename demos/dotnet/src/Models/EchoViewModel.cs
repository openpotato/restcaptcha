#region RESTCaptcha ASP.NET Core MVC Example
/*    
 *    RESTCaptcha ASP.NET Core MVC Example
 */
#endregion

using RestCaptcha.Client;
using System.ComponentModel.DataAnnotations;

namespace CaptchaSample.Models
{
    /// <summary>
    /// View model used by the Echo page.
    /// </summary>
    public class EchoViewModel
    {
        /// <summary>
        /// The password entered by the user
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The return URL for the back button on the Echo page
        /// </summary>
        [Required]
        public string ReturnUrl { get; set; } = string.Empty;

        /// <summary>
        /// The verification status returned by the RESTCaptcha API
        /// </summary>
        [Required]
        public VerifyStatus Status { get; set; }

        /// <summary>
        /// The username entered by the user
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;
    }
}