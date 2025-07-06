using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ERPCompanySystem.Models;
using ERPCompanySystem.Services;
using ERPCompanySystem.Attributes;
using ERPCompanySystem.Authorization;
using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configure rate limiting
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = StatusCodes.Status429TooManyRequests;
    options.RealIpHeader = "X-Real-IP";
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "api/auth/login",
            Period = "5m",
            Limit = 10,
            IpWhitelist = new List<string> { "127.0.0.1", "::1" }
        },
        new RateLimitRule
        {
            Endpoint = "api/[controller]",
            Period = "15m",
            Limit = 100,
            IpWhitelist = new List<string> { "127.0.0.1", "::1" }
        },
        new RateLimitRule
        {
            Endpoint = "api/sales/[action]",
            Period = "1m",
            Limit = 20,
            IpWhitelist = new List<string> { "127.0.0.1", "::1" }
        }
    };
});

// Configure IP blocking
builder.Services.Configure<IpPolicyStoreOptions>(options =>
{
    options.PolicyKey = "ip-policy";
    options.PolicyDuration = TimeSpan.FromDays(1);
});

// Configure JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Configure connection pooling for SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
            .UseConnectionResiliency()
            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            .EnableDetailedErrors(true)
            .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())));

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder
            .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>())
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configure database connection with connection pooling and retry logic
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions
            .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null)
            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            .EnableDetailedErrors(true)
            .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()))
    .UseInternalServiceProvider(builder.Services.BuildServiceProvider()));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero, // لا تسمح بتمديد توكنات منتهية الصلاحية
        RequireExpirationTime = true, // تحقق من وجود توكن انتهاء
        RequireSignedTokens = true
    };
});

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => 
        policy.RequireRole("Admin")
             .RequireAuthenticatedUser());
    
    options.AddPolicy("Manager", policy => 
        policy.RequireRole("Admin", "Manager")
             .RequireAuthenticatedUser());
    
    options.AddPolicy("User", policy => 
        policy.RequireRole("Admin", "Manager", "User")
             .RequireAuthenticatedUser());
    
    options.AddPolicy("RequireJwt", policy => 
        policy.Requirements.Add(new CustomAuthorizeAttribute(true)));
    
    options.AddPolicy("RequireRefreshToken", policy => 
        policy.Requirements.Add(new CustomAuthorizeAttribute(false)));
});

// Register authorization handlers
builder.Services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore// Configure Swagger/OpenAPI with XML comments
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ERPCompanySystem API V1");
        options.RoutePrefix = string.Empty;
        options.InjectStylesheet("/swagger-ui/custom.css");
        options.InjectJavascript("/swagger-ui/custom.js");
        options.OAuthClientId("swagger-ui");
        options.OAuthClientSecret("secret");
        options.OAuthRealm("ERPCompanySystem");
        options.OAuthAppName("ERPCompanySystem API");
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Security middleware
app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Logging and error handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
        
        if (ex is SecurityTokenExpiredException)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Token has expired" });
        }
        else
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "An internal server error occurred" });
        }
    }
});

// Configure middleware pipeline
app.UseStaticFiles();
app.UseRouting();

// Security middleware
app.UseCors("AllowFrontend");
app.UseIpRateLimiting();
app.UseMiddleware<IpBlockingMiddleware>();

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Global error handling
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            var activityContext = Activity.Current?.Id ?? context.TraceIdentifier;
            var statusCode = context.Response.StatusCode;
            var message = error.Error.Message;
            var stackTrace = builder.Environment.IsDevelopment() ? error.Error.StackTrace : null;
            var requestPath = context.Request.Path;

            // Log the error
            logger.LogError(error.Error, "An unhandled exception occurred: {Message}", message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(new
            {
                error = true,
                code = statusCode,
                message = builder.Environment.IsDevelopment() ? message : "Internal server error",
                stackTrace,
                timestamp = DateTime.UtcNow,
                path = requestPath,
                requestId = activityContext
            });
        }
    });
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
