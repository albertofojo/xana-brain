using Xana.Domain.Enums;

namespace Xana.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirebaseUid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? PhotoUrl { get; set; }
    public UserRole Role { get; set; } = UserRole.User;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;

    // Domain Methods
    public void UpdateLoginDate()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string? displayName, string? photoUrl)
    {
        DisplayName = displayName;
        PhotoUrl = photoUrl;
    }
}
