# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:11.0-preview AS build
WORKDIR /src

COPY *.sln ./

COPY SmartApp.WebApi/*.csproj SmartApp.WebApi/
COPY SmartApp.Application/*.csproj SmartApp.Application/
COPY SmartApp.Domain/*.csproj SmartApp.Domain/
COPY SmartApp.Infrastructure/*.csproj SmartApp.Infrastructure/
COPY SmartApp.Persistence/*.csproj SmartApp.Persistence/
COPY SmartApp.Shared/*.csproj SmartApp.Shared/

RUN dotnet restore SmartApp.sln

COPY . .

WORKDIR /src/SmartApp.WebApi
RUN dotnet publish -c Release -o /app/publish --no-restore

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:11.0-preview
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
RUN mkdir -p /app/wwwroot && chown -R appuser:appuser /app/wwwroot
RUN useradd -m appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "SmartApp.WebApi.dll"]