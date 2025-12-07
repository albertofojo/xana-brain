using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xana.Domain.Entities;
using Xana.Domain.Enums;
using Xana.Infrastructure.Persistence;

namespace Xana.Infrastructure.Middleware;

public class FirebaseUserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<FirebaseUserMiddleware> _logger;

    public FirebaseUserMiddleware(RequestDelegate next, ILogger<FirebaseUserMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        string? authHeader = context.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            await _next(context);
            return;
        }

        string token = authHeader.Substring("Bearer ".Length).Trim();

        try
        {
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            string uid = decodedToken.Uid;

            // Extract claims
            string email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"].ToString() ?? "" : "";
            string name = decodedToken.Claims.ContainsKey("name") ? decodedToken.Claims["name"].ToString() ?? "" : "";
            string picture = decodedToken.Claims.ContainsKey("picture") ? decodedToken.Claims["picture"].ToString() ?? "" : "";

            // Find or Create User
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.FirebaseUid == uid);

            if (user == null)
            {
                user = new User
                {
                    FirebaseUid = uid,
                    Email = email,
                    DisplayName = name,
                    PhotoUrl = picture,
                    Role = UserRole.User
                };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
                _logger.LogInformation($"Created new user for Firebase UID: {uid}");
            }
            else
            {
                // Update Last Login
                // Only update database if significantly changed to avoid effortless writes on every request if needed, 
                // but for now, simple login date update is fine.
                if (DateTime.UtcNow - user.LastLoginAt > TimeSpan.FromMinutes(5))
                {
                    user.UpdateLoginDate();
                    await dbContext.SaveChangesAsync();
                }
            }

            // Store user in context items for easy access in Controllers
            context.Items["User"] = user;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to verify Firebase Token");
            // Do not block request here, let Controllers decide if they need [Authorize]
            // But if the header was Authorization, it's likely they wanted to be authenticated.
        }

        await _next(context);
    }
}
