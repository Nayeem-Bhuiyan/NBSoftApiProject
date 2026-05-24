namespace SmartApp.Domain.Entities.Logging;

public sealed class ApplicationLog
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string MessageTemplate { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
    public string? LogEvent { get; set; }

    // ← enriched fields for fintech audit
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestPath { get; set; }
    public string? HttpMethod { get; set; }
    public int? StatusCode { get; set; }
    public long? DurationMs { get; set; }
    public string? MachineName { get; set; }
    public string? Environment { get; set; }
}