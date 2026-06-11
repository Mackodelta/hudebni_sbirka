using System.Collections.Generic;
using System.Threading.Tasks;
using MusicCollection.Models;

namespace MusicCollection.Repositories;

public interface IZanrRepository
{
    Task<IEnumerable<Zanr>> GetAllAsync();
}
