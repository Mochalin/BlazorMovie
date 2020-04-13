using System.Collections.Generic;
using BlazorDemoUdemy.Shared.Entity;

namespace BlazorDemoUdemy.Shared.DTO {

    public class IndexPageDTO{
        public List<Movie> Intheatres{get; set;}
         public List<Movie> UpcomingReleases{get; set;}
    }
}