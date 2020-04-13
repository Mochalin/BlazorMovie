namespace BlazorDemoUdemy.Shared.DTO {
    public class FilterMoviesDTO{
        public int Page {get; set;} =1;
        public int RecordsPerPage {get; set;} = 10;
        public PaginationDTO Pagination{
            get {return new PaginationDTO(){Page = Page, RecordsPerPage = RecordsPerPage};}
        }
        public string Title {get; set;}
        public int GenrId {get; set;}
        public bool InTheatres{ get; set;}
        public bool UpcomingReleases {get; set;}
        
    }
}