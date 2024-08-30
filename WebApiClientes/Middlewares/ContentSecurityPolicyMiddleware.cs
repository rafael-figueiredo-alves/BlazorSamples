namespace WebApiClientes.Middlewares
{
    public class ContentSecurityPolicyMiddleware
    {
        private readonly RequestDelegate _next;
        public ContentSecurityPolicyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self'");
            await _next(context);
        }
    }

    public static class CSPMiddleware
    {
        /// <summary>
        /// Método para adicionar o middleware
        /// </summary>
        /// <param name="app"></param>
        public static void UseContentSecurityPolicyMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ContentSecurityPolicyMiddleware>();
        }
    }
}
