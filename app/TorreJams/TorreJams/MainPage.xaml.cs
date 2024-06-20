namespace TorreJams
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            DownloadFileAsync("http://192.168.0.3:8000/stream/Sense.mp3");
        }

        private async Task DownloadFileAsync(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var fileName = "downloaded_file.mp3"; // You can extract this from the URL or response headers
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fs);
                }

                await DisplayAlert("Success", $"File downloaded to {filePath}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while downloading the file: {ex.Message}", "OK");
            }
        }
    }

}
