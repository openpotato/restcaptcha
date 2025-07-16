![GitHub](https://img.shields.io/github/license/openpotato/restcaptcha)

# RESTCaptcha

RESTCaptcha is a lightweight, privacy-friendly CAPTCHA solution that requires no image puzzles, no third-party tracking, and no persistent server-side sessions.

Instead, it uses proof-of-work and optional browser fingerprinting to verify human interaction with your form.

Features:

+ Stateless challenge using HMAC-signed nonce
+ Client-side proof-of-work puzzle (SHA-256 hash under threshold)
+ Optional trust scoring using browser fingerprint entropy
+ Fully integrable with Node.js, PHP, ASP.NET or any other server technology
+ CDN-ready restcaptcha.js script with pluggable endpoints
+ Build with [.NET 9](https://dotnet.microsoft.com/) and [JavaScript](https://developer.mozilla.org/docs/Web/JavaScript).

## Technology stack

+ [ASP.NET 9](https://dotnet.microsoft.com/apps/aspnet) as web framework
+ [Swagger UI](https://swagger.io/tools/swagger-ui/) for OpenAPI based documentation
+ [JavaScript](https://developer.mozilla.org/docs/Web/JavaScript) for the restcaptcha.js client script

## Getting started 

The following instructions show you how to set up a development environment on your computer.

### Prerequisites

+ Clone or download this repository.
+ Open the solution file `RestCaptcha.sln` in Visual Studio 2022.

### Configure the RestCaptcha WebService

+ Switch to the  `RestCaptcha.WebService`. 
+ Make a copy of the the `appsettings.json` file and name it `appsettings.Development.json`.
+ Exchange the content with the following JSON document and adjust the values to your needs. This configures the database connection.

    ``` json
    "Database": {
      "Server": "localhost",
      "Database": "CodeListHub",
      "Username": "postgres",
      "Password": "qwertz"
    }
    ```

### Build and test the API

+ Build the `RestCaptcha.WebService` project.
+ Run the `RestCaptcha.WebService` project and play with the Swagger UI.

## Can I help?

Yes, that would be much appreciated. The best way to help is to post a response via the Issue Tracker and/or submit a Pull Request.
