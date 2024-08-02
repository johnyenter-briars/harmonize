namespace Harmonize
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            CurrentItem = Items.FirstOrDefault(item => item.Title == "Media List");
        }
    }
}
