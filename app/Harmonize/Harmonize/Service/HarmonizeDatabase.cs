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

        var result = await Database.CreateTableAsync<LocalMediaEntry>();

        logger.LogInformation($"Init database: {Constants.DatabaseFilename}");
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

    public async Task<int> DeleteMediaEntry(LocalMediaEntry item)
    {
        await Init();
        return await Database!.DeleteAsync(item);
    }
}
