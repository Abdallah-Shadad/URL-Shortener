using System.ComponentModel.DataAnnotations;

namespace UrlShortenerAPI.DTOs
{
    public class CreateUrlDto
    {
        [Required]
        [Url]
        public string LongUrl { get; set; } = string.Empty;
    }
}
