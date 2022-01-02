using GradesTrackingBot.Database;
using GradesTrackingBot.Database.Sqlite;
using Microsoft.Extensions.Configuration;

namespace GradesTrackingBot;

internal static partial class Program
{
    private static IDatabase? s_database;

    private static void ConfigureDatabase(IConfigurationSection databaseConfig)
    {
        IConfigurationSection databaseConfigSection = databaseConfig.GetChildren().First();

        s_database = databaseConfigSection.Key switch
        {
            "Sqlite" => SqliteDatabase.Configure(databaseConfigSection),
            _ => throw new ArgumentException("Invalid database name")
        };
    }
}