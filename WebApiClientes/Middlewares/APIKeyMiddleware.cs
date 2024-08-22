using System.Text;
using System.Text.Json;

namespace WebApiClientes.Middlewares
{
    public class Error
    {
        public string _message { get; set; }

        public Error(string message)
        {
            _message = message;
        }
    }

    public class APIKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY = "API-Key";
        private const string _key = "123456";

        public APIKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(!context.Request.Headers.TryGetValue(APIKEY, out var key))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new Error("Não foi fornecida uma chave de API válida!")), Encoding.UTF8);
                return;
            }

            if (!key.Equals(_key))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new Error("Foi fornecida uma chave de API inválida!")), Encoding.UTF8);
                return;
            }

            await _next(context);
        }
    }

    public static class ApiKeyMiddleware
    {
        public static void UseApiKeyMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<APIKeyMiddleware>();
        }
    }
}
