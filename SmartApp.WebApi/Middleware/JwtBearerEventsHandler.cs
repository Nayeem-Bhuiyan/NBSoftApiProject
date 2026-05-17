using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SmartApp.Shared.Common;
using System.Net;
using System.Text.Json;

namespace SmartApp.WebApi.Middleware
{
    public static class JwtBearerEventsHandler
    {
        public static JwtBearerEvents BuildEvents() => new JwtBearerEvents
        {
            OnChallenge = HandleChallengeAsync,
            OnForbidden = HandleForbiddenAsync
        };

        private static async Task HandleChallengeAsync(JwtBearerChallengeContext context)
        {
            context.HandleResponse(); // suppress default WWW-Authenticate header behavior

            context.Response.StatusCode  = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            var response = Response<string>.Failure("Authentication required. Please provide a valid token.");
            await WriteJsonAsync(context.Response, response);
        }

        private static async Task HandleForbiddenAsync(ForbiddenContext context)
        {
            context.Response.StatusCode  = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";

            var response = Response<string>.Failure("You do not have permission to access this resource.");
            await WriteJsonAsync(context.Response, response);
        }

        private static async Task WriteJsonAsync<T>(HttpResponse httpResponse, Response<T> payload)
        {
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await httpResponse.WriteAsync(json);
        }
    }
}