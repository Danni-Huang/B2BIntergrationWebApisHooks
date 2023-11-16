using Microsoft.AspNetCore.Mvc;
using MoviesApp.Entities;
using MoviesApp.Models;

namespace MoviesApp.Controllers
{
    public class PartnerController : Controller
    {
        public PartnerController(MovieDbContext movieDbContext)
        {
            _movieDbContext = movieDbContext;
        }

        // going to AddPartner page after clicking Streaming Partner link in the home page
        [HttpGet("/streaming-partners/add-request")]
        public IActionResult StreamPartnerAddRequest()
        {
            return View("AddPartner");
        }

        // adding new partner to the database and going to ShowPartnerWeekhook Action
        [HttpPost("/streaming-partner/add-request")]
        public IActionResult AddPartnerWebhook(PartnerViewModel partnerViewModel)
        {
            Partner newPartner = new Partner()
            {
                APIKey = Guid.NewGuid().ToString(),
                WebhookURL = partnerViewModel.WebhookURL             
            };

            _movieDbContext.Partners.Add(newPartner);
            _movieDbContext.SaveChanges();

            // showing verification message after the new streaming partner was added successfully
            TempData["LastActionMessage"] = "A new streaming partner was added successfully.";
            
            return RedirectToAction("ShowPartnerWebhook", "Partner", new { id = newPartner.PartnerId });
        }

        // going to Streaming partner details page
        [HttpGet("/streaming-partners/{id}")]
        public IActionResult ShowPartnerWebhook(int id)
        {
            var partner = _movieDbContext.Partners.Where(p => p.PartnerId == id).Select(p => new Partner()
            {
                WebhookURL = p.WebhookURL,
                APIKey = p.APIKey
            }).FirstOrDefault();

            PartnerViewModel partnerViewModel = new PartnerViewModel()
            {
                WebhookURL = partner.WebhookURL,
                ApiKey = partner.APIKey
            };
            return View("ShowPartner", partnerViewModel);
        }

        private MovieDbContext _movieDbContext;
    }
}
