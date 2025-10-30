#region RESTCaptcha ASP.NET Core MVC Example
/*    
 *    RESTCaptcha ASP.NET Core MVC Example
 */
#endregion

using RestCaptcha;

var builder = WebApplication.CreateBuilder(args);

// Bind configuration
builder.Services.Configure<Configuration>(
    builder.Configuration.GetSection("CaptchaSample"));

// Add controller support
builder.Services.AddControllersWithViews();

// Let's build our web app
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{action=Index}",
    defaults: new { controller = "Home" });

app.Run();