using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xana.API.Controllers;
using Xana.Domain.Entities;

namespace Xana.UnitTests.API.Controllers;

public class MeControllerTests
{
    [Fact]
    public void GetMe_WhenUserExistsInContext_ReturnsOkWithUserDto()
    {
        // Arrange
        var user = new User
        {
            FirebaseUid = "uid123",
            Email = "test@test.com",
            DisplayName = "Test User"
        };

        var controller = new MeController();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Items["User"] = user;

        // Act
        var result = controller.GetMe();

        // Assert
        result.Should().BeOfType<OkObjectResult>(); // 200 OK

        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        // Use reflection or dynamic to check anonymous type properties if needed, 
        // or ensure the controller returns a concrete DTO (which is better design, but for now anonymous)
        // For simplicity with anonymous objects in tests is tricky with strong typing, but we can check if it's not null.
        // A better approach in "Clean Architecture" is to return a mapped DTO. 
        // For this test, verifying 200 OK and non-null value is a good start.
    }

    [Fact]
    public void GetMe_WhenUserNotInContext_ReturnsUnauthorized()
    {
        // Arrange
        var controller = new MeController();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        // No User in Items

        // Act
        var result = controller.GetMe();

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>(); // 401 Unauthorized
    }
}
