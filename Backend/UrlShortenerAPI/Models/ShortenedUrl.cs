namespace UrlShortenerAPI.Models
{
    public class ShortenedUrl
    {
        public int Id { get; set; }
        public string LongUrl { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }

        // Foreign Key
        public int UserId { get; set; }
        public User? User { get; set; }

        // Navigation property
        public ICollection<Analytics> Analytics { get; set; } = new List<Analytics>();
    }
}
