<div align="center">
  
# RESTCaptcha

[![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![GitHub License](https://img.shields.io/github/license/openpotato/restcaptcha?style=for-the-badge)](./LICENSE)
[![Documentation](https://img.shields.io/badge/docs-available-brightgreen?style=for-the-badge)](https://restcaptcha.openpotato.org)

</div>

RESTCaptcha is a lightweight, privacy-friendly CAPTCHA solution that requires no image puzzles, no third-party tracking, and no persistent server-side sessions.

Instead, it uses proof-of-work to verify human interaction with your form.

Features:

+ Stateless challenge using HMAC-signed nonce
+ Client-side proof-of-work puzzle (SHA-256 or SHA-512 hash under threshold)
+ CDN-ready restcaptcha.js script with pluggable API endpoint
+ Optional IP reputation check as an additional security layer
+ Fully customisable
+ Fully integrable with Node.js, PHP, ASP.NET or any other server technology
+ Support for different modes (interactive, auto, invisible, headless)
+ Support for multiple languages
+ Build with [.NET 10](https://dotnet.microsoft.com/) and [JavaScript](https://developer.mozilla.org/docs/Web/JavaScript).

## Technology stack

+ [.NET 10](https://dotnet.microsoft.com/)
+ [ASP.NET](https://dotnet.microsoft.com/apps/aspnet) as web framework
+ [JavaScript](https://developer.mozilla.org/docs/Web/JavaScript) for the restcaptcha.js client script

## Getting started 

Starting points for documentation can be found in the [GitHub wiki](https://github.com/openpotato/restcaptcha/wiki).

## Can I help?

Yes, that would be much appreciated. The best way to help is to post a response via the Issue Tracker and/or submit a Pull Request.
