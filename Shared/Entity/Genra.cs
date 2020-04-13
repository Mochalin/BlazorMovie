using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace BlazorDemoUdemy.Shared.Entity {

    public class Genre{
        public int Id {get; set;}

        [Required(ErrorMessage = "This field is required")]
        public string Name {get; set;}

        public List<MoviesGenres> MoviesGenres {get; set;} = new List<MoviesGenres>();

    }
}