using Microsoft.Extensions.Logging;
using Refit;

namespace mobileFitnes
{
    using ApiService;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static class MauiProgram
    {
        public const string BaseUrl = "http://172.20.112.1:2137/api";
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.Services.AddTransient<RefitInterceptor>();
            builder.Services.AddSingleton<ClassesInfo>();


            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
            builder.Services.AddRefitClient<IApiEndpoints>(new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(serializerOptions)
            })
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(BaseUrl);
            })
            .AddHttpMessageHandler<RefitInterceptor>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            ServiceProvider = app.Services;
            return app;
        }
    }
}
