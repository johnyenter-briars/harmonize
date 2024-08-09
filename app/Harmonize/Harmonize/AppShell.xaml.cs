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
        }
    }
}
