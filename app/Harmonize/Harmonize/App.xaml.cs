using Harmonize.Service;

namespace Harmonize
{
    public partial class App : Application
    {
        public App(PreferenceManager preferenceManager)
        {
            InitializeComponent();

            MainPage = new AppShell(preferenceManager);
        }
    }
}
