using System.ComponentModel.DataAnnotations;

namespace MoviesApp.Entities
{
    public class NewMovieResponse
    {
        // EF Core will configure this to be an auto-incremented primary key:
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Please enter a name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a year.")]
        [Range(1850, int.MaxValue, ErrorMessage = "Year must be after 1850.")]
        public int? Year { get; set; }

        [Required(ErrorMessage = "Please enter a rating.")]
        [Range(1, 5, ErrorMessage = "Rating must be btween 1 and 5.")]
        public int? Rating { get; set; }

        [Required(ErrorMessage = "Please specify a genre.")]
        public string? GenreId { get; set; }

        public Genre? Genre { get; set; }

        [Required(ErrorMessage = "Please specify a production name.")]

        public string ProductionName { get; set; }
    }
}
