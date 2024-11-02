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
    private readonly AlertService alertService;

    public static string MediaPath => Path.Combine(FileSystem.AppDataDirectory, "media");
    public static string AudioPath => Path.Combine(FileSystem.AppDataDirectory, "media", "audio");
    public static string VideoPath => Path.Combine(FileSystem.AppDataDirectory, "media", "video");

    public MediaManager(
        ILogger<MediaManager> logger,
        HarmonizeClient harmonizeClient,
        HarmonizeDatabase harmonizeDatabase,
        FailsafeService failsafeService,
        AlertService alertService
        )
    {
        this.logger = logger;
        this.harmonizeClient = harmonizeClient;
        this.harmonizeDatabase = harmonizeDatabase;
        this.failsafeService = failsafeService;
        this.alertService = alertService;
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
    public async Task<List<LocalMediaEntry>> GetMediaEntries()
    {
        var media = await harmonizeDatabase.GetMediaEntries();
        var (response, success) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.GetMedia();
        }, null);

        if (media.Count != response?.Value.Count)
        {
            Task.Run(SyncLocalMediaStore);
        }

        return media;
    }
    private async Task SyncLocalMediaStore()
    {
        var localMedia = await harmonizeDatabase.GetMediaEntries();
        var (response, success) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.GetMedia();
        }, null);

        if (!success)
        {
            //TODO
            return;
        }

        var localMediaIds = new HashSet<Guid>(localMedia.Select(media => media.Id));

        var mediaInServer = (response?.Value ?? [])
            .Where(media => !localMediaIds.Contains(media.Id))
            .ToList();

        foreach (var mediaEntry in mediaInServer)
        {
            await CreateLocalMediaEntry(mediaEntry);
        }

        logger.LogInformation("Finished syncing local db");

        alertService.ShowAlert("Sync", "Finished syncing local db");
    }
    public async Task<LocalMediaEntry> GetMediaEntry(Guid id)
    {
        var media = await harmonizeDatabase.GetMediaEntry(id);

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
    private async Task CreateLocalMediaEntry(MediaEntry entry)
    {
        var localPath = Path.Combine(AudioPath, entry.Name);

        var (fileBytes, _) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.GetMediaBytes(entry);
        }, null);

        if (fileBytes == null)
        {
            //TODO
            return;
        }

        await File.WriteAllBytesAsync(localPath, fileBytes);

        var newMediaElement = new LocalMediaEntry
        {
            LocalPath = localPath,
            Name = entry.Name,
            Id = entry.Id,
        };

        await harmonizeDatabase.CreateMediaEntry(newMediaElement);
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

            //var (fileBytes, _) = await failsafeService.Fallback(async () =>
            //{
            //    return await harmonizeClient.GetMediaBytes(name);
            //}, null);

            //await File.WriteAllBytesAsync(localPath, fileBytes);

            //var newMediaElement = new LocalMediaEntry
            //{
            //    LocalPath = localPath,
            //    Name = name,
            //};

            //await harmonizeDatabase.SaveMediaEntry(newMediaElement);

            //return MediaSource.FromFile(localPath);
            return null;
        }
        else
        {
            logger.LogError($"Media entry is not null - has path: {mediaEntry.LocalPath} but path doesnt exist.");
            throw new DirectoryNotFoundException(mediaEntry?.LocalPath);
        }
    }
    public async Task<MediaMetadata?> GetMediaMetadata(LocalMediaEntry localMediaEntry)
    {
        var (response, success) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.GetMediaMetadata(localMediaEntry);
        }, null);

        if (success) return response?.Value;
        return null;
    }
    public string GetMediaMetadataArtworkUrl(MediaMetadata mediaMetadata, string artworkSize)
    {
        var artworkUrl = harmonizeClient.GetMediaMetadataArtworkUrl(mediaMetadata, artworkSize);

        return artworkUrl;
    }
}
