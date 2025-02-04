using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core.Views;
using Harmonize.Client;
using Harmonize.Log;
using Harmonize.Model;
using Harmonize.Page.View;
using Harmonize.Service;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;
using MediaManager = Harmonize.Service.MediaManager;
using static Harmonize.Constants;
using Harmonize.Kodi;
using Harmonize.Components;

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

            builder.Services.AddSingleton<ILoggerProvider>(new FileLoggerProvider(LogFilePath));

            builder.Services.AddSingleton<AlertService>();
            builder.Services.AddSingleton<FailsafeService>();

            builder.Services.AddSingleton<HarmonizeClient>();
            builder.Services.AddSingleton<HarmonizeDatabase>();
            builder.Services.AddSingleton<KodiClient>();
            builder.Services.AddSingleton((services) =>
            {
                var kodiClient = services.GetService<KodiClient>() ?? throw new NullReferenceException(nameof(KodiClient));
                var harmonizeClient = services.GetService<HarmonizeClient>() ?? throw new NullReferenceException(nameof(HarmonizeClient));

                var preferenceManager = new PreferenceManager(harmonizeClient, kodiClient);

                var userSettings = preferenceManager.UserSettings;

                preferenceManager.SetUserSetttings(userSettings);

                return preferenceManager;
            });

            builder.Services.AddSingleton<MediaElementViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<HomePageViewModel>();
            builder.Services.AddSingleton<JobListViewModel>();
            builder.Services.AddSingleton<EditJobViewModel>();
            builder.Services.AddSingleton<YouTubeSearchViewModel>();
            builder.Services.AddSingleton<YouTubeSearchResultEditViewModel>();
            builder.Services.AddSingleton<YouTubePlaylistSearchResultEditViewModel>();
            builder.Services.AddSingleton<MagnetLinkSearchViewModel>();
            builder.Services.AddSingleton<ManageQbtViewModel>();
            builder.Services.AddSingleton<LogViewModel>();
            builder.Services.AddSingleton<MediaControlViewModel>();
            builder.Services.AddSingleton<AudioLibraryViewModel>();
            builder.Services.AddSingleton<BottomMenuViewModel>();
            builder.Services.AddSingleton<VideoLibraryViewModel>();
            builder.Services.AddSingleton<EditMediaEntryViewModel>();
            builder.Services.AddSingleton<HealthViewModel>();
            builder.Services.AddSingleton<TransferListViewModel>();
            builder.Services.AddSingleton<SeasonLibraryViewModel>();
            builder.Services.AddSingleton<EditSeasonViewModel>();
            builder.Services.AddSingleton<AddToSeasonViewModel>();

            builder.Services.AddSingleton<MediaElementPage>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<JobListPage>();
            builder.Services.AddSingleton<EditJobPage>();
            builder.Services.AddSingleton<YouTubeSearchPage>();
            builder.Services.AddSingleton<YouTubeSearchResultEditPage>();
            builder.Services.AddSingleton<YouTubePlaylistSearchResultEditPage>();
            builder.Services.AddSingleton<MagnetLinkSearchPage>();
            builder.Services.AddSingleton<ManageQbtPage>();
            builder.Services.AddSingleton<LogPage>();
            builder.Services.AddSingleton<MediaControlPage>();
            builder.Services.AddSingleton<AudioLibraryPage>();
            builder.Services.AddSingleton<VideoLibraryPage>();
            builder.Services.AddSingleton<EditMediaEntryPage>();
            builder.Services.AddSingleton<HealthPage>();
            builder.Services.AddSingleton<TransferListPage>();
            builder.Services.AddSingleton<SeasonLibraryPage>();
            builder.Services.AddSingleton<CreateSeasonPopup>();
            builder.Services.AddSingleton<EditSeasonPage>();

            builder.Services.AddSingleton<MediaManager>();

            return builder.Build();
        }
    }
}
