using UrlShortenerAPI.DTOs;

namespace UrlShortenerAPI.Services
{
    public interface IUrlService
    {
        Task<ICollection<UrlResponseDto>> GetAllUrls(int userId);
        Task<UrlResponseDto> GetUrlById(int id, int userId);

        Task<UrlResponseDto> CreateUrl(CreateUrlDto dto, int userId);
        Task<UrlResponseDto> UpdateUrl(int id, int userId, UpdateUrlDto dto);
        Task DeleteUrl(int id, int userId);

        Task<string> GetLongUrlByCode(string code);
    }
}
