using System;
using System.Collections.Generic;

namespace DiscordChatExporter.Core.Database;

public class AssetRepository
{
    private readonly DatabaseClient _databaseClient;

    public AssetRepository()
    {
        _databaseClient = new DatabaseClient();
    }

    public List<string> FetchUrlsByHash(string hash)
    {
        string query = $"SELECT Url FROM AssetHash WHERE Hash = '{hash}'";
        var reader = _databaseClient.ExecuteReader(query);
        var urls = new List<string>();
        while (reader.Read())
        {
            urls.Add(reader.GetString(0));
        }
        return urls;
    }

    public bool IsKnownUrl(string url)
    {
        string query = $"SELECT COUNT(*) FROM AssetHash WHERE Url = '{url}'";
        return _databaseClient.ExecuteScalar<long>(query) > 0;
    }

    public string GetHashByUrl(string url)
    {
        string query = $"SELECT Hash FROM AssetHash WHERE Url = '{url}'";
        return _databaseClient.ExecuteScalar<string>(query);
    }

    public bool IsKnownHash(string hash)
    {
        string query = $"SELECT COUNT(*) FROM Asset WHERE Hash = '{hash}'";
        return _databaseClient.ExecuteScalar<long>(query) > 0;
    }

    public string GetPathByHash(string hash)
    {
        string query = $"SELECT FilePath FROM Asset WHERE Hash = '{hash}'";
        return _databaseClient.ExecuteScalar<string>(query);
    }

    public string GetPathByUrl(string url)
    {
        string hash = GetHashByUrl(url);
        return GetPathByHash(hash);
    }

    public bool AddUrlHash(string url, string hash)
    {
        string query = $"INSERT INTO AssetHash (Hash, Url) VALUES ('{hash}', '{url}')";
        try
        {
            _databaseClient.ExecuteNonQuery(query);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public int AddManyUrlHash(Dictionary<string, string> urlsByHash)
    {
        int count = 0;
        foreach (var (hash, url) in urlsByHash)
        {
            if (AddUrlHash(url, hash))
            {
                count++;
            }
        }
        return count;
    }

    public bool AddAssetHash(string path, string hash)
    {
        string query = $"INSERT INTO Asset (Hash, FilePath) VALUES ('{hash}', '{path}')";
        try
        {
            _databaseClient.ExecuteNonQuery(query);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
