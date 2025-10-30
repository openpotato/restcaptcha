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
using Enbrea.ApiKey;
using HealthChecks.ApplicationStatus.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using RestCaptcha;
using Serilog;
using System.Net;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Bind configuration
builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("RestCaptcha"));

// Monitor changes in configration
builder.Services.AddSingleton<IOptionsMonitor<AppConfiguration>, OptionsMonitor<AppConfiguration>>();

// Add Cross-Origin Resource Sharing support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .WithMethods(WebRequestMethods.Http.Get, WebRequestMethods.Http.Post).AllowAnyHeader()
              .WithHeaders(HeaderNames.ContentType, HeaderNames.Accept, HeaderNames.AcceptLanguage);
    });
});

// Add HTTP Strict Transport Security) support
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(180); 
    options.IncludeSubDomains = true;        
});

// Add localization support
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

// Add API-key validation support
builder.Services
    .AddApiKeyValidation()
    .UseProblemDetailsFactory();

// Add controller support
builder.Services
    .AddControllers()
    .AddDataAnnotationsLocalization(setup =>
        setup.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource))
    )
    .AddJsonOptions(setup =>
    {
        setup.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        setup.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add exception handling
builder.Services.AddProblemDetails();

// Add memory cache
builder.Services.AddMemoryCache();

// Add API versioning
builder.Services.
    AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// Add health checks
builder.Services
    .AddHealthChecks()
    .AddApplicationStatus("self");

// Add service metadata
builder.Services.AddServiceMetadata();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "RESTCaptcha v1",
            Version = "v1",
            Description = "API for the RESTCaptcha Project",
            Contact = new OpenApiContact
            {
                Name = "The RestCaptcha Project",
                Url = new Uri("https://github.com/openpotato/restcaptcha")
            },
            License = new OpenApiLicense
            {
                Name = "License",
                Url = new Uri("https://github.com/openpotato/restcaptcha/blob/main/LICENSE")
            }
        });

    setup.AddSecurityDefinition("ApiKeyHeader", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Description = "Provide your API key in the X-API-KEY header."
    });

    setup.AddSecurityDefinition("ApiKeyAuthorizationHeader", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Provide your API key in format: ApiKey {your-key}"
    });

    setup.EnableAnnotations();
    setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "RestCaptcha.WebService.xml"));
    setup.OperationFilter<ApiKeyRequirementOperationFilter>();
    setup.OrderActionsBy((apiDesc) => apiDesc.RelativePath);
});

// Use Serilog for logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration);
});

// Let's build our web app
var app = builder.Build();

// Use Serilog request logging
app.UseSerilogRequestLogging();

// Flush Serilog when the host stops
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "RESTCaptcha";
        options.SwaggerEndpoint("v1/swagger.json", "RESTCaptcha v1");
    });
}
else
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
    });
    app.UseStatusCodePages();
    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        ExceptionHandler = async (HttpContext context) =>
        {
            // Pass-through status codes from BadHttpRequestException. See: https://github.com/dotnet/aspnetcore/issues/43831
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var error = exceptionHandlerFeature?.Error;

            if (error is BadHttpRequestException badRequestEx)
            {
                context.Response.StatusCode = badRequestEx.StatusCode;
            }

            if (context.RequestServices.GetRequiredService<IProblemDetailsService>() is { } problemDetailsService)
            {
                await problemDetailsService.WriteAsync(new()
                {
                    HttpContext = context,
                    AdditionalMetadata = exceptionHandlerFeature?.Endpoint?.Metadata,
                    ProblemDetails = { Status = context.Response.StatusCode, Detail = error?.Message }
                });
            }
            else if (ReasonPhrases.GetReasonPhrase(context.Response.StatusCode) is { } reasonPhrase)
            {
                await context.Response.WriteAsync(reasonPhrase);
            }
        }
    });
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseRequestLocalization(options =>
{
    options.DefaultRequestCulture = new RequestCulture(LocalizationHelper.GetDefaultCultureName());
    options.SupportedCultures = LocalizationHelper.GetSupportedCultures();
    options.SupportedUICultures = LocalizationHelper.GetSupportedCultures();
    options.RequestCultureProviders =
    [
        new QueryStringRequestCultureProvider() { QueryStringKey = "language" },
        new AcceptLanguageHeaderRequestCultureProvider()
    ];
});

app.UseCors();
app.UseStaticFiles();
app.MapControllers();
app.Run();
