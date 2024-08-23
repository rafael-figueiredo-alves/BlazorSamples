using System.Text;
using System.Text.Json;
using WebApiClientes.Services;
using WebApiClientes.Services.Interfaces;

namespace WebApiClientes.Middlewares
{
    /// <summary>
    /// Classe para formatar JSON de erro
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Propriedade com a mensagem sobre o erro
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Método construtor da classe de erros
        /// </summary>
        /// <param name="message">Mensagem a reportar</param>
        public Error(string message)
        {
            Message = message;
        }
    }

    /// <summary>
    /// Middleware para informar e validar a API Key passada
    /// </summary>
    public class APIKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY = "API-Key";
        
        private readonly IApiKeys fapiKeys = new ApiKeysBD();

        /// <summary>
        /// Método Construtor
        /// </summary>
        /// <param name="next">Próximo Middleware</param>
        public APIKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Método para invocar função do middleware
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if(!context.Request.Headers.TryGetValue(APIKEY, out var key))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new Error("Não foi fornecida uma chave de API válida!")), Encoding.UTF8);
                return;
            }

            if(!await fapiKeys.isValidApiKey(key))  
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new Error("Foi fornecida uma chave de API inválida!")), Encoding.UTF8);
                return;
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Classe de extensão para o Middleware
    /// </summary>
    public static class ApiKeyMiddleware
    {
        /// <summary>
        /// Método para adicionar o middleware
        /// </summary>
        /// <param name="app"></param>
        public static void UseApiKeyMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<APIKeyMiddleware>();
        }
    }
}
