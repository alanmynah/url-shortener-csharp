using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// todo:
// - healthcheck
// - get unit of work in
// - write exception middleware
// - easiest way to deploy?

namespace url_shortener_csharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedirectController : ControllerBase
    {
        private readonly ILogger<ShortLinkController> _logger;
        private readonly AppDbContext _db; // yes, yes, i know. maybe later
        private readonly IShortLinkCache _cache;

        public RedirectController(ILogger<ShortLinkController> logger, AppDbContext db, IShortLinkCache cache)
        {
            _logger = logger;
            _db = db;
            _cache = cache;
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> Get(string slug)
        {
            if (slug is null)
                return NoContent();
            var cachedLink =  await _cache.TryGetCachedLink(slug);
            if (cachedLink is not null)
                return Ok(cachedLink);

            var link = await _db.ShortLinks.FirstOrDefaultAsync(sl => sl.Slug == slug);
            if (link is not null)
            {
                await _cache.CacheLink(link);
                return Redirect(link.Destination);
            }

            return NoContent();
        }
    }
}