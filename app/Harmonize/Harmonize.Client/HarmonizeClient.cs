using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Response;
using Harmonize.Client.Model.System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Harmonize.Client.Utility.Utility;

namespace Harmonize.Client;

public class HarmonizeClient
{
    private string hostName;
    private int port;
    private static readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
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
    public HarmonizeClient(string hostName, int port)
    {
        this.hostName = hostName;
        this.port = port;
    }
    #region GET
    public string GetMediaMetadataArtworkUrl(MediaMetadata mediaMetadata, string artworkSize)
    {
        var artwork = mediaMetadata.Artwork;

        var artworkProperty = GetPropertyValue(artwork, artworkSize);

        var xlMediaUrl = $"http://{hostName}:{port}/api/{mediaMetadata.Artwork}";

        return xlMediaUrl;
    }
    public async Task<byte[]> GetMedia(string fileName)
    {
        return await HarmonizeRequestBytes($"stream/{fileName}", HttpMethod.Get);
    }
    public async Task<Playlist> GetPlaylist(string playlistName)
    {
        return await HarmonizeRequest<Playlist>($"playlist/{playlistName}", HttpMethod.Get);
    }
    public async Task<MediaMetadata> GetMediaMetadata(string fileName)
    {
        return await HarmonizeRequest<MediaMetadata>($"metadata/media/{fileName}", HttpMethod.Get);
    }
    public async Task<List<Job>> GetJobs()
    {
        return await HarmonizeRequest<List<Job>>($"job", HttpMethod.Get);
    }
    #endregion
    #region POST
    public async Task<BaseResponse<Job>> CancelJob(Guid jobId)
    {
        return await HarmonizeRequest<BaseResponse<Job>>($"job/cancel/{jobId}", HttpMethod.Post);
    }
    #endregion


    #region BASE REQUESTS
    private async Task<TResponse> HarmonizeRequest<TResponse>(string path, HttpMethod httpMethod)
    {
        var request = new HttpRequestMessage(httpMethod, $"http://{hostName}:{port}/api/" + path);
        request.Headers.Accept.Clear();
        //request.Headers.Add("x-api-key", _apiKey);
        //request.Headers.Add("x-user-id", _userId);

        return await SendRequest<TResponse>(request);
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

        request.Content = new StringContent(JsonSerializer.Serialize(requestObject, serializeOptions), Encoding.UTF8, "application/json");

        return await SendRequest<TResponse>(request);
    }
    //TODO: return a monad
    private async Task<TResponse> SendRequest<TResponse>(HttpRequestMessage request)
    {
        var clientResponse = await httpClient.SendAsync(request, CancellationToken.None);

        clientResponse.EnsureSuccessStatusCode();

        var str = await clientResponse.Content.ReadAsStringAsync();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
        return await JsonSerializer.DeserializeAsync<TResponse>(stream, serializeOptions) ?? throw new NullReferenceException(nameof(JsonSerializer.DeserializeAsync));
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
