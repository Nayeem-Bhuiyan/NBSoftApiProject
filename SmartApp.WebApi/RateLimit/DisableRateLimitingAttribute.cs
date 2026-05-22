namespace SmartApp.WebApi.RateLimit;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class DisableRateLimitingAttribute : Attribute { }