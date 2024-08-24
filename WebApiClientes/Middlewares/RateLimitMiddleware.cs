using WebApiClientes.Services.Interfaces;
using WebApiClientes.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using BlazorClientes.Shared.Entities

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
            if (context.Request.Headers.TryGetValue(APIKEY, out var key))
            {
                var Limite = await fapiKeys.GetRequestLimit(key);

                if (await fapiKeys.GetRequestLimit(key) == 0)
                {
                    //Não há limite de requesições por segundo
                    await _next(context);
                    return;
                }
            }
        }

    }
}
