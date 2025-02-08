using Harmonize.Client.Model.Health;
using Harmonize.Client.Model.Job;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Playlist;
using Harmonize.Client.Model.QBT;
using Harmonize.Client.Model.Response;
using Harmonize.Client.Model.Season;
using Harmonize.Client.Model.Transfer;
using Harmonize.Client.Model.Youtube;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Harmonize.Client;

public class HarmonizeClient
{
    private string? hostName;
    private int? port;
    private string? username;
    private string? password;

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
    public HarmonizeClient SetCredentials(string username, string password)
    {
        this.username = username;
        this.password = password;
        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
        return this;
    }

    #endregion

    #region Media
    public async Task<MediaEntryResponse > GetMediaEntry(Guid mediaEntryId)
    {
        return await HarmonizeRequest<MediaEntryResponse >($"media/{mediaEntryId}", HttpMethod.Get);
    }
    public async Task<MediaEntriesResponse> GetAudio()
    {
        return await HarmonizeRequest<MediaEntriesResponse>($"media/audio", HttpMethod.Get);
    }
    public async Task<MediaEntriesResponse> GetSubsPaging(int limit, int skip = 0)
    {
        return await HarmonizeRequest<MediaEntriesResponse>($"media/sub?limit={limit}&skip={skip}", HttpMethod.Get);
    }
    public async Task<MediaEntriesResponse> GetSubsForVideo(IMediaEntry mediaEntry)
    {
        if (mediaEntry.Type != MediaEntryType.Video) throw new ArgumentException("Media entry needs to be a Video.", nameof(mediaEntry));

        return await HarmonizeRequest<MediaEntriesResponse>($"media/video/{mediaEntry.Id}/sub", HttpMethod.Get);
    }
    public async Task<MediaEntriesResponse> GetVideosPaging(
          int limit,
          int skip = 0,
          string? nameSubString = null,
          IList<VideoType>? types = null
    )
    {
        var url = $"media/video?limit={limit}&skip={skip}";

        if (nameSubString is not null)
        {
            url += $"&name_sub_string={nameSubString}";
        }

        if (types is not null && types.Count > 0)
        {
            foreach (var type in types)
            {
                url += $"&type={(int)type}";
            }
        }
        else
        {
            foreach (var type in Enumerable.AsEnumerable([VideoType.Movie, VideoType.Episode]))
            {
                url += $"&type={(int)type}";
            }
        }

        return await HarmonizeRequest<MediaEntriesResponse>(url, HttpMethod.Get);
    }
    public async Task<MediaEntriesResponse> GetVideosPaging(string nameSubString, int limit, int skip = 0)
    {
        return await HarmonizeRequest<MediaEntriesResponse>($"media/video?limit={limit}&skip={skip}&name_sub_string={nameSubString}", HttpMethod.Get);
    }
    public async Task<byte[]> GetMediaBytes(IMediaEntry mediaEntry)
    {
        return await HarmonizeRequestBytes($"stream/{mediaEntry.Id}", HttpMethod.Get);
    }
    public string GetMediaMetadataArtworkUrl(MediaMetadata mediaMetadata, string artworkSize)
    {
        var artwork = mediaMetadata.Artwork;

        var xlMediaUrl = $"http://{hostName}:{port}/api/{mediaMetadata.Artwork}";

        return xlMediaUrl;
    }
    public async Task<MediaMetadataResponse> GetMediaMetadata(IMediaEntry mediaEntry)
    {
        return await HarmonizeRequest<MediaMetadataResponse>($"metadata/media/{mediaEntry.Id}", HttpMethod.Get);
    }
    //TODO: this is also horrible
    public async Task<MediaEntryResponse> UpdateEntry(MediaEntry entry, UpsertMediaEntryRequest request)
    {
        return await HarmonizeRequest<UpsertMediaEntryRequest, MediaEntryResponse>(request, $"media/{entry.Id}", HttpMethod.Put);
    }
    public async Task<MediaEntryResponse> DeleteEntry(MediaEntry entry)
    {
        return await HarmonizeRequest<MediaEntryResponse>($"media/{entry.Id}", HttpMethod.Delete);
    }
    #endregion

    #region Playlist
    public async Task<Playlist> GetPlaylist(string playlistName)
    {
        return await HarmonizeRequest<Playlist>($"playlist/{playlistName}", HttpMethod.Get);
    }
    #endregion

