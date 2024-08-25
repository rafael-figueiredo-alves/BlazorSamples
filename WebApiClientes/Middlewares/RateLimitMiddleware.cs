using WebApiClientes.Services.Interfaces;
using WebApiClientes.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using BlazorClientes.Shared.Entities;

namespace WebApiClientes.Middlewares
{
    /// <summary>
    /// Middleware para controlar limite de requesições por segundo
    /// </summary>
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY = "API-Key";

        private readonly IApiKeys fapiKeys = new ApiKeysBD();
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Construtor do Middleware
        /// </summary>
        /// <param name="next">Próximo middleware</param>
        /// <param name="cache">Serviço de Cache</param>
        public RateLimitMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        /// <summary>
        /// Método de execução do middleware
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>Nada</returns>
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
