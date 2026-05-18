namespace SmartApp.Application.DTOs.Auth;

public record PermissionCacheEntry(string Controller, string Action, string HttpMethod);