using Microsoft.AspNetCore.Mvc;
using Xana.Domain.Entities;

namespace Xana.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeController : ControllerBase
{
    [HttpGet]
    public IActionResult GetMe()
    {
        // Retrieved from FirebaseUserMiddleware
        if (HttpContext.Items["User"] is User user)
        {
            return Ok(new
            {
                user.Id,
                user.Email,
                user.DisplayName,
                user.PhotoUrl,
                Role = user.Role.ToString(),
                user.FirebaseUid
            });
        }

        return Unauthorized(new { message = "User not authenticated or not found in database." });
    }
}
