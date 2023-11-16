using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Entities;

namespace MoviesApp.Controllers
{
    [ApiController]
    public class MovieApiController : Controller
    {
        public MovieApiController(MovieDbContext movieDbContext)
        {
            _movieDbContext = movieDbContext;
        }

        // provide api for MSS to get movie information
        [HttpGet("/api/movies/{id}")]
        public IActionResult ApiGetMovieById(int id)
        {
            var movie = _movieDbContext.Movies.Include(m => m.Genre)
                .Include(m => m.Reviews)
                .Select(m => new ApiGetMovieResponse()
                {
                    MovieId = m.MovieId,
                    Name = m.Name,
                    GenreId = m.GenreId,
                    Genre = m.Genre,
                    Rating = Convert.ToInt32(m.Reviews.Average(r => r.Rating).GetValueOrDefault(1)),
                    Year = m.Year,
                    ProductionName = "MPC Productions"
                }).Where(m => m.MovieId == id).FirstOrDefault();

            return Json(movie);
        }

        // provide api for MSS to request streaming right and log it in console
        [HttpPost("/api/streamingrights/notification/{id}")]
        public IActionResult GetStreamingRightNotification(int id)
        {
            var movie = _movieDbContext.Movies
                .Select(m => new Movie()
                {
                    MovieId = m.MovieId,
                    Name = m.Name,
                    Year = m.Year,
                }).Where(m => m.MovieId == id).FirstOrDefault();

            var headers = Request.Headers;
            var token = headers["Authorization"].ToString();
            Console.WriteLine("Stream rights were request for movie " + movie.Name + 
                " by stream partner with API key: " + token.Replace("Bearer ", ""));

            return Ok();
        }

        private MovieDbContext _movieDbContext;
    }
}
