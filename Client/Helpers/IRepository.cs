using BlazorDemoUdemy.Shared.Entity;
using System.Linq;
using System.Collections.Generic;
namespace BlazorDemoUdemy.Client.Helpers
{
    public interface IRepository
    {
         List<Movie> GetMovies();
    }
}