using System;
using System.Collections.Generic;
using BlazorDemoUdemy.Shared.Entity;

namespace BlazorDemoUdemy.Client.Helpers
{
    public class RepositoryInMemory : IRepository
    {
        public List<Movie> GetMovies()
        {
           return new List<Movie>(){
            new Movie () {Title = "Spiderman Last Hero", ReleaseDate = new DateTime(2017,7,2),
             Poster="Images/Moana.jpg"},
            new Movie () {Title = "Inception", ReleaseDate = new DateTime(2019,5,7),
             Poster="Images/Spider-Man.jpg"}};           
        }
    }
}