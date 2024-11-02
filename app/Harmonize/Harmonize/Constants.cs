using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize;

class Constants
{
    public const string DatabaseFilename = "Harmonize.db3";
    public const string LogFilename = "Harmonize.log";

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    public static string LogFilePath =>
        Path.Combine(FileSystem.AppDataDirectory, LogFilename);

    public static int NumRowsToDisplayInLogFile = 10;
}
