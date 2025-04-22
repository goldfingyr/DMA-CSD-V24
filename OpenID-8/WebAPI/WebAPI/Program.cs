/*
 * Code example of an MVC using Authentication provided by OpenID
 * The OpenIDConnect Identity Provider is KeyCloak
 * 
 * NB: Do not attempt to do this in a containerized version
 *     as the certificate handling is very tricky
 *     https and containers don't mix well.
*/

// >>> Added namespaces
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
// <<<

var builder = WebApplication.CreateBuilder(args);

// >>> This adds the authentication service
builder.Services
       .AddAuthentication()
       .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
       {
           // OpenIDRealmURI coud be "https://auth.a.ucnit.eu/realms/xOIDCx"
           options.Authority = System.Environment.GetEnvironmentVariable("OpenIDRealmURI");
           options.Audience = "account";
           options.MapInboundClaims = false;
           options.Events = new JwtBearerEvents()
           {
               OnMessageReceived = msg =>
               {
                   var token = msg?.Request.Headers.Authorization.ToString();
                   string path = msg?.Request.Path ?? "";
                   if (!string.IsNullOrEmpty(token))

                   {
                       Console.WriteLine("Access token");
                       Console.WriteLine($"URL: {path}");
                       Console.WriteLine($"Token: {token}\r\n");
                   }
                   else
                   {
                       Console.WriteLine("Access token");
                       Console.WriteLine("URL: " + path);
                       Console.WriteLine("Token: No access token provided\r\n");
                   }
                   return Task.CompletedTask;
               }
           };
           options.Events = new JwtBearerEvents()
           {
               //...

               OnTokenValidated = ctx =>
               {
                   Console.WriteLine();
                   Console.WriteLine("Claims from the access token");
                   if (ctx?.Principal != null)
                   {
                       foreach (var claim in ctx.Principal.Claims)
                       {
                           Console.WriteLine($"{claim.Type} - {claim.Value}");
                       }
                   }
                   Console.WriteLine();
                   return Task.CompletedTask;
               }
           };

       });
// <<<

// >>> This adds the authorization service
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("roles", "admin", "warlock"));
});
// <<<

//builder.Services.AddAuthorization();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// >>> Add Swagger with Authorization option
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
     new OpenApiSecurityScheme
     {
       Reference = new OpenApiReference
       {
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer"
       }
      },
      new string[] { }
    }
  });
});
// <<<

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
