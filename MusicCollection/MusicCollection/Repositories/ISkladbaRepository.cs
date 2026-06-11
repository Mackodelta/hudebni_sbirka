using System.Collections.Generic;
using System.Threading.Tasks;
using MusicCollection.Models;

namespace MusicCollection.Repositories;

public interface ISkladbaRepository
{
    Task<IEnumerable<Skladba>> GetByAlbumIdAsync(int albumId);
    Task<Skladba?> GetByIdAsync(int id);
    Task<int> CreateAsync(Skladba skladba);
    Task UpdateAsync(Skladba skladba);
    Task DeleteAsync(int id);
}
