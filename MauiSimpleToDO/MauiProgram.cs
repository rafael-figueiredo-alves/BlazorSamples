using Blazored.Toast;
using MauiSimpleToDO.Services;
using Microsoft.Extensions.Logging;

namespace MauiSimpleToDO
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
            builder.Services.AddScoped<ITarefas, Tarefas>();

            builder.Services.AddBlazoredToast();

            return builder.Build();
        }
    }
}