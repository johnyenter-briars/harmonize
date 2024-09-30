using CommunityToolkit.Maui.Views;
using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Response;
using Harmonize.Model;
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
    readonly HarmonizeDatabase harmonizeDatabase;
    readonly FailsafeService failsafeService;

    public string MediaPath => Path.Combine(FileSystem.AppDataDirectory, "media");
    public string AudioPath => Path.Combine(FileSystem.AppDataDirectory, "media", "audio");
    public string VideoPath => Path.Combine(FileSystem.AppDataDirectory, "media", "video");

    public MediaManager(
        ILogger<MediaManager> logger,
        HarmonizeClient harmonizeClient,
        HarmonizeDatabase harmonizeDatabase,
        FailsafeService failsafeService
        )
    {
        this.logger = logger;
        this.harmonizeClient = harmonizeClient;
        this.harmonizeDatabase = harmonizeDatabase;
        this.failsafeService = failsafeService;
        CreateMediaFolders();
    }
    void CreateMediaFolders()
    {
        foreach (var path in new[] { MediaPath, AudioPath, VideoPath })
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
    public async Task<List<MediaEntry>> GetMediaEntries()
    {
        var media = await harmonizeDatabase.GetMediaEntries();

        return media;
    }
    public async Task<MediaEntry> GetMediaEntry(string name)
    {
        var media = await harmonizeDatabase.GetMediaEntry(name);

        return media;
    }
    public async Task<Playlist?> GetPlaylist(string name)
    {
        var (playlist, _) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.GetPlaylist(name);
        }, null);

        return playlist;
    }
    public async Task<MediaSource> GetMediaResource(string name)
    {
        var mediaEntry = await harmonizeDatabase.GetMediaEntry(name);

        if (Path.Exists(mediaEntry?.LocalPath))
        {
            return MediaSource.FromFile(mediaEntry.LocalPath);
        }

        if (mediaEntry == null)
        {
            var localPath = Path.Combine(AudioPath, name);

            var (fileBytes, _) = await failsafeService.Fallback(async () =>
            {
                return await harmonizeClient.GetMedia(name);
            }, null);

            await File.WriteAllBytesAsync(localPath, fileBytes);

            var newMediaElement = new MediaEntry
            {
                LocalPath = localPath,
                Name = name,
            };

            await harmonizeDatabase.SaveMediaEntry(newMediaElement);

            return MediaSource.FromFile(localPath);
        }
        else
        {
            logger.LogError($"Media entry is not null - has path: {mediaEntry.LocalPath} but path doesnt exist.");
            throw new DirectoryNotFoundException(mediaEntry?.LocalPath);
        }
    }
    public async Task<MediaMetadata?> GetMediaMetadata(string name)
    {
        var (mediaMetadata, _) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.GetMediaMetadata(name);
        }, null);

        return mediaMetadata;
    }
    public string GetMediaMetadataArtworkUrl(MediaMetadata mediaMetadata, string artworkSize)
    {
        var artworkUrl = harmonizeClient.GetMediaMetadataArtworkUrl(mediaMetadata, artworkSize);

        return artworkUrl;
    }
}
