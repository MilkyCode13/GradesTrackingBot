using GradesTrackingBot.Model;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace GradesTrackingBot.Database.Sqlite;

public class SqliteDatabase : IDatabase
{
    private const string AddDisciplineQuery =
        "INSERT INTO Disciplines (Id, Name, UserId, Target) VALUES (@id, @name, @userId, @target);";

    private const string GetDisciplineQuery =
        "SELECT Name, UserId, Target FROM Disciplines WHERE Id = @id;";
    
    private const string GetDisciplinesByUserQuery =
        "SELECT Id, Name, Target FROM Disciplines WHERE UserId = @userId;";

    private const string AddMarkElementQuery =
        "INSERT INTO MarkElements (Id, Name, DisciplineId, Weight, Grade) VALUES (@id, @name, @disciplineId, @weight, @grade);";

    private readonly string connectionString;

    public SqliteDatabase(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public static SqliteDatabase Configure(IConfigurationSection databaseConfig)
    {
        var path = databaseConfig.GetValue<string>("Path");

        var connectionString = new SqliteConnectionStringBuilder {DataSource = path}.ToString();

        return new SqliteDatabase(connectionString);
    }
    
    public async Task AddDiscipline(Discipline discipline)
    {
        var args = new Dictionary<string, object?>
        {
            {"@id", discipline.Id},
            {"@name", discipline.Name},
            {"@userId", discipline.UserId},
            {"@target", discipline.Target ?? (object) DBNull.Value}
        };

        await ExecuteWriteAsync(AddDisciplineQuery, args);
    }

    public async Task<Discipline?> GetDiscipline(Guid id)
    {
        await using var connection = new SqliteConnection(connectionString);
        Task open = connection.OpenAsync();
        await using var cmd = new SqliteCommand(GetDisciplineQuery, connection);

        cmd.Parameters.AddWithValue("@id", id);

        await open;
        await using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        if (!reader.Read())
        {
            return null;
        }

        var name = await reader.GetFieldValueAsync<string>(0);
        var userId = await reader.GetFieldValueAsync<long>(1);
        int? target = await reader.IsDBNullAsync(2) ? null : await reader.GetFieldValueAsync<int>(2);
        return new Discipline(id, name, userId, target);
    }

    public async Task<List<Discipline>> GetDisciplines(long userId)
    {
        await using var connection = new SqliteConnection(connectionString);
        Task open = connection.OpenAsync();
        await using var cmd = new SqliteCommand(GetDisciplinesByUserQuery, connection);

        cmd.Parameters.AddWithValue("@userId", userId);

        await open;
        await using SqliteDataReader reader = await cmd.ExecuteReaderAsync();

        var disciplines = new List<Discipline>();

        while (reader.Read())
        {
            var id = await reader.GetFieldValueAsync<Guid>(0);
            var name = await reader.GetFieldValueAsync<string>(1);
            int? target = await reader.IsDBNullAsync(2) ? null : await reader.GetFieldValueAsync<int>(2);
            disciplines.Add(new Discipline(id, name, userId, target));
        }

        return disciplines;
    }

    public async Task AddMarkElement(MarkElement markElement)
    {
        var args = new Dictionary<string, object?>
        {
            {"@id", markElement.Id},
            {"@name", markElement.Name},
            {"@disciplineId", markElement.Discipline.Id},
            {"@weight", markElement.Weight},
            {"@grade", markElement.Grade ?? (object) DBNull.Value}
        };

        await ExecuteWriteAsync(AddMarkElementQuery, args);
    }

    public Task<MarkElement?> GetMarkElement(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<MarkElement>> GetMarkElements(Discipline discipline)
    {
        throw new NotImplementedException();
    }

    private async Task ExecuteWriteAsync(string query, Dictionary<string, object?> args)
    {
        await using var connection = new SqliteConnection(connectionString);
        Task open = connection.OpenAsync();
        await using var cmd = new SqliteCommand(query, connection);

        foreach ((string? key, object? value) in args)
        {
            cmd.Parameters.AddWithValue(key, value);
        }

        await open;
        await cmd.ExecuteNonQueryAsync();
    }
}