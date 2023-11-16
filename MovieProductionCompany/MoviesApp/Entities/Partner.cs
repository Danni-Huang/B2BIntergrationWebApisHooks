using System.ComponentModel.DataAnnotations;

namespace MoviesApp.Entities
{
    public class Partner
    {
        // EF Core will configure this to be an auto-incremented primary key:
        public int PartnerId { get; set; }

        [Required(ErrorMessage = "Please enter a Webhook URL.")]
        public string WebhookURL { get; set; }
        public string APIKey { get; set; }
    }
}
