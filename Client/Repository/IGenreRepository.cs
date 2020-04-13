using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemoUdemy.Shared.Entity;

namespace BlazorDemoUdemy.Client.Repository{

    public interface IGenreRepository
    {
        Task CreateGenre (Genre genre);
        Task<List<Genre>> GetGenres();
        Task UpdateGenre(Genre genre);
        Task<Genre> GetGenre(int id);
        Task DeleteGenre (int id);
    }
}