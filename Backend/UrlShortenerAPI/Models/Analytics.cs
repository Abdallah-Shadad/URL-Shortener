namespace UrlShortenerAPI.Models
{
    public class Analytics
    {
        public int Id { get; set; }
        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int ShortenedUrlId { get; set; }
        public ShortenedUrl? ShortenedUrl { get; set; }
    }
}

