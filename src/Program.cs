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
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using RestCaptcha;
using System.Net;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Bind configuration
builder.Services.Configure<WebServiceConfiguration>(
    builder.Configuration.GetSection("RestCaptcha"));

// Enable cross-origin resource sharing 
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .WithMethods(WebRequestMethods.Http.Get, WebRequestMethods.Http.Post).AllowAnyHeader();
              //.WithHeaders(HeaderNames.ContentType, HeaderNames.Accept, HeaderNames.AcceptLanguage);
    });
});

// Add controller support
builder.Services
    .AddControllers()
    .AddJsonOptions(setup =>
    {
        setup.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        setup.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Exception handling
builder.Services.AddProblemDetails();

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
    setup.EnableAnnotations();
    setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "RestCaptcha.WebService.xml"));
    //setup.OperationFilter<AppOperationFilter>();
    //setup.OperationFilter<ProblemDetailsOperationFilter>();
    setup.OrderActionsBy((apiDesc) => apiDesc.RelativePath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
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

// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "RESTCaptcha";
        options.SwaggerEndpoint("v1/swagger.json", "RESTCaptcha v1");
    });
}

app.UseCors();
app.UseStaticFiles();
app.MapControllers();
app.Run();
