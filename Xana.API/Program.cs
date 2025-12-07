using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Xana.Infrastructure.Middleware;
using Xana.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuration Check
// In Cloud Run, secrets are ENV variables. In Repos, they are IConfiguration.
// We expect "ConnectionStrings:DefaultConnection" and "Firebase:CredentialPath" or ENV vars.

// 2. Add Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Xana Brain API", Version = "v1" });
    // Add JWT Bearer Security Definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Firebase JWT Token. Example: \"Bearer {token}\"",
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
            new string[] {}
        }
    });
});

// 3. Database
// Using Npgsql with EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? Environment.GetEnvironmentVariable("DATABASE_URL"); // Fallback for some cloud providers

if (string.IsNullOrEmpty(connectionString))
{
    // For now, allow running without DB for build check, but warn.
    Console.WriteLine("WARNING: No ConnectionString found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 4. Firebase
try
{
    // Check if we have a credential file path configured, otherwise assume Application Default Credentials (ADC)
    var firebaseConfig = builder.Configuration["Firebase:CredentialPath"];

    if (!string.IsNullOrEmpty(firebaseConfig) && File.Exists(firebaseConfig))
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(firebaseConfig)
        });
    }
    else
    {
        // Use ADC (good for Cloud Run)
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.GetApplicationDefault()
        });
    }
}
catch (Exception ex)
{
    Console.WriteLine($"WARNING: Firebase Init Failed: {ex.Message}. Ignore if running build only.");
}

var app = builder.Build();

// 5. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Custom Middleware MUST be before UseAuthorization
app.UseMiddleware<FirebaseUserMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
