using CommunityToolkit.Maui.Views;
using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Service;

public class MediaManager
{
    readonly ILogger logger;
    readonly HarmonizeClient harmonizeClient;

    public MediaManager(
        ILogger<MediaManager> logger,
        HarmonizeClient harmonizeClient
        )
    {
        this.logger = logger;
        this.harmonizeClient = harmonizeClient;

        CreateMediaFolders();
    }
    void CreateMediaFolders()
    {

    }
    public async Task<Playlist> GetPlaylist(string name)
    {
        var playlist = await harmonizeClient.GetPlaylist("foo");

        return playlist;
    }
    public async Task<MediaSource> GetMediaResource(string name)
    {
        var localPath = Path.Combine(FileSystem.AppDataDirectory, name);

        if (Path.Exists(localPath))
        {
            return MediaSource.FromFile(localPath);
        }

        var fileBytes = await harmonizeClient.GetMedia(name);

        await File.WriteAllBytesAsync(localPath, fileBytes);

        return MediaSource.FromFile(localPath);
    }
    public async Task<MediaMetadata> GetMediaMetadata(string name)
    {
        var mediaMetadata = await harmonizeClient.GetMediaMetadata(name);

        return mediaMetadata;
    }
    public string GetMediaMetadataArtworkUrl(MediaMetadata mediaMetadata, string artworkSize)
    {
        var artworkUrl = harmonizeClient.GetMediaMetadataArtworkUrl(mediaMetadata, artworkSize);

        return artworkUrl;
    }
}
