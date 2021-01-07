using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace url_shortener_csharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedirectController : ControllerBase
    {
        private readonly ILogger<ShortLinkController> _logger;
        private readonly AppDbContext _db; // yes, yes, i know. maybe later
        private readonly IDistributedCache _cache;

        public RedirectController(ILogger<ShortLinkController> logger, AppDbContext db, IDistributedCache cache)
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
            var cachedLink =  await TryGetCachedLink(slug);
            if (cachedLink is not null)
                return Ok(cachedLink);
            
            var link = await _db.ShortLinks.FirstOrDefaultAsync(sl => sl.Slug == slug);
            if (link is not null)
            {
                await CacheLink(link);
                return Redirect(link.Destination);
            }

            return NoContent();
        }
        
        private async Task CacheLink(ShortLink link)
        {
            var linkToCache = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(link));
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(DateTime.Now.AddHours(6));

            _logger.LogInformation("Adding to cache for next time");

            await Task.WhenAll(new List<Task>
            {
                _cache.SetAsync(link.Id.ToString(), linkToCache, options),
                _cache.SetAsync(link.Slug, linkToCache, options)
            });
        }
        
        private async Task<ShortLink> TryGetCachedLink(string key)
        {
            var cachedLinkEntry = await _cache.GetAsync(key);
            if (cachedLinkEntry is not null)
            {
                _logger.LogInformation("Found cached entry, returning from cache");

                var serialisedLink = Encoding.UTF8.GetString(cachedLinkEntry);
                var cachedShortLink = JsonConvert.DeserializeObject<ShortLink>(serialisedLink);

                return cachedShortLink;
            }

            return null;
        }
    }
}