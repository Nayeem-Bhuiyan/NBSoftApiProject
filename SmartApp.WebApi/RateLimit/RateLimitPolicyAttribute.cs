namespace SmartApp.WebApi.RateLimit;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class RateLimitPolicyAttribute : Attribute
{
    public string PolicyName { get; }

    public RateLimitPolicyAttribute(string policyName)
    {
        PolicyName = policyName;
    }
}