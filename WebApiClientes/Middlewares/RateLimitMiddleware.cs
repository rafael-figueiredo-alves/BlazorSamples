using WebApiClientes.Services.Interfaces;
using WebApiClientes.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace WebApiClientes.Middlewares
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY = "API-Key";

        private readonly IApiKeys fapiKeys = new ApiKeysBD();
        private readonly IDistributedCache _cache;

        public RateLimitMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEY, out var key))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new Error("Não foi fornecida uma chave de API válida!")), Encoding.UTF8);
                return;
            }

            if (!await fapiKeys.isValidApiKey(key))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new Error("Foi fornecida uma chave de API inválida!")), Encoding.UTF8);
                return;
            }

            if(await fapiKeys.GetRequestLimit(key) == 0)
            {
                //Não há limite de requesições por segundo
                await _next(context);
                return;
            }
        }

    }

    public class DadosConsumo
    {
        public DateTime LastResponse { get; private set; }
        public int NumberOfRequests { get; private set; }

        public DadosConsumo(DateTime lastResponse, int numberOfRequests)
        {
            LastResponse = lastResponse;
            NumberOfRequests = numberOfRequests;
        }

        public bool HasConsumedAllRequests(int timeWindowInSeconds, int maxRequests)
            => DateTime.UtcNow < LastResponse.AddSeconds(timeWindowInSeconds) && NumberOfRequests == maxRequests;

        public void IncreaseRequests(int maxRequests)
        {
            LastResponse = DateTime.UtcNow;

            if (NumberOfRequests == maxRequests)
                NumberOfRequests = 1;

            else
                NumberOfRequests++;
        }
    }
}
