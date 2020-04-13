using System.Threading.Tasks;
using BlazorDemoUdemy.Shared.Entity;

namespace BlazorDemoUdemy.Client.Repository {
    public interface IRatingRepository
    {
        Task Vote(MovieRating movieRating);
    }
}