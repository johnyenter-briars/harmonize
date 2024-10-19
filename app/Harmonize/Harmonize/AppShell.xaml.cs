using Harmonize.Client.Model.Media;
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

            CurrentItem = Items.FirstOrDefault(item => item.Title == defaultPage);
            
            Routing.RegisterRoute(nameof(EditJobPage), typeof(EditJobPage));
            Routing.RegisterRoute(nameof(YouTubeSearchResultEditPage), typeof(YouTubeSearchResultEditPage));
            Routing.RegisterRoute(nameof(YouTubePlaylistSearchResultEditPage), typeof(YouTubePlaylistSearchResultEditPage));
            Routing.RegisterRoute(nameof(MediaElementPage), typeof(MediaElementPage));
        }
    }
}
