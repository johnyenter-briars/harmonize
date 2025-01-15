using Harmonize.Client.Model.Media;
using Harmonize.Model;
using Microsoft.Extensions.Logging;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Service;

public class HarmonizeDatabase(
    ILogger<HarmonizeDatabase> logger,
    PreferenceManager preferenceManager
    )
{
    SQLiteAsyncConnection? Database;
    ILogger logger = logger;

    async Task Init()
    {
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        logger.LogInformation($"Init database: {Constants.DatabaseFilename}");

        var createTableResult = await Database.CreateTableAsync<LocalMediaEntry>();
        logger.LogInformation($"Create Table Result: {createTableResult.ToString()}");

        if(preferenceManager.UserSettings.ResetDatabaseOnLaunch)
        {
            var wipeDataResult = await Database.DeleteAllAsync<LocalMediaEntry>();
            logger.LogInformation($"Wipe Table Result: {wipeDataResult.ToString()}");
        }
    }
    public async Task<List<LocalMediaEntry>> GetMediaEntries()
    {
        await Init();
        return await Database!.Table<LocalMediaEntry>().ToListAsync();
    }

    public async Task<LocalMediaEntry> GetMediaEntry(Guid id)
    {
        await Init();
        return await Database!.Table<LocalMediaEntry>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }
    public async Task<LocalMediaEntry> GetMediaEntry(string name)
    {
        await Init();
        return await Database!.Table<LocalMediaEntry>().Where(i => i.Name == name).FirstOrDefaultAsync();
    }

    public async Task<int> CreateMediaEntry(LocalMediaEntry item)
    {
        await Init();
        return await Database!.InsertAsync(item);
    }
    public async Task<int> CreateUnsyncedMediaEntry(MediaEntry item)
    {
        await Init();

        var newLocalEntry = new LocalMediaEntry
        {
            Id = item.Id,
            Name = item.Name,
            LocalPath = null,
            Type = item.Type,
            IsSynced = false,
        };

        return await Database!.InsertAsync(newLocalEntry);
    }

    public async Task<int> DeleteMediaEntry(LocalMediaEntry item)
    {
        await Init();
        return await Database!.DeleteAsync(item);
    }
}
