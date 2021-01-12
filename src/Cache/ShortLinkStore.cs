using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace url_shortener_csharp
{
    // pretty basic, but a link to more for reference
    // https://nickcraver.com/blog/2019/08/06/stack-overflow-how-we-do-app-caching/
    public interface IShortLinkCache
    { 
        Task CacheLink(ShortLink link);
        Task<ShortLink> TryGetCachedLink(string key);

    }
    public class ShortLinkCache : IShortLinkCache
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<ShortLinkCache> _logger;

        public ShortLinkCache(IDistributedCache cache, ILogger<ShortLinkCache> logger)
        {
            _cache = cache;
            _logger = logger;
        }
        public async Task CacheLink(ShortLink link)
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
        
        public async Task<ShortLink> TryGetCachedLink(string key)
        {
            var cachedLinkEntry = await _cache.GetAsync(key);
            if (cachedLinkEntry is null) 
                return null;
            
            _logger.LogInformation("Found cached entry, returning from cache");

            var serialisedLink = Encoding.UTF8.GetString(cachedLinkEntry);
            var cachedShortLink = JsonConvert.DeserializeObject<ShortLink>(serialisedLink);

            return cachedShortLink;
        }
    }
}