using CommunityToolkit.Maui;
using Harmonize.Client;
using Harmonize.Page.View;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;

namespace Harmonize
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<MediaElementViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();

            builder.Services.AddSingleton<MediaElementPage>();
            builder.Services.AddSingleton<SettingsPage>();

            builder.Services.AddSingleton(service =>
            {
                var domainName = PreferenceManager.GetDomainName();
                var port = 8000;

                var client = new HarmonizeClient(domainName, port);

                return client;
            });

            return builder.Build();
        }
    }
}
