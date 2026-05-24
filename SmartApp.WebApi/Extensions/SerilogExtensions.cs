using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using SmartApp.WebApi.Logging;
using System.Collections.ObjectModel;
using System.Data;

namespace SmartApp.WebApi.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        var logSettings = builder.Configuration
            .GetSection(LogSettings.SectionName)
            .Get<LogSettings>() ?? new LogSettings();

        var connectionString = builder.Configuration
            .GetConnectionString("AppDbConnection")
            ?? throw new InvalidOperationException("AppDbConnection not configured.");

        var columnOptions = BuildColumnOptions();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration) // ← reads MinimumLevel from appsettings
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.WithCorrelationId()

            // ── Console sink (Development) ────────────────────────────────
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj} " +
                                "{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Debug)

            // ── File sink (rolling daily) ─────────────────────────────────
            .WriteTo.File(
                path: logSettings.LogFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: logSettings.RetainedFileCount,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] " +
                                        "[{CorrelationId}] [{UserId}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information)

            // ── MSSQL sink ────────────────────────────────────────────────
            .WriteTo.MSSqlServer(
                connectionString: connectionString,
                sinkOptions: new MSSqlServerSinkOptions
                {
                    TableName        = logSettings.MsSqlTableName,
                    AutoCreateSqlTable = false, // ← EF migration handles creation
                    BatchPostingLimit = 50
                },
                columnOptions: columnOptions,
                restrictedToMinimumLevel: LogEventLevel.Warning) // ← only warnings+ to DB

            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }

    private static ColumnOptions BuildColumnOptions()
    {
        var columnOptions = new ColumnOptions();

        // ← remove default columns not needed
        columnOptions.Store.Remove(StandardColumn.Properties);
        columnOptions.Store.Remove(StandardColumn.MessageTemplate);

        // ← keep these standard columns
        columnOptions.Store.Add(StandardColumn.LogEvent);

        // ← custom enriched columns
        columnOptions.AdditionalColumns = new Collection<SqlColumn>
        {
            new() { ColumnName = "CorrelationId", DataType = SqlDbType.NVarChar, DataLength = 100,  AllowNull = true },
            new() { ColumnName = "UserId",        DataType = SqlDbType.NVarChar, DataLength = 450,  AllowNull = true },
            new() { ColumnName = "UserName",      DataType = SqlDbType.NVarChar, DataLength = 256,  AllowNull = true },
            new() { ColumnName = "ClientIp",      DataType = SqlDbType.NVarChar, DataLength = 50,   AllowNull = true },
            new() { ColumnName = "UserAgent",     DataType = SqlDbType.NVarChar, DataLength = 500,  AllowNull = true },
            new() { ColumnName = "RequestPath",   DataType = SqlDbType.NVarChar, DataLength = 500,  AllowNull = true },
            new() { ColumnName = "HttpMethod",    DataType = SqlDbType.NVarChar, DataLength = 10,   AllowNull = true },
            new() { ColumnName = "StatusCode",    DataType = SqlDbType.Int,                         AllowNull = true },
            new() { ColumnName = "DurationMs",    DataType = SqlDbType.BigInt,                      AllowNull = true },
            new() { ColumnName = "MachineName",   DataType = SqlDbType.NVarChar, DataLength = 100,  AllowNull = true },
            new() { ColumnName = "Environment",   DataType = SqlDbType.NVarChar, DataLength = 50,   AllowNull = true },
        };

        columnOptions.TimeStamp.ConvertToUtc = true;

        return columnOptions;
    }
}