#region RESTCaptcha ASP.NET Core MVC Example
/*    
 *    RESTCaptcha ASP.NET Core MVC Example
 */
#endregion

using CaptchaSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestCaptcha;
using RestCaptcha.Client;

namespace CaptchaSample.Controllers
{
    /// <summary>
    /// Controller demonstrating RESTCaptcha integration in an ASP.NET Core MVC application.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly Configuration _configuration;

        /// <summary>
        /// Initialises a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration options.</param>
        public HomeController(IOptions<Configuration> configuration)
            : base()
        {
            _configuration = configuration.Value;
        }

        /// <summary>
        /// Displays a simple echo page showing the entered username, password and verification status.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="returnUrl">The verification status</param>
        /// <returns>The created <see cref="ViewResult"/> object for the response.</returns>
        [HttpGet]
        public IActionResult Echo(string username, string password, string returnUrl)
        {
            var model = new EchoViewModel { Username = username, Password = password, ReturnUrl = returnUrl };
            return View(model);
        }

        /// <summary>
        /// Displays the default login form with RESTCaptcha protection.
        /// </summary>
        /// <returns>The created <see cref="ViewResult"/> object for the response.</returns>
        [HttpGet]
        [ActionName("index")]
        public IActionResult Index()
        {
            ViewData["ApiBaseUrl"] = _configuration.ApiBaseUrl.ToString();
            ViewData["SiteKey"] = _configuration.SiteKey;
            ViewData["JsClientUrl"] = _configuration.JsClientUrl.ToString();
            ViewData["AboutLink"] = _configuration.AboutLink;
            ViewData["PrivacyLink"] = _configuration.PrivacyLink;

            return View(nameof(Index), new LoginViewModel());
        }

        /// <summary>
        /// Displays a login form using the "auto" integration mode.
        /// </summary>
        /// <returns>The created <see cref="ViewResult"/> object for the response.</returns>
        [HttpGet]
        [ActionName("index-auto")]
        public IActionResult IndexAuto()
        {
            ViewData["ApiBaseUrl"] = _configuration.ApiBaseUrl.ToString();
            ViewData["SiteKey"] = _configuration.SiteKey;
            ViewData["JsClientUrl"] = _configuration.JsClientUrl.ToString();
            ViewData["AboutLink"] = _configuration.AboutLink;
            ViewData["PrivacyLink"] = _configuration.PrivacyLink;

            return View(nameof(IndexAuto), new LoginViewModel());
        }

        /// <summary>
        /// Displays a login form using the "headless" integration mode.
        /// </summary>
        /// <returns>The created <see cref="ViewResult"/> object for the response.</returns>
        [HttpGet]
        [ActionName("index-headless")]
        public IActionResult IndexHeadless()
        {
            ViewData["ApiBaseUrl"] = _configuration.ApiBaseUrl.ToString();
            ViewData["SiteKey"] = _configuration.SiteKey;
            ViewData["JsClientUrl"] = _configuration.JsClientUrl.ToString();

            return View(nameof(IndexHeadless), new LoginViewModel());
        }

        /// <summary>
        /// Displays a login form using the "invisible" integration mode.
        /// </summary>
        /// <returns>The created <see cref="ViewResult"/> object for the response.</returns>
        [HttpGet]
        [ActionName("index-invisible")]
        public IActionResult IndexInvisible()
        {
            ViewData["ApiBaseUrl"] = _configuration.ApiBaseUrl.ToString();
            ViewData["SiteKey"] = _configuration.SiteKey;
            ViewData["JsClientUrl"] = _configuration.JsClientUrl.ToString();
            ViewData["AboutLink"] = _configuration.AboutLink;
            ViewData["PrivacyLink"] = _configuration.PrivacyLink;

            return View(nameof(IndexInvisible), new LoginViewModel());
        }

        /// <summary>
        /// Processes a login request by verifying the RESTCaptcha solution and redirecting
        /// to the echo page with the submitted credentials and verification status.
        /// </summary>
        /// <param name="model">Contains fields submitted by the web client</param>
        /// <returns>Redirects to the <see cref="Echo"/> action if verification succeeds,
        /// passing along the entered credentials and verification status. </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var apiClient = new ApiClient(_configuration.ApiBaseUrl, _configuration.SiteKey, _configuration.SiteSecret, "en");

            var status = await apiClient.VerifySolutionAsync(model.CaptchaToken, model.CaptchaSolution);

            var returnUrl = Request.Headers.Referer.ToString();

            return RedirectToAction(nameof(Echo), new { username = model.Username, password = model.Password, status, returnUrl });
        }
    }
}