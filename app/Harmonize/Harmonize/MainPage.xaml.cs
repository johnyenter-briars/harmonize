using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Web;
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
        public required Artwork Artwork { get; set; }
    }

    public partial class Artwork
    {
        [JsonProperty("xl")]
        public required string Xl { get; set; }

        [JsonProperty("large")]
        public required string Large { get; set; }

        [JsonProperty("small")]
        public required string Small { get; set; }
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
                using var httpClient = new HttpClient();
                var metadataUrl = $"http://{domainName}:{PORT}/metadata/media/{fileName}";
                var response = await httpClient.GetAsync(metadataUrl);
                response.EnsureSuccessStatusCode();

                MediaMetaData? mediaMetadata = await response.Content.ReadFromJsonAsync<MediaMetaData>();

                if (mediaMetadata != null)
                {
                    Console.WriteLine(mediaMetadata.ToJson());
                    string trackURL = $"http://{domainName}:{PORT}/stream/{fileName}";

                    string xlMediaUrl = $"http://{domainName}:{PORT}/{mediaMetadata.Artwork.Xl}";
                    Console.WriteLine(xlMediaUrl);

                    PlaybackMediaElement.ShouldShowPlaybackControls = true;
                    PlaybackMediaElement.MetadataArtist = mediaMetadata.Artist;
                    PlaybackMediaElement.MetadataTitle = mediaMetadata.Title;
                    PlaybackMediaElement.MetadataArtworkUrl = xlMediaUrl;
                    PlaybackMediaElement.Source = MediaSource.FromUri(trackURL);

                    await DisplayAlert("Success", $"{mediaMetadata.ToJson()}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while downloading the file: {ex.Message}", "OK");
            }
        }
    }

}
