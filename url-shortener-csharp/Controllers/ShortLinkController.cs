using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace url_shortener_csharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShortLinkController : ControllerBase
    {
        private readonly ILogger<ShortLinkController> _logger;

        public ShortLinkController(ILogger<ShortLinkController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ShortLink shortLink) // no DTO, because what for? too simple of an example
        {
            _logger.LogInformation("Creating new short URL");
            var link = new ShortLink(shortLink.Slug, shortLink.Destination);
            return Ok(link);
        }
    }
}