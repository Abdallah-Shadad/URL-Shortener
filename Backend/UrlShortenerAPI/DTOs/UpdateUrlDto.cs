using System.ComponentModel.DataAnnotations;

namespace UrlShortenerAPI.DTOs
{
    public class UpdateUrlDto
    {
        [Required]
        [Url]
        public string LongUrl { get; set; } = string.Empty;
    }
}
