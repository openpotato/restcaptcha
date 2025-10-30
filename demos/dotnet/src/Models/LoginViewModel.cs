#region RESTCaptcha ASP.NET Core MVC Example
/*    
 *    RESTCaptcha ASP.NET Core MVC Example
 */
#endregion

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CaptchaSample.Models
{
    /// <summary>
    /// View model used by the login form. 
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// The CAPTCHA solution injected to the form
        /// </summary>
        [Required]
        [FromForm(Name = "captcha-solution")]
        public string CaptchaSolution { get; set; }

        /// <summary>
        /// The CAPTCHA token injected to the form
        /// </summary>
        [Required]
        [FromForm(Name = "captcha-token")]
        public string CaptchaToken { get; set; }

        /// <summary>
        /// The password entered by the user
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The username entered by the user
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;
    }
}