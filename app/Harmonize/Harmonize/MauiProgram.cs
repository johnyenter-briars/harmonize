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
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif


            builder.Services.AddSingleton<AlertService>();
            builder.Services.AddSingleton<FailsafeService>();

            builder.Services.AddSingleton<PreferenceManager>();
            builder.Services.AddSingleton<HarmonizeDatabase>();

            builder.Services.AddSingleton<MediaElementViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<MediaListViewModel>();
            builder.Services.AddSingleton<HomePageViewModel>();
            builder.Services.AddSingleton<JobListViewModel>();
            builder.Services.AddSingleton<EditJobViewModel>();
            builder.Services.AddSingleton<YouTubeSearchViewModel>();
            builder.Services.AddSingleton<YouTubeSearchResultEditViewModel>();
            builder.Services.AddSingleton<YouTubePlaylistSearchResultEditViewModel>();
            builder.Services.AddSingleton<MagnetLinkSearchViewModel>();
            builder.Services.AddSingleton<ManageQbtViewModel>();

            builder.Services.AddSingleton<MediaElementPage>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<MediaListPage>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<JobListPage>();
            builder.Services.AddSingleton<EditJobPage>();
            builder.Services.AddSingleton<YouTubeSearchPage>();
            builder.Services.AddSingleton<YouTubeSearchResultEditPage>();
            builder.Services.AddSingleton<YouTubePlaylistSearchResultEditPage>();
            builder.Services.AddSingleton<MagnetLinkSearchPage>();
            builder.Services.AddSingleton<ManageQbtPage>();

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
