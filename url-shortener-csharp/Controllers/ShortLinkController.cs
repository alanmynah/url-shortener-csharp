﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ShortLinkController : ControllerBase
    {
        private readonly ILogger<ShortLinkController> _logger;
        private readonly AppDbContext _db; // yep, just here in the controller.
        // i have heard of repositories and units of work, yes, but again,
        // let's keep it simple
        private readonly IDistributedCache _cache;
        
        public ShortLinkController(ILogger<ShortLinkController> logger, AppDbContext db, IDistributedCache cache)
        {
            _logger = logger;
            _db = db;
            _cache = cache;
        }

        // this one isn't going to age well if we have a lot of links
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // bear with me, yes, awful, i know
            var links = await _db.ShortLinks.ToListAsync();
            
            await Task.WhenAll(links.Select(CacheLink));
            
            return Ok(links);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation($"Checking cache for link with id {id}"); // yes this syntax isn't quite right for rolling up logs, i know, but i'm the only one using it, and it's right there in the console.
            
            var cachedLink =  await TryGetCachedLink(id.ToString());
            if (cachedLink is not null)
                return Ok(cachedLink);
            
            _logger.LogInformation("Didn't find cached entry, returning from DB");

            var link = await _db.ShortLinks.FirstOrDefaultAsync(sl => sl.Id == id);
           
            await CacheLink(link);

            return Ok(link);
        }

        [HttpGet("/check/{slug}")]
        public async Task<IActionResult> Get(string slug)
        {
            var cachedLink =  await TryGetCachedLink(slug);
            if (cachedLink is not null)
                return Ok(cachedLink);
            
            var link = await _db.ShortLinks.FirstOrDefaultAsync(sl => sl.Slug == slug);

            if (link is not null)
                await CacheLink(link);
            
            return Ok(link);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ShortLinkRequest linkRequest) // no DTO, because what for? too simple of an example
        {
            _logger.LogInformation("Creating new short URL");
            
            ShortLink link;
            if (linkRequest.Slug is null)
            {
                var randomSlug = await SlugGenerator.GenerateRandomSlug(_db);
                link = new ShortLink(randomSlug, linkRequest.Destination);
            }
            else
            {
                var isSlugTaken = await _db.ShortLinks.AnyAsync(sl => sl.Slug == linkRequest.Slug);
                if (!isSlugTaken)
                {
                    link = new ShortLink(linkRequest.Slug, linkRequest.Destination);
                }
                else
                {
                    return BadRequest($"Custom slug with value '{linkRequest.Slug}' is taken, please provide a different one");
                }
                    
            }

            // save to db
            await _db.ShortLinks.AddAsync(link);
            await _db.SaveChangesAsync(); // lol, kind of love how awful this is.
            // if this is shocking, do read the note at the top.
            // wonder how many calls would start causing issues with this

            await CacheLink(link);
            
            return Ok(link);
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