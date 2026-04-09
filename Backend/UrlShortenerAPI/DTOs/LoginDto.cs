using System.ComponentModel.DataAnnotations;

namespace UrlShortenerAPI.DTOs
{
    public class LoginDto
    {
        // public string Username { get; set; };

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
