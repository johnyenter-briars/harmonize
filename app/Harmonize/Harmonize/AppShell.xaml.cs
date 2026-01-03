using Harmonize.Page.View;
using Harmonize.Service;

namespace Harmonize
{
    public partial class AppShell : Shell
    {
        public AppShell(PreferenceManager preferenceManager)
        {
            InitializeComponent();

            var defaultPage = preferenceManager.UserSettings.DefaultPageOnLaunch;

            if (Items.Any(page => page.Title == defaultPage))
            {
                CurrentItem = Items.FirstOrDefault(item => item.Title == defaultPage);
            }

            Routing.RegisterRoute(nameof(EditJobPage), typeof(EditJobPage));
            Routing.RegisterRoute(nameof(YouTubeSearchResultEditPage), typeof(YouTubeSearchResultEditPage));
            Routing.RegisterRoute(nameof(YouTubePlaylistSearchResultEditPage), typeof(YouTubePlaylistSearchResultEditPage));
            Routing.RegisterRoute(nameof(MediaElementPage), typeof(MediaElementPage));
            Routing.RegisterRoute(nameof(EditMediaEntryPage), typeof(EditMediaEntryPage));
            Routing.RegisterRoute(nameof(EditSeasonPage), typeof(EditSeasonPage));
            Routing.RegisterRoute(nameof(TvcControlPage), typeof(TvcControlPage));
            Routing.RegisterRoute(nameof(MediaControlPage), typeof(MediaControlPage));

            if (!preferenceManager.UserSettings.IncludeMediaControlPage)
            {
                var mediaControlPage = Items.FirstOrDefault(item => item.Title == "Media Control");

                if (mediaControlPage is not null)
                {
                    mediaControlPage.IsVisible = false;
                }
            }

            if (!preferenceManager.UserSettings.IncludeTvcControlPage)
            {
                var tvcControlPage = Items.FirstOrDefault(item => item.Title == "TVC Control");

                if (tvcControlPage is not null)
                {
                    tvcControlPage.IsVisible = false;
                }
            }
        }
    }
}
