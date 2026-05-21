namespace SmartApp.Application.DTOs.Auth;

public sealed record BulkAssignPermissionsDto(
    string RoleId,
    string Controller,  // ← assign all actions of this controller
    bool IsGranted
);