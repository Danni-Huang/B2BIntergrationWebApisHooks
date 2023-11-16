using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MoviesApp.Entities;
using MoviesApp.Models;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace MoviesApp.Controllers
{
    public class MovieController : Controller
    {
        public MovieController(MovieDbContext movieDbContext, IConfiguration configuration)
        {
            _movieDbContext = movieDbContext;
            _configuration = configuration;
        }

        [HttpGet()]
        public IActionResult List()
        {
            var allMovies = _movieDbContext.Movies
                    .Include(m => m.Genre)
                    .Include(m => m.ProductionStudio)
                    .OrderBy(m => m.Name).ToList();

            return View(allMovies);
        }

        [HttpGet()]
        public IActionResult Add()
        {
            MovieViewModel movieViewModel = new MovieViewModel()
            {
                Genres = _movieDbContext.Genres.OrderBy(g => g.Name).ToList(),
                Studios = _movieDbContext.Studios.OrderBy(s => s.Name).ToList(),
                ActiveMovie = new Movie()
            };

            return View(movieViewModel);
        }

        [HttpPost()]
        public IActionResult Add(MovieViewModel movieViewModel)
        {
            if (ModelState.IsValid)
            {
                // it's valid so we want to add the new movie to the DB:
                _movieDbContext.Movies.Add(movieViewModel.ActiveMovie);
                _movieDbContext.SaveChanges();
                return RedirectToAction("List", "Movie");
            }
            else
            {
                // it's invalid so we simply return the movie object
                // to the Edit view again:
                movieViewModel.Genres = _movieDbContext.Genres.OrderBy(g => g.Name).ToList();
                movieViewModel.Studios = _movieDbContext.Studios.OrderBy(s => s.Name).ToList();
                return View(movieViewModel);
            }
        }


        [HttpGet()]
        public IActionResult Edit(int id)
        {
            MovieViewModel movieViewModel = new MovieViewModel()
            {
                Genres = _movieDbContext.Genres.OrderBy(g => g.Name).ToList(),
                Studios = _movieDbContext.Studios.OrderBy(s => s.Name).ToList(),
                ActiveMovie = _movieDbContext.Movies.Find(id)
            };

            return View(movieViewModel);
        }

        [HttpPost()]
        public IActionResult Edit(MovieViewModel movieViewModel)
        {
            if (ModelState.IsValid)
            {
                // it's valid so we want to update the existing movie in the DB:
                _movieDbContext.Movies.Update(movieViewModel.ActiveMovie);
                _movieDbContext.SaveChanges();
                return RedirectToAction("List", "Movie");
            }
            else
            {
                movieViewModel.Genres = _movieDbContext.Genres.OrderBy(g => g.Name).ToList();
                movieViewModel.Studios = _movieDbContext.Studios.OrderBy(s => s.Name).ToList();
                return View(movieViewModel);
            }
        }

        [HttpGet()]
        public IActionResult Delete(int id)
        {
            // Find/retrieve/select the movie to edit and then pass it to the view:
            var movie = _movieDbContext.Movies.Find(id);
            return View(movie);
        }

        [HttpPost()]
        public IActionResult Delete(Movie movie)
        {
            // Simply remove the movie and then redirect back to the all movies view:
            _movieDbContext.Movies.Remove(movie);
            _movieDbContext.SaveChanges();
            return RedirectToAction("List", "Movie");
        }

        // request streaming rights
        [HttpPost()]
        public IActionResult RequestStreamingRights(int movieId)
        {
            var existingMovie = _movieDbContext.Movies.FirstOrDefault(m => m.MovieId == movieId);

            if (existingMovie == null)
            {
                throw new Exception("movie not exists");
            }

            // Send a HTTP request to MPC new endpoint.
            HttpClient client = new HttpClient();

            string apiKey = _configuration["ProductionStudioSettings:ApiKey"];

            client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", apiKey);

            string url = existingMovie.ClaimUrl;

            HttpResponseMessage resp = client.PostAsync(url, null).Result;

            if (resp.IsSuccessStatusCode)
            {
                // it's valid so we want to update the existing movie's streaming status in the DB:
                existingMovie.StreamingStatus = "StreamingRightsRequested";
                _movieDbContext.SaveChanges();
            }

            return RedirectToAction("List", "Movie");
        }

        private MovieDbContext _movieDbContext;
        private readonly IConfiguration _configuration;
    }
}
