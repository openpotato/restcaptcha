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

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace RestCaptcha
{
    /// <summary>
    /// RESTCaptcha API controller
    /// </summary>
    [ApiController]
    [ApiVersion(1)]
    [Route("v{v:apiVersion}")]
    [SwaggerTag("RESTCaptcha API endpoints")]
    public class CaptchaController : ControllerBase
    {
        private readonly AppConfiguration _configuration;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<CaptchaController> _logger;
        private readonly IMemoryCache _nonceCache;
        private readonly MemoryCacheEntryOptions _nonceCacheEntryOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaController"/> class.
        /// </summary>
        /// <param name="logger">Injected logger</param>
        /// <param name="localizer">Injected  localizer</param>
        /// <param name="configuration">Injected app configuration</param>
        /// <param name="nonceCache">Injected cahce for generated nonces</param>
        public CaptchaController(
            ILogger<CaptchaController> logger,
            IStringLocalizer<SharedResource> localizer,
            IOptionsMonitor<AppConfiguration> configuration,
            IMemoryCache nonceCache)
            : base()
        {
            _logger = logger;
            _localizer = localizer;
            _configuration = configuration.CurrentValue;
            _nonceCache = nonceCache;
            _nonceCacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(_configuration.NonceMaxTTL);
        }

        /// <summary>
        /// Generates a CAPTCHA challenge for a client.
        /// </summary>
        /// <param name="siteKey">Site key</param>
        /// <returns>A <see cref="ChallengeResponse"/> object</returns>
        [HttpGet("challenge")]
        [ProducesResponseType(typeof(ChallengeResponse), statusCode: StatusCodes.Status200OK, MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemDetails)]
        public IActionResult GetChallenge(
            [FromQuery, Required(ErrorMessage = "missingSiteKey")] string siteKey)
        {
            // Get host name of incoming request
            var originHostName = GetOriginHostName(Request);

            // Check site
            var site = _configuration.Sites.FirstOrDefault(x => x.SiteKey == siteKey);
            if (site == null)
            {
                _logger.LogWarning("Request for {originDomain} not proceesed: Invalid {SiteKey} provided.", originHostName, siteKey);

                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    type: "https://tools.ietf.org/html/rfc9110#section-15.5.5", 
                    title: _localizer["invalidSiteKey"]
                );
            }

            // Check domain name
            if ((site.ValidHostNames != null) && (site.ValidHostNames.Length > 0))
            {
                var normalizedDomainNames = site.ValidHostNames.Select(d => d.ToLowerInvariant()).ToHashSet();

                if (!StringHelper.Match(originHostName, normalizedDomainNames))
                {
                    _logger.LogWarning("Request not verified for domain {domain}: Domain not registered.", originHostName);

                    return Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        type: "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                        title: string.Format(_localizer["invalidDomainName"], originHostName)
                    );
                }
            }

            // Generate challenge response
            var nonce = Guid.NewGuid().ToString("N");
            var nonceTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var nonceSignature = GenerateNonceSignature(nonce, nonceTimestamp, _configuration.HMACKey);
            var token = new Token(nonce, nonceSignature);
            var challengeAlgorithm = _configuration.ChallengeType;
            var responseData = new ChallengeResponse(token.ToString(), challengeAlgorithm);

            // Cache none with timestamp and hostname
            _nonceCache.Set(nonce, Tuple.Create(nonceTimestamp, originHostName), _nonceCacheEntryOptions);

            _logger.LogInformation("Challenge for {originDomain} sucessfully generated: {Response}.", originHostName, responseData);

            return Ok(responseData);
        }

        /// <summary>
        /// Resets a CAPTCHA challenge by invalidating the token
        /// </summary>
        /// <param name="siteKey">Site key</param>
        /// <param name="requestData">A <see cref="ResetRequest"/> object</param>
        /// <returns>The created <see cref="ObjectResult"/> for the response.</returns>
        [HttpPost("reset")]
        [Consumes(typeof(VerifyRequest), MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain)]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemDetails)]
        public IActionResult Reset(
            [FromQuery, Required(ErrorMessage = "missingSiteKey")] string siteKey,
            [FromBody, Required(ErrorMessage = "missingPayload")] ResetRequest requestData)
        {
            // Get host name of incoming request
            var originHostName = GetOriginHostName(Request);

            // Check site
            var site = _configuration.Sites.FirstOrDefault(x => x.SiteKey == siteKey);
            if (site == null)
            {
                _logger.LogWarning("{Request} not for {originDomain} proceesed: Invalid {SiteKey} provided.", requestData, originHostName, siteKey);

                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    type: "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    title: _localizer["invalidSiteKey"]
                );
            }

            // Check domain name
            if ((site.ValidHostNames != null) && (site.ValidHostNames.Length > 0))
            {
                var normalizedDomainNames = site.ValidHostNames.Select(d => d.ToLowerInvariant()).ToHashSet();

                if (!StringHelper.Match(originHostName, normalizedDomainNames))
                {
                    _logger.LogWarning("{Request} not verified for domain {domain}: Domain not registered.", requestData, originHostName);

                    return Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        type: "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                        title: string.Format(_localizer["invalidDomainName"], originHostName)
                    );
                }
            }

            // Parse token
            var tokenToBeReset = Token.FromValue(requestData.Token);

            // If nonce exists, remove from cache
            if (_nonceCache.TryGetValue(tokenToBeReset.Nonce, out long nonceTimeStamp))
            {
                _nonceCache.Remove(tokenToBeReset.Nonce);

                _logger.LogInformation("{Request} for {originDomain} successfully proceesed", requestData, originHostName);

                return Ok(new ResetResponse(VerifyStatus.Success));
            }
            else
            {
                _logger.LogWarning("{Request} for {originDomain} not proceesed: Nonce already used or expired.", requestData, originHostName);

                return Ok(new ResetResponse(VerifyStatus.InvalidToken));
            }
        }

        /// <summary>
        /// Validates the CAPTCHA solution submitted by a client.
        /// </summary>
        /// <param name="siteKey">Site key</param>
        /// <param name="requestData">A <see cref="VerifyRequest"/> object</param>
        /// <returns>The created <see cref="ObjectResult"/> for the response.</returns>
        [HttpPost("verify")]
        [Consumes(typeof(VerifyRequest), MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain)]
        [ProducesResponseType(typeof(VerifyResponse), statusCode: StatusCodes.Status200OK, MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemDetails)]
        public IActionResult Verify(
            [FromQuery, Required(ErrorMessage = "missingSiteKey")] string siteKey,
            [FromBody, Required(ErrorMessage = "missingPayload")] VerifyRequest requestData)
        {
            // Check site
            var site = _configuration.Sites.FirstOrDefault(x => x.SiteKey == siteKey);
            if (site == null)
            {
                _logger.LogWarning("{Request} not verified: Invalid {SiteKey} provided.", requestData, siteKey);

                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    type: "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    title: _localizer["invalidSiteKey"]
                );
            }

            // Check site secret
            if (!StringHelper.FixedTimeEquals(requestData.SiteSecret, site.SiteSecret))
            {
                _logger.LogWarning("{Request} not verified: Invalid site secret provided.", requestData);

                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    type: "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                    title: _localizer["invalidSiteSecret"]
                );
            }

            // Parse token
            var tokenToBeVerified = Token.FromValue(requestData.Token);

            // If nonce exists...
            if (_nonceCache.TryGetValue(tokenToBeVerified.Nonce, out Tuple<long, string> nonceCacheEntry))
            {
                // Remove nonce from chache
                _nonceCache.Remove(tokenToBeVerified.Nonce);

                // Get cached values
                var nonceTimeStamp = nonceCacheEntry.Item1;
                var nonceHostName = nonceCacheEntry.Item2;

                // Get current timestamp
                var currentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Check signature
                if (!VerifyNonce(tokenToBeVerified.Nonce, tokenToBeVerified.NonceSignature, nonceCacheEntry.Item1, _configuration.HMACKey))
                {
                    _logger.LogWarning("{Request} not verified: Invalid signature.", requestData);

                    return Ok(new VerifyResponse(VerifyStatus.InvalidToken));
                }

                // Check whether the time between receiving the challenge and checking the solution was too short
                if ((currentTimeStamp - nonceTimeStamp) < _configuration.VerificationMinDelay.TotalSeconds)
                {
                    _logger.LogWarning("{Request} not verified: The time between solving the CAPTCHA and server-side verification was too short.", requestData);

                    return Ok(new VerifyResponse(VerifyStatus.InvalidToken));
                }

                // Check provided solution
                if (_configuration.ChallengeType is ProofOfWork proofOfWork)
                {
                    var hex = GenerateSolutionHash(proofOfWork.Algorithm, tokenToBeVerified.Nonce, requestData.Solution);

                    if (hex.StartsWith(new string('0', proofOfWork.Difficulty)))
                    {
                        _logger.LogInformation("Request {Request} sucessfully verified", requestData);

                        return Ok(new VerifyResponse(VerifyStatus.Success, nonceHostName));
                    }
                    else
                    {
                        _logger.LogWarning("Request {Request} not verified: Invalid solution", requestData);

                        return Ok(new VerifyResponse(VerifyStatus.InvalidSolution));
                    }
                }
                else
                {
                    _logger.LogError("{Request} not verified: Challenge type not definied.", requestData);

                    throw new ArgumentException("Challenge type not definied.");
                }
            }
            else
            {
                _logger.LogWarning("{Request} not verified: Nonce already used or expired.", requestData);

                return Ok(new VerifyResponse(VerifyStatus.InvalidToken));
            }
        }

        private static string GenerateNonceSignature(string nonce, long timestamp, string hmacKey)
        {
            var key = Encoding.UTF8.GetBytes(hmacKey);
            var data = Encoding.UTF8.GetBytes($"{nonce}:{timestamp}");

            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(data);

            return Convert.ToHexString(hash);
        }

        private static string GenerateSolutionHash(ProofOfWorkAlgorithm algorithm, string nonce, string solution)
        {
            var hashInput = $"{nonce}:{solution}";
            var hash = algorithm switch
            {
                ProofOfWorkAlgorithm.SHA256 => SHA256.HashData(Encoding.UTF8.GetBytes(hashInput)),
                ProofOfWorkAlgorithm.SHA384 => SHA384.HashData(Encoding.UTF8.GetBytes(hashInput)),
                ProofOfWorkAlgorithm.SHA512 => SHA512.HashData(Encoding.UTF8.GetBytes(hashInput)),
                _ => throw new ArgumentException("Unsupported hash algorithm", nameof(algorithm))
            };
            return Convert.ToHexStringLower(hash);
        }

        private static string GetOriginHostName(HttpRequest request)
        {
            var origin = request.Headers.Origin.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var originUri))
            {
                return originUri.Host.ToLowerInvariant();
            }
            else
            {
                return default;
            }
        }

        private static bool VerifyNonce(string nonce, string signature, long timestamp, string hmacKey)
        {
            var expected = GenerateNonceSignature(nonce, timestamp, hmacKey);
            return string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase);
        }
    }
}