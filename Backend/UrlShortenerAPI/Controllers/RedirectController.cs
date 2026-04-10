using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortenerAPI.Services;

namespace UrlShortenerAPI.Controllers
{
    [ApiController]
    [Route("")] // root
    public class RedirectController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public RedirectController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectToLongUrl(string code)
        {
            try
            {
                var longUrl = await _urlService.GetLongUrlByCode(code);

                return Redirect(longUrl);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}