    #region Job
    public async Task<JobsResponse> GetJobs()
    {
        return await HarmonizeRequest<JobsResponse>($"job", HttpMethod.Get);
    }
    public async Task<JobResponse> GetJob(Guid jobId)
    {
        return await HarmonizeRequest<JobResponse>($"job/{jobId}", HttpMethod.Get);
    }
    public async Task<BaseResponse<Job>> CancelJob(Guid jobId)
    {
        return await HarmonizeRequest<BaseResponse<Job>>($"job/cancel/{jobId}", HttpMethod.Post);
    }
    #endregion

    #region Search
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
    #endregion 

    #region Transfer
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
    public async Task<JobResponse> Untransfer(TransferDestination transferDestination, IMediaEntry entry)
    {
        var destination = transferDestination.ToString().ToLower();
        return await HarmonizeRequest<JobResponse>($"transfer/{destination}/{entry.Id}/untransfer", HttpMethod.Post);
    }
    #endregion

    #region Health
    public async Task<HealthStatusResponse> GetHealthStatus()
    {
        return await HarmonizeRequest<HealthStatusResponse>($"health/status", HttpMethod.Get);
    }
    #endregion

    #region Season
    public async Task<SeasonsResponse> GetSeasonsPaging(int limit, int skip = 0)
    {
        return await HarmonizeRequest<SeasonsResponse>($"season?limit={limit}&skip={skip}", HttpMethod.Get);
    }
    public async Task<SeasonsResponse> GetSeasonsPaging(string nameSubString, int limit, int skip = 0)
    {
        return await HarmonizeRequest<SeasonsResponse>($"season?limit={limit}&skip={skip}&name_sub_string={nameSubString}", HttpMethod.Get);
    }
    public async Task<MediaEntriesResponse> GetSeasonEntries(Season season)
    {
        return await HarmonizeRequest<MediaEntriesResponse>($"season/{season.Id}", HttpMethod.Get);
    }
    public async Task<SeasonResponse> CreateSeason(UpsertSeasonRequest request)
    {
        return await HarmonizeRequest<UpsertSeasonRequest, SeasonResponse>(request, $"season/", HttpMethod.Post);
    }
    public async Task<SeasonResponse> AssociateToSeason(AssociateToSeasonRequest request)
    {
        return await HarmonizeRequest<AssociateToSeasonRequest, SeasonResponse>(request, $"season/associate", HttpMethod.Post);
    }
    public async Task<SeasonResponse> DisassociateToSeason(DisassociateToSeasonRequest request)
    {
        return await HarmonizeRequest<DisassociateToSeasonRequest, SeasonResponse>(request, $"season/associate", HttpMethod.Post);
    }
    //TODO: this is horrible
    public async Task<SeasonResponse> UpdateSeason(Season season, UpsertSeasonRequest request)
    {
        return await HarmonizeRequest<UpsertSeasonRequest, SeasonResponse>(request, $"season/{season.Id}", HttpMethod.Put);
    }
    public async Task<SeasonResponse> DeleteSeason(Season season)
    {
        return await HarmonizeRequest<SeasonResponse>($"season/{season.Id}", HttpMethod.Delete);
    }
    #endregion

    #region Youtube
    public async Task<BaseResponse<Job>> DownloadYoutubeVideo(string youtubeId)
    {
        return await HarmonizeRequest<BaseResponse<Job>>($"youtube/video/{youtubeId}", HttpMethod.Post);
    }
    public async Task<BaseResponse<Job>> DownloadYoutubePlaylist(string youtubeId)
    {
        return await HarmonizeRequest<BaseResponse<Job>>($"youtube/playlist/{youtubeId}", HttpMethod.Post);
    }
    #endregion

    #region QBT
    public async Task<AddQbtDownloadsResponse> AddQbtDownload(AddQbtDownloadsRequest request)
    {
        var optionsWithIntegers = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return await HarmonizeRequest<AddQbtDownloadsRequest, AddQbtDownloadsResponse>(request, $"qbt/add", HttpMethod.Post, optionsWithIntegers);
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
    private async Task<TResponse> HarmonizeRequest<TRequest, TResponse>(TRequest requestObject, string path, HttpMethod httpMethod, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var request = new HttpRequestMessage(httpMethod, $"http://{hostName}:{port}/api/" + path);
        request.Headers.Accept.Clear();
        //request.Headers.Add("x-api-key", _apiKey);
        //request.Headers.Add("x-user-id", _userId);

        var str = JsonSerializer.Serialize(requestObject, jsonSerializerOptions ?? CamelCaseOptions);
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
