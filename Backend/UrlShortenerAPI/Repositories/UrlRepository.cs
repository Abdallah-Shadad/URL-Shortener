using Microsoft.EntityFrameworkCore;
using System;
using UrlShortenerAPI.Data;
using UrlShortenerAPI.Models;

namespace UrlShortenerAPI.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly AppDbContext _context;
        public UrlRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShortenedUrl> CreateUrl(ShortenedUrl url)
        {
            _context.Add(url);
            await _context.SaveChangesAsync();
            return url;
        }

        public async Task<IEnumerable<ShortenedUrl>> GetAllUrlsByUserId(int userId)
        {
            return await _context.ShortenedUrls
                .Where(u => u.UserId == userId)
                .ToListAsync();
        }
        public async Task<ShortenedUrl?> GetUrlById(int id)
        {
            return await _context.ShortenedUrls
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<ShortenedUrl?> GetUrlByCode(string code)
        {
            return await _context.ShortenedUrls
                .FirstOrDefaultAsync(u => u.Code == code);
        }

        public async Task<ShortenedUrl> UpdateUrl(ShortenedUrl url)
        {
            _context.ShortenedUrls.Update(url);
            await _context.SaveChangesAsync();
            return url;
        }
        public async Task DeleteUrl(int id)
        {
            var url = await _context.ShortenedUrls.FindAsync(id);
            if (url == null)
                throw new KeyNotFoundException("Url not exist");
            _context.ShortenedUrls.Remove(url);
            await _context.SaveChangesAsync();
        }
    }
}
