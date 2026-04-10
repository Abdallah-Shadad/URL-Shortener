using UrlShortenerAPI.Data;
using UrlShortenerAPI.DTOs;
using UrlShortenerAPI.Models;

namespace UrlShortenerAPI.Repositories
{
    public interface IUrlRepository
    {
        Task<ShortenedUrl> CreateUrl(ShortenedUrl url);
        Task<ShortenedUrl?> GetUrlById(int id);
        Task<ShortenedUrl?> GetUrlByCode(string code);
        Task<IEnumerable<ShortenedUrl>> GetAllUrlsByUserId(int userId);
        Task<ShortenedUrl> UpdateUrl(ShortenedUrl url);
        Task DeleteUrl(int id);
    }
}
