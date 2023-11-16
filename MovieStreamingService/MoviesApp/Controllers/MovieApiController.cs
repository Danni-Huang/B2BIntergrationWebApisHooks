using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MoviesApp.Entities;
using System.Net.Http.Headers;

namespace MoviesApp.Controllers
{
    [ApiController]
    public class MovieApiController : Controller
    {
        public MovieApiController(MovieDbContext movieDbContext, IConfiguration configuration)
        {
            _movieDbContext = movieDbContext;
            _configuration = configuration;
        }

        // adding new movie in not streaming state
        [HttpPost("/api/movie-notifications")]
        public IActionResult GetNewMovieNotification(NewMovieNotification newMovieNotification)
        {
            int movieId = newMovieNotification.MovieId;
            string claimUrl = newMovieNotification.ClaimUrl;

            HttpClient client = new HttpClient();

            string apiKey = _configuration["ProductionStudioSettings:ApiKey"];

            client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", apiKey);

            string url = "https://localhost:7082/api/movies/" + movieId;
            HttpResponseMessage resp = client.GetAsync(url).Result;

            if (resp.IsSuccessStatusCode)
            {
                NewMovieResponse movieResponse = resp.Content.ReadFromJsonAsync<NewMovieResponse>().Result;

                ProductionStudio productionStudio = _movieDbContext.Studios
                    .FirstOrDefault(s => s.Name == movieResponse.ProductionName);

                int productionStudioId = 0;

                if (productionStudio != null)
                {
                    // use the ProductionStudioId obtained to fetch other related records
                    productionStudioId = productionStudio.ProductionStudioId;

                    // fetch the ProductionStudio with the specific ProductionStudioId
                    ProductionStudio selectedStudio = _movieDbContext.Studios
                        .Where(s => s.ProductionStudioId == productionStudioId)
                        .Select(s => new ProductionStudio
                        {
                            ProductionStudioId = s.ProductionStudioId
                        })
                        .FirstOrDefault();
                }

                Movie newMovie = new Movie()
                {
                    ProducerMovieId = movieId,
                    Name = movieResponse.Name,
                    Year = movieResponse.Year,
                    Rating = movieResponse.Rating,
                    GenreId = movieResponse.GenreId,
                    ProductionStudioId = productionStudioId,
                    ClaimUrl = claimUrl,
                };

                _movieDbContext.Movies.Add(newMovie);
                _movieDbContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("Hmmmm, there was a problem adding new movie.");
            }
            return Ok();
        }

        private MovieDbContext _movieDbContext;
        private readonly IConfiguration _configuration;
    }
}
