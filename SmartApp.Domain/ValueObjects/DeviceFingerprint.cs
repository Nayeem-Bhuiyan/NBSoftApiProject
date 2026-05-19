namespace SmartApp.Domain.ValueObjects;

public sealed record DeviceFingerprint(
    string DeviceId,
    string UserAgent,
    string IpAddress,
    string Platform
)
{
    public string ToCompositeKey() =>
        $"{DeviceId}:{IpAddress}:{Platform}";
}