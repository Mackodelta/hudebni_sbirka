using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MusicCollection.Models;
using Npgsql;

namespace MusicCollection.Repositories;

public class AlbumRepository : IAlbumRepository
{
    private readonly string _connectionString;

    public AlbumRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private NpgsqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Album>> GetAllAsync()
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT a.*, z.id as zanr_id, z.nazev as zanr_nazev
            FROM album a
            JOIN zanr z ON a.zanr_id = z.id
            ORDER BY a.interpret, a.nazev
            """;

        return await conn.QueryAsync<Album, Zanr, Album>(sql,
            (album, zanr) => { album.Zanr = zanr; return album; },
            splitOn: "zanr_id");
    }

    public async Task<Album?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT a.*, z.id as zanr_id, z.nazev as zanr_nazev
            FROM album a
            JOIN zanr z ON a.zanr_id = z.id
            WHERE a.id = @Id
            """;

        var results = await conn.QueryAsync<Album, Zanr, Album>(sql,
            (album, zanr) => { album.Zanr = zanr; return album; },
            new { Id = id },
            splitOn: "zanr_id");

        return results.FirstOrDefault();
    }

    public async Task<int> CreateAsync(Album album)
    {
        using var conn = CreateConnection();
        const string sql = """
            INSERT INTO album (nazev, interpret, rok, zanr_id, popis)
            VALUES (@Nazev, @Interpret, @Rok, @ZanrId, @Popis)
            RETURNING id
            """;
        return await conn.ExecuteScalarAsync<int>(sql, album);
    }

    public async Task UpdateAsync(Album album)
    {
        using var conn = CreateConnection();
        const string sql = """
            UPDATE album
            SET nazev = @Nazev, interpret = @Interpret, rok = @Rok,
                zanr_id = @ZanrId, popis = @Popis
            WHERE id = @Id
            """;
        await conn.ExecuteAsync(sql, album);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("DELETE FROM album WHERE id = @Id", new { Id = id });
    }
}
