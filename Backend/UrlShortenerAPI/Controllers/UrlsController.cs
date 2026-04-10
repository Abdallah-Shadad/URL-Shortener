using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortenerAPI.DTOs;
using UrlShortenerAPI.Services;

namespace UrlShortenerAPI.Controllers
{
    [Route("api/urls")]
    [ApiController]
    [Authorize]
    public class UrlsController : BaseController
    {
        private readonly IUrlService _urlService;

        public UrlsController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUrl([FromBody] CreateUrlDto dto)
        {
            var result = await _urlService.CreateUrl(dto, GetUserId());
            return CreatedAtAction(nameof(GetUrlById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUrls()
        {
            var result = await _urlService.GetAllUrls(GetUserId());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUrlById(int id)
        {
            var result = await _urlService.GetUrlById(id, GetUserId());
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUrl(int id, [FromBody] UpdateUrlDto dto)
        {
            var result = await _urlService.UpdateUrl(id, GetUserId(), dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUrl(int id)
        {
            await _urlService.DeleteUrl(id, GetUserId());
            return NoContent();
        }
    }
}