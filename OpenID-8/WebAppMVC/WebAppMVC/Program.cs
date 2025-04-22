// https://juliocasal.com/blog/Securing-Aspnet-Core-Applications-With-OIDC-And-Microsoft-Entra-ID
/*
 * Code example of an MVC using Authentication provided by OpenID
 * The OpenIDConnect Identity Provider is KeyCloak
 * 
 * NB: Do not attempt to do this in a containerized version
 *     as the certificate handling is very tricky
 *     https and containers don't mix well.
*/



// >>> added using directives
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;
using WebAppMVC.Services;
// <<<

var builder = WebApplication.CreateBuilder(args);

// Added to support OpenIDConnect
// Setting up OpenIDConnect authentication
builder.Services
       .AddAuthentication()
       .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
       {
           // Authority could be https://auth.a.ucnit.eu/realms/xOIDCx
           options.Authority = System.Environment.GetEnvironmentVariable("OpenIDRealmURI");
           options.Audience = "account";
           options.MapInboundClaims = false;
       });
builder.Services
        .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddOpenIdConnect(options =>
        {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.Authority = System.Environment.GetEnvironmentVariable("OpenIDRealmURI");
            options.ClientId = System.Environment.GetEnvironmentVariable("OpenIDClient");
            options.ClientSecret = System.Environment.GetEnvironmentVariable("OpenIDSecret");
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = true;
            options.MapInboundClaims = false;
            options.Scope.Add("openid");
            options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            options.TokenValidationParameters.RoleClaimType = "roles";
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
builder.Services
       .AddAuthorizationBuilder()
       .AddPolicy("read_access", builder =>
       {
           // claim, list of acceptable values
           builder.RequireClaim("myClaim", "MyClaimValueRO1", "MyClaimValueRO2");
       })
       .AddPolicy("write_access", builder =>
       {
           builder.RequireClaim("myClaim", "MyClaimValueRW1", "MyClaimValueRW2");
       });
builder.Services.AddAuthorizationBuilder();
builder.Services.AddAuthorization();

// Register HttpContextAccessor for getting the HttpContext.
builder.Services.AddHttpContextAccessor();

// Configure HttpClient with token retrieval from HttpContext
builder.Services.AddHttpClient("WeatherApiClient", (provider, client) =>
{
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    var httpContext = httpContextAccessor.HttpContext;

    if (httpContext?.User?.Identity?.IsAuthenticated ?? false)
    {
        var accessToken = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    client.BaseAddress = new Uri("https://localhost:8888/");
});
builder.Services.AddScoped<WeatherService>();
// <<<



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// >>> added to support OpenIDConnect, endpoints
// Synthetic endpoint for authentication
// Call this endpoint to start login sequence.
// Something like this will do: <a href="authentication/login" class="btn btn-warning">Login</a>
app.MapGet("/authentication/login", ()
    => TypedResults.Challenge(
        new AuthenticationProperties { RedirectUri = "/" }))
    .AllowAnonymous();
app.MapGet("/authentication/logout", ()
    => TypedResults.Challenge(
        new AuthenticationProperties { RedirectUri = "/?theaction=logout" }))
    .AllowAnonymous();

// <<<


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
