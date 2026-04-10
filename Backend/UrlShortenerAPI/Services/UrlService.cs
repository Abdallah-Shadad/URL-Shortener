using AutoMapper;
using Microsoft.AspNetCore.Components.Web;
using System.Xml;
using UrlShortenerAPI.DTOs;
using UrlShortenerAPI.Models;
using UrlShortenerAPI.Repositories;

namespace UrlShortenerAPI.Services
{
    public class UrlService : IUrlService
    {
        private readonly IUrlRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UrlService(IUrlRepository repository, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        // Helper Methods 
        private string GenerateCode()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-")
                .Substring(0, 8);
        }
        private void ConstructShortUrl(ShortenedUrl url, UrlResponseDto responseDto)
        {
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            responseDto.ShortUrl = $"{baseUrl}/{url.Code}";
        }

        private async Task<ShortenedUrl> GetAndValidateUrlOwnershipAsync(int id, int userId)
        {
            var url = await _repository.GetUrlById(id);

            if (url == null)
                throw new KeyNotFoundException("Url not found.");

            if (userId != url.UserId)
                throw new UnauthorizedAccessException("You do not have permission to access this Url.");

            return url;
        }

        //  Services    

        public async Task<UrlResponseDto> CreateUrl(CreateUrlDto dto, int userId)
        {
            var url = new ShortenedUrl
            {
                LongUrl = dto.LongUrl,
                Code = GenerateCode(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateUrl(url);

            var responseDto = _mapper.Map<UrlResponseDto>(url);
            ConstructShortUrl(url, responseDto);
            return responseDto;
        }

        public async Task<ICollection<UrlResponseDto>> GetAllUrls(int userId)
        {
            var urlEntities = await _repository.GetAllUrlsByUserId(userId);
            var responseDtos = _mapper.Map<List<UrlResponseDto>>(urlEntities);

            foreach (var (entity, dto) in urlEntities.Zip(responseDtos))
            {
                ConstructShortUrl(entity, dto);
            }

            return responseDtos;
        }
        public async Task<UrlResponseDto> GetUrlById(int id, int userId)
        {
            var url = await GetAndValidateUrlOwnershipAsync(id, userId);

            var responseDto = _mapper.Map<UrlResponseDto>(url);
            ConstructShortUrl(url, responseDto);
            return responseDto;
        }

        public async Task<string> GetLongUrlByCode(string code)
        {
            var url = await _repository.GetUrlByCode(code);

            if (url == null)
                throw new KeyNotFoundException("This short Url does not exist.");

            if (url.ExpiresAt.HasValue && DateTime.UtcNow > url.ExpiresAt.Value)
                throw new InvalidOperationException("This short Url has expired.");

            return url.LongUrl;
        }

        public async Task<UrlResponseDto> UpdateUrl(int id, int userId, UpdateUrlDto dto)
        {
            var url = await GetAndValidateUrlOwnershipAsync(id, userId);

            _mapper.Map(dto, url);
            await _repository.UpdateUrl(url);

            var responseDto = _mapper.Map<UrlResponseDto>(url);
            ConstructShortUrl(url, responseDto);
            return responseDto;
        }
        public async Task DeleteUrl(int id, int userId)
        {
            await GetAndValidateUrlOwnershipAsync(id, userId);

            await _repository.DeleteUrl(id);
        }
    }
}
