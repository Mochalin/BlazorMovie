using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorDemoUdemy.Server.Helpers;
using BlazorDemoUdemy.Shared.DTO;
using BlazorDemoUdemy.Shared.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemoUdemy.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MoviesController : ControllerBase{
       private readonly ApplicationDbContext context;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private string ContainerName = "movies";
        public MoviesController(ApplicationDbContext context,
        IFileStorageService fileStorageService,
        IMapper mapper, UserManager<IdentityUser> userManager = null)
        {
            this.context = context;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet] 
        [AllowAnonymous]     
        public async Task<ActionResult<IndexPageDTO>> Get(){
            var limit =6;
            var moviesInTheatres = await context.Movies.Where(e=>e.InTheatres)
            .Take(limit).OrderByDescending(e=>e.ReleaseDate).ToListAsync();

            var todaysDate = DateTime.Today;

            var upcomingReleases = await context.Movies.Where(e=>e.ReleaseDate>todaysDate)
            .OrderBy(e=>e.ReleaseDate).Take(limit).ToListAsync();

            var response = new IndexPageDTO();
            response.Intheatres = moviesInTheatres;
            response.UpcomingReleases = upcomingReleases;

            return response;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DetailsMovieDTO>> Get(int id){
            var movie = await context.Movies.Where(e=>e.Id == id)
            .Include(e=> e.MoviesGenres).ThenInclude(e=> e.Genre)
            .Include(e=> e.MoviesActors).ThenInclude(e=>e.Person)
            .FirstOrDefaultAsync();
            if (movie == null) {return NotFound();}

            var voteAvarage = 0.0;
            var uservote = 0;
            if (await context.MovieRatings.AnyAsync(e=> e.MovieId == id)){
                voteAvarage = await context.MovieRatings.Where(e=> e.MovieId == id)
                    .AverageAsync(e=>e.Rate);
                
                if (HttpContext.User.Identity.IsAuthenticated){
                    var user= await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                    var userId = user.Id;

                    var userVoteDB = await context.MovieRatings
                    .FirstOrDefaultAsync(e=>e.MovieId == id && e.UserId == userId);

                    if (userVoteDB != null){
                        uservote = userVoteDB.Rate;
                    }
                }
            }

            movie.MoviesActors = movie.MoviesActors.OrderBy(e=>e.Order).ToList();
            var model = new DetailsMovieDTO();
            model.Movie = movie;
            model.Genres = movie.MoviesGenres.Select(e=>e.Genre).ToList();
            model.Actors = movie.MoviesActors.Select(e=>
            new Person{
                Name = e.Person.Name,
                Picture = e.Person.Picture,
                Character = e.Character,
                Id = e.PersonId
            }).ToList();

            model.AverageVote = voteAvarage;
            model.UserVote = uservote;
            return model;
        }

        [HttpPost("filter")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Movie>>> Filter(FilterMoviesDTO filterMoviesDTO){
            var moviesQueryable = context.Movies.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.Title)){
                moviesQueryable = moviesQueryable.Where(e=>e.Title.Contains(filterMoviesDTO.Title));
            }
            if (filterMoviesDTO.InTheatres){
                moviesQueryable = moviesQueryable.Where(e=> e.InTheatres);
            }
            if (filterMoviesDTO.UpcomingReleases){
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(e=> e.ReleaseDate> today);
            }
            if (filterMoviesDTO.GenrId !=0){
                moviesQueryable = moviesQueryable
                .Where(e=>e.MoviesGenres.Select(x=> x.GenreId).Contains(filterMoviesDTO.GenrId));
            }

            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable, filterMoviesDTO.RecordsPerPage);
            var movies = await moviesQueryable.Paginate(filterMoviesDTO.Pagination).ToListAsync();
            return movies;
        }

        [HttpGet("update/{id}")]
        public async Task<ActionResult<MovieUpdateDTO>> PutGet(int id){
            var movieActionResult = await Get(id);
            if (movieActionResult.Result is NotFoundResult) {return NotFound();}

            var movieDetailDTO = movieActionResult.Value;
            var selectedGenresIds = movieDetailDTO.Genres.Select(e=> e.Id).ToList();
            var notSelectedGenres = await context.Genres
            .Where(e=> !selectedGenresIds.Contains(e.Id)).ToListAsync();

            var model = new MovieUpdateDTO();
            model.Movie = movieDetailDTO.Movie;
            model.SelectedGenres = movieDetailDTO.Genres;
            model.NotSelectedGenres = notSelectedGenres;
            model.Actors = movieDetailDTO.Actors;

            return model;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post (Movie movie){           
            if (!string.IsNullOrEmpty(movie.Poster)){
                var poster = Convert.FromBase64String(movie.Poster);
                movie.Poster = await fileStorageService.SaveFile(poster, "jpg", ContainerName);
            }

            if (movie.MoviesActors !=null){
                for (int i=0; i<movie.MoviesActors.Count; i++){
                    movie.MoviesActors[i].Order = i+1;
                }
            }
            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.Id;

        }    

        [HttpPut]
        public async Task<ActionResult> Put(Movie movie){
            var movieDB = await context.Movies.FirstOrDefaultAsync(e=>e.Id == movie.Id);
            if (movieDB == null) {return NotFound();} 
            movieDB = mapper.Map(movie, movieDB);
            if (!string.IsNullOrWhiteSpace(movie.Poster)){
                var moviePoster = Convert.FromBase64String(movie.Poster);
                movieDB.Poster = await fileStorageService.EditFile(moviePoster, "jpg", ContainerName, movieDB.Poster);
            }

            await context.Database.ExecuteSqlInterpolatedAsync(
                $"delete from  MoviesActors where MovieId = {movie.Id}; delete from MoviesGenres where  MovieId = {movie.Id}");

             if (movie.MoviesActors !=null){
                for (int i=0; i<movie.MoviesActors.Count; i++){
                    movie.MoviesActors[i].Order = i+1;
                }
            }
            movieDB.MoviesActors = movie.MoviesActors;
            movieDB.MoviesGenres = movie.MoviesGenres;

            await context.SaveChangesAsync();
            return NoContent();
        } 

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id){
            var movie = await context.Movies.FirstOrDefaultAsync(e=> e.Id == id);
            if (movie == null){
                return NotFound();
            }
            context.Remove(movie);
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}