using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using CommunityToolkit.Maui.Views;
using Harmonize.Client.Model.Media;


namespace Harmonize;
public partial class MainPage : ContentPage
{
    const int PORT = 8000;
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        return;
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

            var mediaMetadata = await response.Content.ReadFromJsonAsync<MediaMetadata>();

            if (mediaMetadata != null)
            {
                //Debug.WriteLine(mediaMetadata.ToJson());
                string trackURL = $"http://{domainName}:{PORT}/stream/{fileName}";

                string xlMediaUrl = $"http://{domainName}:{PORT}/{mediaMetadata.Artwork.Xl}";
                Debug.WriteLine(xlMediaUrl);

                PlaybackMediaElement.ShouldShowPlaybackControls = true;
                PlaybackMediaElement.MetadataArtist = mediaMetadata.Artist;
                PlaybackMediaElement.MetadataTitle = mediaMetadata.Title;
                PlaybackMediaElement.MetadataArtworkUrl = xlMediaUrl;
                PlaybackMediaElement.Source = MediaSource.FromUri(trackURL);

                //await DisplayAlert("Success", $"{mediaMetadata.ToJson()}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred while downloading the file: {ex.Message}", "OK");
        }
    }
    //private async Task GetPlaylist(string playListName)
    //{
    //    var domainName = PreferenceManager.GetDomainName();
    //    var port = 8000;

    //    var client = new HarmonizeClient(domainName, port);

    //    var playlist = await client.GetPlaylist(playListName);

    //    using var httpClient = new HttpClient();
    //    var metadataUrl = $"http://{domainName}:{PORT}/metadata/media/{fileName}";
    //    var response = await httpClient.GetAsync(metadataUrl);
    //    response.EnsureSuccessStatusCode();

    //    MediaMetaData? mediaMetadata = await response.Content.ReadFromJsonAsync<MediaMetaData>();

    //    if (mediaMetadata != null)
    //    {
    //        Debug.WriteLine(mediaMetadata.ToJson());
    //        string trackURL = $"http://{domainName}:{PORT}/stream/{fileName}";

    //        string xlMediaUrl = $"http://{domainName}:{PORT}/{mediaMetadata.Artwork.Xl}";
    //        Debug.WriteLine(xlMediaUrl);

    //        PlaybackMediaElement.ShouldShowPlaybackControls = true;
    //        PlaybackMediaElement.MetadataArtist = mediaMetadata.Artist;
    //        PlaybackMediaElement.MetadataTitle = mediaMetadata.Title;
    //        PlaybackMediaElement.MetadataArtworkUrl = xlMediaUrl;
    //        PlaybackMediaElement.Source = MediaSource.FromUri(trackURL);

    //        await DisplayAlert("Success", $"{mediaMetadata.ToJson()}", "OK");
    //    }
    //}

}
