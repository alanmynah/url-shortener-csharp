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
        public IActionResult Post([FromBody] ShortLinkRequest linkRequest) // no DTO, because what for? too simple of an example
        {
            _logger.LogInformation("Creating new short URL");
            ShortLink link;
            if (linkRequest.Slug is null)
            {
                var randomSlug = ShortLink.GenerateRandomSlug();
                link = new ShortLink(randomSlug, linkRequest.Destination);
            }
            else
            {
                link = new ShortLink(linkRequest.Slug, linkRequest.Destination);
            } 
            return Ok(link);
        }
    }
}