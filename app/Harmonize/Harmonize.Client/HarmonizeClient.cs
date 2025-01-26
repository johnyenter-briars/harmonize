using Harmonize.Client.Model.Job;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.QBT;
using Harmonize.Client.Model.Response;
using Harmonize.Client.Model.Transfer;
using Harmonize.Client.Model.Youtube;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Harmonize.Client.Utility.Utility;

namespace Harmonize.Client;

public class HarmonizeClient
{
    private string? hostName;
    private int? port;
    private static readonly JsonSerializerOptions SnakeCaseOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };
    private static readonly JsonSerializerOptions CamelCaseOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };
    private static HttpClientHandler handler = new()
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    private static readonly HttpClient httpClient = new HttpClient(handler)
    {
        Timeout = new TimeSpan(0, 1, 100),
    };
    #region Utilities
    public HarmonizeClient SetPort(int port)
    {
        this.port = port;
        return this;
    }
    public HarmonizeClient SetHostName(string hostName)
    {
        this.hostName = hostName;
        return this;
    }
    #endregion
    #region GET
    public string GetMediaMetadataArtworkUrl(MediaMetadata mediaMetadata, string artworkSize)
    {
        var artwork = mediaMetadata.Artwork;

        var xlMediaUrl = $"http://{hostName}:{port}/api/{mediaMetadata.Artwork}";

        return xlMediaUrl;
    }
    public async Task<MediaEntriesResponse> GetAudio()
    {
        return await HarmonizeRequest<MediaEntriesResponse>($"media/audio", HttpMethod.Get);
    }
    public async Task<MediaEntriesResponse> GetVideo()
    {
        return await HarmonizeRequest<MediaEntriesResponse>($"media/video", HttpMethod.Get);
    }
    public async Task<byte[]> GetMediaBytes(IMediaEntry mediaEntry)
    {
        return await HarmonizeRequestBytes($"stream/{mediaEntry.Id}", HttpMethod.Get);
    }
    public async Task<Playlist> GetPlaylist(string playlistName)
    {
        return await HarmonizeRequest<Playlist>($"playlist/{playlistName}", HttpMethod.Get);
    }
    public async Task<MediaMetadataResponse> GetMediaMetadata(IMediaEntry mediaEntry)
    {
        return await HarmonizeRequest<MediaMetadataResponse>($"metadata/media/{mediaEntry.Id}", HttpMethod.Get);
    }
    public async Task<JobsResponse> GetJobs()
    {
        return await HarmonizeRequest<JobsResponse>($"job", HttpMethod.Get);
    }
    public async Task<JobResponse> GetJob(Guid jobId)
    {
        return await HarmonizeRequest<JobResponse>($"job/{jobId}", HttpMethod.Get);
    }
    public async Task<YoutubeVideoSearchResponse> GetYoutubeVideoSearchResults(string query)
    {
        return await HarmonizeRequest<YoutubeVideoSearchResponse>($"search/youtube/video/{query}", HttpMethod.Get);
    }
    public async Task<YoutubePlaylistSearchResponse> GetYoutubePlaylistSearchResults(string query)
    {
        return await HarmonizeRequest<YoutubePlaylistSearchResponse>($"search/youtube/playlist/{query}", HttpMethod.Get);
    }
    public async Task<MagnetLinkSearchResults> GetPiratebaySearchResults(string query)
    {
        return await HarmonizeRequest<MagnetLinkSearchResults>($"search/piratebay/{query}", HttpMethod.Get);
    }
    public async Task<MagnetLinkSearchResults> GetXT1337SearchResults(string query)
    {
        return await HarmonizeRequest<MagnetLinkSearchResults>($"search/xt1337/{query}", HttpMethod.Get);
    }
    public async Task<TransferProgressResponse> GetTransferProgress(TransferDestination transferDestination)
    {
        var destination = transferDestination.ToString().ToLower();
        return await HarmonizeRequest<TransferProgressResponse>($"transfer/{destination}", HttpMethod.Get);
    }
    public async Task<JobResponse> StartTransfer(TransferDestination transferDestination, IMediaEntry entry) 
    {
        var destination = transferDestination.ToString().ToLower();
        return await HarmonizeRequest<JobResponse>($"transfer/{destination}/{entry.Id}", HttpMethod.Post);
    }
    #endregion
    #region POST
    public async Task<BaseResponse<Job>> CancelJob(Guid jobId)
    {
        return await HarmonizeRequest<BaseResponse<Job>>($"job/cancel/{jobId}", HttpMethod.Post);
    }
    public async Task<BaseResponse<Job>> DownloadYoutubeVideo(string youtubeId)
    {
        return await HarmonizeRequest<BaseResponse<Job>>($"youtube/video/{youtubeId}", HttpMethod.Post);
    }
    public async Task<BaseResponse<Job>> DownloadYoutubePlaylist(string youtubeId)
    {
        return await HarmonizeRequest<BaseResponse<Job>>($"youtube/playlist/{youtubeId}", HttpMethod.Post);
    }
    public async Task<AddQbtDownloadsResponse> AddQbtDownload(AddQbtDownloadsRequest request)
    {
        return await HarmonizeRequest<AddQbtDownloadsRequest, AddQbtDownloadsResponse>(request, $"qbt/add", HttpMethod.Post);
    }
    public async Task<ListQbtDownloadsResponse> GetQbtDownloads()
    {
        return await HarmonizeRequest<ListQbtDownloadsResponse>($"qbt/list", HttpMethod.Get);
    }
    public async Task<DeleteDownloadsResponse> DeleteQbtDownloads(DeleteDownloadsRequest request)
    {
        return await HarmonizeRequest<DeleteDownloadsRequest, DeleteDownloadsResponse>(request, $"qbt/delete", HttpMethod.Post);
    }
    public async Task<PauseDownloadsResponse> PauseQbtDownloads(PauseDownloadsRequest request)
    {
        return await HarmonizeRequest<PauseDownloadsRequest, PauseDownloadsResponse>(request, $"qbt/pause", HttpMethod.Post);
    }
    public async Task<ResumeDownloadsResponse> ResumeQbtDownloads(ResumeDownloadsRequest request)
    {
        return await HarmonizeRequest<ResumeDownloadsRequest, ResumeDownloadsResponse>(request, $"qbt/resume", HttpMethod.Post);
    }
    #endregion
    #region BASE REQUESTS
    private async Task<TResponse> HarmonizeRequest<TResponse>(string path, HttpMethod httpMethod)
    {
        var request = new HttpRequestMessage(httpMethod, $"http://{hostName}:{port}/api/" + path);
        request.Headers.Accept.Clear();
        //request.Headers.Add("x-api-key", _apiKey);
        //request.Headers.Add("x-user-id", _userId);

        return await SendRequest<TResponse>(request, CamelCaseOptions);
    }
    private async Task<byte[]> HarmonizeRequestBytes(string path, HttpMethod httpMethod)
    {
        var request = new HttpRequestMessage(httpMethod, $"http://{hostName}:{port}/api/" + path);
        request.Headers.Accept.Clear();
        //request.Headers.Add("x-api-key", _apiKey);
        //request.Headers.Add("x-user-id", _userId);

        return await SendRequestBytes(request);
    }
    private async Task<TResponse> HarmonizeRequest<TRequest, TResponse>(TRequest requestObject, string path, HttpMethod httpMethod)
    {
        var request = new HttpRequestMessage(httpMethod, $"http://{hostName}:{port}/api/" + path);
        request.Headers.Accept.Clear();
        //request.Headers.Add("x-api-key", _apiKey);
        //request.Headers.Add("x-user-id", _userId);

        var str = JsonSerializer.Serialize(requestObject, CamelCaseOptions);
        request.Content = new StringContent(str, Encoding.UTF8, "application/json");

        return await SendRequest<TResponse>(request, CamelCaseOptions);
    }
    //TODO: return a monad
    private async Task<TResponse> SendRequest<TResponse>(HttpRequestMessage request, JsonSerializerOptions settings)
    {
        var clientResponse = await httpClient.SendAsync(request, CancellationToken.None);

        clientResponse.EnsureSuccessStatusCode();

        var str = await clientResponse.Content.ReadAsStringAsync();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
        return await JsonSerializer.DeserializeAsync<TResponse>(stream, settings) ?? throw new NullReferenceException(nameof(JsonSerializer.DeserializeAsync));
    }
    private async Task<byte[]> SendRequestBytes(HttpRequestMessage request)
    {
        var clientResponse = await httpClient.SendAsync(request, CancellationToken.None);

        clientResponse.EnsureSuccessStatusCode();

        var fileBytes = await clientResponse.Content.ReadAsByteArrayAsync();

        return fileBytes;
    }
    #endregion
}
