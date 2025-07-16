#region RestCaptcha - Copyright (C) STÜBER SYSTEMS GmbH
/*    
 *    RestCaptcha
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
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace RestCaptcha
{
    /// <summary>
    /// RestCaptcha API controller
    /// </summary>
    [ApiController]
    [ApiVersion(1)]
    [Route("v{v:apiVersion}")]
    [SwaggerTag("RESTCaptcha API endpoints")]
    public class WebServiceController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly WebServiceConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceController"/> class.
        /// </summary>
        public WebServiceController(IOptions<WebServiceConfiguration> configuration, IMemoryCache cache)
            : base()
        {
            _configuration = configuration.Value;
            _cache = cache;
        }

        /// <summary>
        /// Generates a CAPTCHA challenge for a client.
        /// </summary>
        /// <returns>A <see cref="ChallengeResponse"/> object</returns>
        [HttpGet("challenge")]
        [ProducesResponseType(typeof(ChallengeResponse), statusCode: StatusCodes.Status200OK, MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 500, MediaTypeNames.Application.ProblemDetails)]
        public IActionResult GetChallenge()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var nonce = Guid.NewGuid().ToString("N");
            var nonceSignature = GenerateNonceSignature(nonce, timestamp);
            var proofOfWorkDifficulty = _configuration.ProofOfWorkDifficulty;

            return Ok(new ChallengeResponse(nonce, nonceSignature, timestamp, proofOfWorkDifficulty));
        }

        /// <summary>
        /// Validates the proof-of-work solution submitted by a client.
        /// </summary>
        /// <param name="body">A <see cref="VerifyRequest"/> object</param>
        [HttpPost("verify")]
        [Consumes(typeof(VerifyRequest), MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain)]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 406, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 500, MediaTypeNames.Application.ProblemDetails)]
        public IActionResult Verify([FromBody, Required] VerifyRequest body)
        {
            var trustScore = 100;
            var currentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Check signature
            if (!VerifyNonce(body.Nonce, body.TimeStamp, body.NonceSignature))
            {
                return Problem(statusCode: StatusCodes.Status406NotAcceptable, detail: $"Invalid signature.");
            }

            // Check for low entropy and recent reuse
            if (string.IsNullOrEmpty(body.Fingerprint) || body.Fingerprint.Length < 64)
            {
                trustScore -= 50;
            }
            else
            {
                if (_cache.TryGetValue(body.Fingerprint, out _))
                {
                    trustScore -= 30;
                }
                else
                {
                    _cache.Set(body.Fingerprint, true, _configuration.FingerprintTTL);
                }

                int distinctChars = body.Fingerprint.Distinct().Count();
                if (distinctChars < 16)
                {
                    trustScore -= 20;
                }
            }

            // Check whether the time between receiving the challenge and checking the solution was too short
            if ((currentTimeStamp - body.TimeStamp) < _configuration.ChallengeResponseMinDuration.TotalSeconds)
            {
                trustScore -= 30;
            }

            // Check whether the time between receiving the challenge and checking the solution was too long
            if ((currentTimeStamp - body.TimeStamp) > _configuration.ChallengeResponseMaxDuration.TotalSeconds)
            {
                trustScore -= 30;
            }

            // User-Agent sanity check
            if (!Request.Headers.UserAgent.Any(x => x.Contains("Mozilla")))
            {
                trustScore -= 30;
            }

            // Check trust scoring
            if (trustScore < 50)
            {
                return Problem(statusCode: StatusCodes.Status406NotAcceptable, detail: $"Untrusted client.");
            }

            // Check provided solution
            var hashInput = $"{body.Nonce}:{body.Solution}";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(hashInput));
            var hex = Convert.ToHexStringLower(hash);

            if (hex.StartsWith(new string('0', _configuration.ProofOfWorkDifficulty)))
            {
                return Ok();
            }
            else
            {
                return Problem(statusCode: StatusCodes.Status406NotAcceptable, detail: $"Invalid solution.");
            }
        }

        private string GenerateNonceSignature(string nonce, long timestamp)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.HMACKey);
            var data = Encoding.UTF8.GetBytes($"{nonce}:{timestamp}");

            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(data);

            return Convert.ToHexString(hash);
        }

        private bool VerifyNonce(string nonce, long timestamp, string signature)
        {
            var expected = GenerateNonceSignature(nonce, timestamp);
            return string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase);
        }
    }
}