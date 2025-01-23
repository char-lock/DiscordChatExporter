using System;
using System.Data.SQLite;

namespace DiscordChatExporter.Core.Database;

public class DatabaseClient : IDisposable
{
    private readonly SQLiteConnection _connection;

    public DatabaseClient(string assetDatabasePath)
    {
        if (!System.IO.File.Exists(assetDatabasePath))
        {
            SQLiteConnection.CreateFile(assetDatabasePath);
        }
        _connection = new SQLiteConnection("Data Source=" + assetDatabasePath);
        _connection.Open();
        EnsureTables();
    }

    public void EnsureTables()
    {
        ExecuteNonQuery(
            @"
            CREATE TABLE IF NOT EXISTS AssetHash (
                Hash TEXT PRIMARY KEY,
                Url TEXT NOT NULL
            );
        "
        );

        ExecuteNonQuery(
            @"
            CREATE TABLE IF NOT EXISTS Asset (
                Hash TEXT PRIMARY KEY,
                FilePath TEXT NOT NULL
            );
        "
        );
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    public void ExecuteNonQuery(string query)
    {
        using var command = new SQLiteCommand(query, _connection);
        command.ExecuteNonQuery();
    }

    public T ExecuteScalar<T>(string query)
    {
        using var command = new SQLiteCommand(query, _connection);
        return (T)command.ExecuteScalar()!;
    }

    public SQLiteDataReader ExecuteReader(string query)
    {
        using var command = new SQLiteCommand(query, _connection);
        return command.ExecuteReader();
    }

    public void ExecuteTransaction(Action<SQLiteTransaction> action)
    {
        using var transaction = _connection.BeginTransaction();
        action(transaction);
        transaction.Commit();
    }
}
