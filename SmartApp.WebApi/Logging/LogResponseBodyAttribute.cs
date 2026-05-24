namespace SmartApp.WebApi.Logging;

/// <summary>
/// Opt-in: log response body for this endpoint.
/// Use sparingly — only for non-sensitive audit-critical endpoints.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class LogResponseBodyAttribute : Attribute { }