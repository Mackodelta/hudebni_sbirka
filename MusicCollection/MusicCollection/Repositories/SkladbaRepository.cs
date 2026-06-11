using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MusicCollection.Models;
using Npgsql;

namespace MusicCollection.Repositories;

public class SkladbaRepository : ISkladbaRepository
{
    private readonly string _connectionString;

    public SkladbaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private NpgsqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Skladba>> GetByAlbumIdAsync(int albumId)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT * FROM skladba
            WHERE album_id = @AlbumId
            ORDER BY cislo_stopy
            """;
        return await conn.QueryAsync<Skladba>(sql, new { AlbumId = albumId });
    }

    public async Task<Skladba?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Skladba>(
            "SELECT * FROM skladba WHERE id = @Id", new { Id = id });
    }

    public async Task<int> CreateAsync(Skladba skladba)
    {
        using var conn = CreateConnection();
        const string sql = """
            INSERT INTO skladba (album_id, nazev, delka_sekund, cislo_stopy)
            VALUES (@AlbumId, @Nazev, @DelkaSekund, @CisloStopy)
            RETURNING id
            """;
        return await conn.ExecuteScalarAsync<int>(sql, skladba);
    }

    public async Task UpdateAsync(Skladba skladba)
    {
        using var conn = CreateConnection();
        const string sql = """
            UPDATE skladba
            SET nazev = @Nazev, delka_sekund = @DelkaSekund, cislo_stopy = @CisloStopy
            WHERE id = @Id
            """;
        await conn.ExecuteAsync(sql, skladba);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("DELETE FROM skladba WHERE id = @Id", new { Id = id });
    }
}
