namespace SmartApp.Application.DTOs.Auth;

public sealed class RolePermissionDto
{
    public int PermissionId { get; init; }
    public string Controller { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string HttpMethod { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public bool IsGranted { get; init; }
}