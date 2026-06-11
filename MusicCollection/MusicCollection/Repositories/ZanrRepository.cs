using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MusicCollection.Models;
using Npgsql;

namespace MusicCollection.Repositories;

public class ZanrRepository : IZanrRepository
{
    private readonly string _connectionString;

    public ZanrRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Zanr>> GetAllAsync()
    {
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryAsync<Zanr>("SELECT * FROM zanr ORDER BY nazev");
    }
}
