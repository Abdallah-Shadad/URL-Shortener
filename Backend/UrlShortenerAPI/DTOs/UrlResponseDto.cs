namespace UrlShortenerAPI.DTOs
{
    public class UrlResponseDto
    {
        public int Id { get; set; }
        public string LongUrl { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty; // Reconstructed
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
