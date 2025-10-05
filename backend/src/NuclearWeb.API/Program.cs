using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using NuclearWeb.Infrastructure.Data;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Application.Services;
using NuclearWeb.Infrastructure.FileStorage;
using NuclearWeb.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger/OpenAPI configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NuclearWeb API",
        Version = "v1",
        Description = "Integrated Multi-Module Web Application Platform"
    });

    // JWT Bearer authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

// Database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// JWT Authentication (T095)
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization policies (T096)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin", "User"));
});

// CORS configuration (T099)
var corsOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register application services with configuration
// AuthService with JWT settings
builder.Services.AddScoped<IAuthService>(sp =>
{
    var context = sp.GetRequiredService<ApplicationDbContext>();
    var jwtSecret = jwtSettings["Secret"]!;
    var jwtIssuer = jwtSettings["Issuer"]!;
    var jwtAudience = jwtSettings["Audience"]!;
    var jwtExpiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");
    var refreshTokenExpiryDays = int.Parse(jwtSettings["RefreshTokenExpiryDays"] ?? "7");

    return new AuthService(context, jwtSecret, jwtIssuer, jwtAudience, jwtExpiryMinutes, refreshTokenExpiryDays);
});

// UserService with AuthService dependency
builder.Services.AddScoped<IUserService, UserService>();

// Other services
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IMenuService, MenuService>();

// File storage infrastructure (T100)
var uploadPath = builder.Configuration["FileUpload:UploadPath"] ?? "./uploads";
builder.Services.AddSingleton<IFileStorage>(new LocalFileStorage(uploadPath));

// FileService with storage path
builder.Services.AddScoped<IFileService>(sp =>
{
    var context = sp.GetRequiredService<ApplicationDbContext>();
    return new FileService(context, uploadPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline

// Exception handling middleware (T097)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Request/response logging middleware (T098)
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NuclearWeb API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

app.UseHttpsRedirection();

// CORS must be before Authentication/Authorization
app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
