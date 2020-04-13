using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemoUdemy.Shared.DTO;
using BlazorDemoUdemy.Shared.Entity;

namespace BlazorDemoUdemy.Client.Repository{

    public interface IMoviesRepository
    {
        Task<int> CreateMovie(Movie movie);
        Task<IndexPageDTO> GetIndexPageDTO();
        Task<DetailsMovieDTO> GetDetailsMovieDTO(int id);
        Task<MovieUpdateDTO> GetMovieForUpdate(int id);
        Task UpdateMovie(Movie movie);
        Task DeleteMovie(int id);
        Task<PaginatedResponse<List<Movie>>> GetMoviesFiltered(FilterMoviesDTO filterMoviesDTO);
}
}