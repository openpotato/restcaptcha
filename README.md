![GitHub](https://img.shields.io/github/license/openpotato/restcaptcha)

# RESTCaptcha

RESTCaptcha is a lightweight, privacy-friendly CAPTCHA solution that requires no image puzzles, no third-party tracking, and no persistent server-side sessions.

Instead, it uses proof-of-work to verify human interaction with your form.

Features:

+ Stateless challenge using HMAC-signed nonce
+ Client-side proof-of-work puzzle (SHA-256 or SHA-512 hash under threshold)
+ Fully integrable with Node.js, PHP, ASP.NET or any other server technology
+ CDN-ready restcaptcha.js script with pluggable API endpoint
+ Build with [.NET 9](https://dotnet.microsoft.com/) and [JavaScript](https://developer.mozilla.org/docs/Web/JavaScript).

## Technology stack

+ [ASP.NET 9](https://dotnet.microsoft.com/apps/aspnet) as web framework
+ [JavaScript](https://developer.mozilla.org/docs/Web/JavaScript) for the restcaptcha.js client script

## Getting started 

Documentation is available in the [GitHub wiki](https://github.com/openpotato/restcaptcha/wiki).

## Can I help?

Yes, that would be much appreciated. The best way to help is to post a response via the Issue Tracker and/or submit a Pull Request.
