using System.Globalization;
using System.Net;
using System.Net.Sockets;
using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Harmonize
{
    public partial class MediaMetaData
    {
        [JsonProperty("title")]
        public required string Title { get; set; }

        [JsonProperty("artist")]
        public required string Artist { get; set; }

        [JsonProperty("album")]
        public required string Album { get; set; }

        [JsonProperty("artwork")]
        public required Artwork[] Artwork { get; set; }
    }

    public partial class Artwork
    {
        [JsonProperty("src")]
        public required Uri Src { get; set; }

        [JsonProperty("sizes")]
        public required string Sizes { get; set; }

        [JsonProperty("type")]
        public required string Type { get; set; }
        [JsonProperty("name")]
        public required string Name { get; set; }
    }

    public partial class MediaMetaData
    {
        public static MediaMetaData FromJson(string json) => JsonConvert.DeserializeObject<MediaMetaData>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this MediaMetaData self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    public partial class MainPage : ContentPage
    {
        const int PORT = 8000;
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await GetMusicAndMetadataAsync("Sense.mp3");
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private async Task GetMusicAndMetadataAsync(string fileName)
        {
            var domainName = PreferenceManager.GetDomainName();
            try
            {
                var foo = $"http://{domainName}:{PORT}/metadata/media/{fileName}";
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(foo);
                response.EnsureSuccessStatusCode();

                string? responseStr = await response.Content.ReadAsStringAsync();
                if (responseStr != null)
                {
                    string trackURL = $"http://{domainName}:{PORT}/stream/{fileName}";
                    var mediaMetadata = MediaMetaData.FromJson(responseStr);

                    var bar = $"http://{domainName}/{mediaMetadata.Artwork[0].Src}";

                    TestMediaElement.ShouldShowPlaybackControls = true;
                    TestMediaElement.MetadataArtist = mediaMetadata.Artist;
                    TestMediaElement.MetadataTitle = mediaMetadata.Title;
                    TestMediaElement.MetadataArtworkUrl = $"http://{domainName}:{PORT}/{mediaMetadata.Artwork[0].Src}";
                    TestMediaElement.Source = MediaSource.FromUri(trackURL);

                    await DisplayAlert("Success", $"{responseStr}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while downloading the file: {ex.Message}", "OK");
            }
        }
    }

}
