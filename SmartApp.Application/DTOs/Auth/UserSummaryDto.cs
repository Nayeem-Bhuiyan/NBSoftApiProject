namespace SmartApp.Application.DTOs.Auth;

public sealed class UserSummaryDto
{
    public string Id { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public bool? IsActive { get; init; }
    public List<string> Roles { get; init; } = new();
}