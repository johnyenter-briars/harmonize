using CommunityToolkit.Maui;
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

            return builder.Build();
        }
    }
}
