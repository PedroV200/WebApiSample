using System.Security.Claims;
using App.Middlewares;
using App.ValidateScopes;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

// LISTED 19_7_2023 13_23  


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();

builder.Host.ConfigureAppConfiguration((configBuilder) =>
{
    //configBuilder.Sources.Clear();
    DotEnv.Load();
    configBuilder.AddEnvironmentVariables();
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

// Add services to the container.
//builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            builder.Configuration.GetValue<string>("CLIENT_ORIGIN_URL"))
            .WithHeaders(new string[] {
                HeaderNames.ContentType,
                HeaderNames.Authorization,
            })
            //.WithMethods("GET")
            //.SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
            .AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers();

builder.Host.ConfigureServices((services) =>
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var audience = builder.Configuration.GetValue<string>("AUTH0_AUDIENCE");

            options.Authority = $"https://{builder.Configuration.GetValue<string>("AUTH0_DOMAIN")}/";
            options.Audience = audience;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                NameClaimType = ClaimTypes.NameIdentifier
            };
        })
);



/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = domain;
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});
*/

var domain =  $"https://{builder.Configuration.GetValue<string>("AUTH0_DOMAIN")}/";

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("read:sample-role-admin-messages", policy => policy.Requirements.Add(new HasScopeRequirement("read:sample-role-admin-messages", domain)));
    //options.AddPolicy("put:sample-role-admin-messages", policy => policy.Requirements.Add(new HasScopeRequirement("put:sample-role-admin-messages", domain)));
    options.AddPolicy("tarifas:read", policy => policy.Requirements.Add(new HasScopeRequirement("tarifas:read", domain)));
    options.AddPolicy("tarifas:update", policy => policy.Requirements.Add(new HasScopeRequirement("tarifas:update", domain)));
    options.AddPolicy("tarifas:create", policy => policy.Requirements.Add(new HasScopeRequirement("tarifas:create", domain)));
    options.AddPolicy("tarifas:delete", policy => policy.Requirements.Add(new HasScopeRequirement("tarifas:delete", domain)));
});



builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Agrega el middleware CORS antes de app.Run()
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

var requiredVars =
    new string[] {
          "PORT",
          "CLIENT_ORIGIN_URL",
          "AUTH0_DOMAIN",
          "AUTH0_AUDIENCE",
    };

foreach (var key in requiredVars)
{
    var value = app.Configuration.GetValue<string>(key);

    if (value == "" || value == null)
    {
        throw new Exception($"Config variable missing: {key}.");
    }
}

// app.Urls.Add($"http://+:{app.Configuration.GetValue<string>("PORT")}");



app.UseErrorHandler();
app.UseSecureHeaders();
app.MapControllers();
app.UseCors();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});




app.Run();
