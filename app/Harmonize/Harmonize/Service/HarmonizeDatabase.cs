using Harmonize.Model;
using Microsoft.Extensions.Logging;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Service;

public class HarmonizeDatabase
{
    SQLiteAsyncConnection? Database;
    ILogger logger;

    public HarmonizeDatabase(ILogger<HarmonizeDatabase> logger)
    {
        this.logger = logger;
    }

    async Task Init()
    {
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        var result = await Database.CreateTableAsync<MediaEntry>();

        logger.LogInformation($"Init database: {Constants.DatabaseFilename}");
    }
    public async Task<List<MediaEntry>> GetMediaEntries()
    {
        await Init();
        return await Database!.Table<MediaEntry>().ToListAsync();
    }

    public async Task<MediaEntry> GetMediaEntry(int id)
    {
        await Init();
        return await Database!.Table<MediaEntry>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }
    public async Task<MediaEntry> GetMediaEntry(string name)
    {
        await Init();
        return await Database!.Table<MediaEntry>().Where(i => i.Name == name).FirstOrDefaultAsync();
    }

    public async Task<int> SaveMediaEntry(MediaEntry item)
    {
        await Init();
        if (item.Id != 0)
            return await Database!.UpdateAsync(item);
        else
            return await Database!.InsertAsync(item);
    }

    public async Task<int> DeleteMediaEntry(MediaEntry item)
    {
        await Init();
        return await Database!.DeleteAsync(item);

    }
}
