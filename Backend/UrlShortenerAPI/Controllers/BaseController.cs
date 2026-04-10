using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UrlShortenerAPI.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
