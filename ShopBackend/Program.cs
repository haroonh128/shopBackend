// Api/Program.cs
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Services;
using Shop.Infrastructure.UnitOfWork;
using ShopBackend.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid request value." : e.ErrorMessage))
                .ToList();

            var response = BaseResponse<object>.ErrorResponse(
                "Validation failed.",
                errors.Count > 0 ? errors : new List<string> { "One or more validation errors occurred." }
            );

            return new BadRequestObjectResult(response);
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Authentication API",
        Version = "v1",
        Description = "Authentication API with Two-Factor Authentication",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Paste only your access token from the login/verify-otp response (no 'Bearer' prefix needed).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

});

var appMode = builder.Configuration["AppMode"];
// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (appMode == "Desktop")
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "ShopPOS", "pos.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        options.UseSqlite($"Data Source={dbPath}", b => b.MigrationsAssembly("Shop.Infrastructure"));
    }
    else
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            b =>
            {
                b.MigrationsAssembly("Shop.Infrastructure");
            });
    }
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<ISmsService, SmsService>();

builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<ILicenseService, LicenseService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAddressService, AddressService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    options.SaveToken = true;
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
                context.Response.Headers.Append("Token-Expired", "true");
            return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {
            context.HandleResponse();

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = BaseResponse<object>.ErrorResponse(
                    "Unauthorized access.",
                    new List<string> { "A valid access token is required." }
                );

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
            }
        },
        OnForbidden = async context =>
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = BaseResponse<object>.ErrorResponse(
                    "Forbidden.",
                    new List<string> { "You do not have permission to access this resource." }
                );

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
            }
        }
    };
});


// CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    options.AddPolicy("ConfiguredOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddAuthorization();

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// Response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication API V1");
    });
}

// Apply CORS before custom middleware and auth so preflight (OPTIONS) can succeed.
app.UseCors(app.Environment.IsDevelopment() ? "DevCors" : "ConfiguredOrigins");

// Custom middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (appMode != "Desktop")
{
    app.UseHttpsRedirection();
}
app.UseResponseCompression();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Seed database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        await context.Database.MigrateAsync();
        app.Logger.LogInformation("Database migrated successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating the database");
    }
}

app.Run();