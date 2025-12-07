using FluentAssertions;
using Xana.Domain.Entities;

namespace Xana.UnitTests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void UpdateLoginDate_ShouldSetLastLoginAt_ToRecentTime()
    {
        // Arrange
        var user = new User();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        user.UpdateLoginDate();

        // Assert
        user.LastLoginAt.Should().BeAfter(beforeUpdate);
        user.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateDisplayNameAndPhotoUrl()
    {
        // Arrange
        var user = new User();
        var newName = "New Name";
        var newPhoto = "http://photo.url";

        // Act
        user.UpdateProfile(newName, newPhoto);

        // Assert
        user.DisplayName.Should().Be(newName);
        user.PhotoUrl.Should().Be(newPhoto);
    }
}
