using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public ShortLinkController(ILogger<ShortLinkController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db; 
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // bear with me, yes, awful, i know
            var links = await _db.ShortLinks.ToListAsync(); 
            return Ok(links);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            // bear with me, yes, awful, i know
            var note = await _db.ShortLinks.FirstOrDefaultAsync(sl => sl.Id == id);

            return Ok(note);
        }

        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ShortLinkRequest linkRequest) // no DTO, because what for? too simple of an example
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
            
            // save to db
            await _db.ShortLinks.AddAsync(link);
            await _db.SaveChangesAsync(); // lol, kind of love how awful this is.
            // if this is shocking, do read the note at the top. 
            // wonder how many calls would start causing issues with this

            return Ok(link);
        }
    }
}