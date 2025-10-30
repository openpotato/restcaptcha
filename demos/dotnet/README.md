# RESTCaptcha ASP.NET Core Example

This example integrates RESTCaptcha into a ASP.NET Core MVC project. 

## Dependencies

+ .NET 9
+ [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
+ [RESTCaptcha .NET Client](https://github.com/openpotato/restcaptcha-client.net)
+ [Bootstrap 5](https://getbootstrap.com/)

## Getting started

1. Copy this file from 'src/appsettings.json' to 'src/appsettings.Development.json' and change the settings according to your needs. By default, the settings are compatible with the RESTCaptcha Visual Studio project running on the same machine. 

2. Open `CaptchaSample.sln` in Visual Studio 2022.

3. Run `composer install` from Terminal to install dependencies.

4. Launch 'PHP Debug'.

5. Start task `Open Browser (Windows)` or `Open Browser (Unix)`.




# StaticLoginMvc (.NET 8, ASP.NET Core MVC)

A minimal MVC project with a Bootstrap 5 login screen. Submitting echoes the username and password (demo only).

## Quickstart (Visual Studio 2022)
1. Open `StaticLoginMvc.sln`.
2. Press **F5** (Debug/Run). VS will restore packages and start Kestrel.
3. The app opens at `/Home/Login`.
4. Enter any username/password and submit; you'll see both echoed on the next page.

> Security note: This sample echoes the password back to the browser for demonstration purposes. Do **not** reuse real credentials.