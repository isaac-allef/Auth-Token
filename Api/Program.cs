using System.Text;
using Api.Models;
using Api.Services;
using Api.Services.Caching;
using Api.UseCases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configure Swagger
builder.Services.AddSwaggerGen(s => {
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Here enter JWT token with bearer format like bearer[space] token"
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
//

// Configure auth
var settings = builder.Configuration.Get<Settings>();
var secretKey = Encoding.ASCII.GetBytes(settings.Secret);
builder.Services.AddAuthentication(a =>
{
    a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(j =>
{
    j.RequireHttpsMetadata = false;
    j.SaveToken = true;
    j.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization();
//

builder.Services.AddSingleton(builder.Configuration.Get<Settings>());

var useRedis = builder.Configuration.GetValue<bool>("UseRedis");
if (!useRedis)
{
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<ICachingService, CachingMemoryService>();
    const int concurrentTasksNumber = 1;
    builder.Services.AddSingleton<ILockingService>(new LockingSemaphoreService(concurrentTasksNumber));
}
else
{
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddScoped<ICachingService, CachingDistributedRedisService>();
    var redisConnection = await ConnectionMultiplexer.ConnectAsync(settings.RedisConnectionString);
    builder.Services.AddSingleton<ILockingService>(new LockingDistributedRedisService(redisConnection));
}

builder.Services.AddScoped<ITokenService, TokenJwtService>();
builder.Services.AddScoped<IAuthenticateUseCase, AuthenticateUseCase>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
