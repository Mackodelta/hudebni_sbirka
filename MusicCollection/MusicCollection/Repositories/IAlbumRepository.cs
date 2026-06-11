using System.Collections.Generic;
using System.Threading.Tasks;
using MusicCollection.Models;

namespace MusicCollection.Repositories;

public interface IAlbumRepository
{
    Task<IEnumerable<Album>> GetAllAsync();
    Task<Album?> GetByIdAsync(int id);
    Task<int> CreateAsync(Album album);
    Task UpdateAsync(Album album);
    Task DeleteAsync(int id);
}
