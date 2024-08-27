using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text;
using System.Text.Json;
using WebApiClientes.Helpers;
using WebApiClientes.Services;
using WebApiClientes.Services.Interfaces;

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

                if (Limite.MaxRequests == 0)
                {
                    //Não há limite de requesições por segundo
                    await _next(context);
                    return;
                }

                var dadosConsumo = await _cache.GetDadosConsumoAsync(key!);
                if (dadosConsumo is not null)
                {
                    if(dadosConsumo.AtingiuConsumo(Limite.TimeWindowInSeconds, Limite.MaxRequests))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new Error("O limite de requisições por 5 segundos foi atingido. É necessário aguardar para realizar novas chamadas a API.")), Encoding.UTF8);
                        return;
                    }

                    dadosConsumo.IncrementarRequesicoes(Limite.MaxRequests);
                }

                await _cache.SaveDadosConsumoAsync(key!, dadosConsumo);
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new Error("Não foi fornecida uma chave de API válida!")), Encoding.UTF8);
                return;
            }
        }
    }

    /// <summary>
    /// Classe de extensão para o Middleware
    /// </summary>
    public static class RateLimitingMiddleware
    {
        /// <summary>
        /// Método para adicionar o middleware
        /// </summary>
        /// <param name="app"></param>
        public static void UseRateLimitMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<RateLimitMiddleware>();
        }
    }
}
