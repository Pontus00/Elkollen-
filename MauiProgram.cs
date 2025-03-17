using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Elkollen.Pages;
using Elkollen.Services;
using Elkollen.ViewModels;


namespace Elkollen
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            var hostBuilder = new HostBuilder();

            hostBuilder.ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;

                config.AddJsonFile("Resources/appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"Resources/appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            });

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiCommunityToolkit();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<PricesViewModel>();
            builder.Services.AddSingleton<PricesPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}