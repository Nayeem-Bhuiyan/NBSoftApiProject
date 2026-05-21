namespace SmartApp.Application.DTOs.Auth;

public sealed record AssignPermissionDto(
    string RoleId,
    int PermissionId,
    bool IsGranted
);