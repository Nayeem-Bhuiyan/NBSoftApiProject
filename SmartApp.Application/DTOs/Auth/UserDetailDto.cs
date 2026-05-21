namespace SmartApp.Application.DTOs.Auth;

public sealed class UserDetailDto
{
    public string Id { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool? IsActive { get; init; }
    public bool EmailConfirmed { get; init; }
    public int AccessFailedCount { get; init; }
    public DateTimeOffset? LockoutEnd { get; init; }
    public List<string> Roles { get; init; } = new();
}