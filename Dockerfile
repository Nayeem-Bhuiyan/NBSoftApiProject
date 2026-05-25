# ═══════════════════════════════════════════════════════════════
# STAGE 1 — RESTORE
# ═══════════════════════════════════════════════════════════════
FROM mcr.microsoft.com/dotnet/sdk:11.0-preview AS restore

WORKDIR /src

COPY *.sln ./
COPY SmartApp.WebApi/*.csproj         SmartApp.WebApi/
COPY SmartApp.Application/*.csproj    SmartApp.Application/
COPY SmartApp.Domain/*.csproj         SmartApp.Domain/
COPY SmartApp.Infrastructure/*.csproj SmartApp.Infrastructure/
COPY SmartApp.Persistence/*.csproj    SmartApp.Persistence/
COPY SmartApp.Shared/*.csproj         SmartApp.Shared/

RUN dotnet restore SmartApp.sln --runtime linux-x64

# ═══════════════════════════════════════════════════════════════
# STAGE 2 — BUILD & PUBLISH
# ═══════════════════════════════════════════════════════════════
FROM restore AS build

WORKDIR /src
COPY . .
WORKDIR /src/SmartApp.WebApi

RUN dotnet publish \
    -c Release \
    -r linux-x64 \
    -o /app/publish \
    --no-restore \
    --self-contained false \
    /p:UseAppHost=false

# ═══════════════════════════════════════════════════════════════
# STAGE 3 — RUNTIME
# ═══════════════════════════════════════════════════════════════
FROM mcr.microsoft.com/dotnet/aspnet:11.0-preview AS final

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Create non-root user
RUN groupadd --system appgroup \
    && useradd --system --gid appgroup --no-create-home --shell /sbin/nologin appuser

# Create directories with correct permissions
RUN mkdir -p /app/wwwroot/UploadedImages \
             /app/wwwroot/UploadedFiles \
             /app/logs \
             /app/DataProtection-Keys \
    && chown -R appuser:appgroup /app

COPY --from=build --chown=appuser:appgroup /app/publish .

USER appuser

EXPOSE 8080

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DataProtection__Directory=/app/DataProtection-Keys \
    ASPNETCORE_DataProtection__Directory=/app/DataProtection-Keys

ENTRYPOINT ["dotnet", "SmartApp.WebApi.dll"]