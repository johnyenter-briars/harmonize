using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core.Views;
using Harmonize.Client;
using Harmonize.Model;
using Harmonize.Page.View;
using Harmonize.Service;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;
using MediaManager = Harmonize.Service.MediaManager;

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

            builder.Services.AddSingleton<PreferenceManager>();
            builder.Services.AddSingleton<HarmonizeDatabase>();

            builder.Services.AddSingleton<MediaElementViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<MediaListViewModel>();

            builder.Services.AddSingleton<MediaElementPage>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<MediaListPage>();

            builder.Services.AddSingleton(service =>
            {
                var preferenceManager = service.GetService<PreferenceManager>() ?? throw new NullReferenceException($"{nameof(PreferenceManager)} is not registered");

                var userSettings = preferenceManager.UserSettings;

                var client = new HarmonizeClient(userSettings.DomainName, userSettings.Port);

                return client;
            });

            builder.Services.AddSingleton<MediaManager>();

            return builder.Build();
        }
    }
}
