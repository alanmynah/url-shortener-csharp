using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace url_shortener_csharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedirectController : ControllerBase
    {
        private readonly ILogger<ShortLinkController> _logger;
        private readonly AppDbContext _db; // yes, yes, i know. maybe later

        public RedirectController(ILogger<ShortLinkController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db; 
        }
        
        [HttpGet("{slug}")]
        public async Task<IActionResult> Get(string slug)
        {
            if (slug is null)
                return NoContent();
            
            var link = await _db.ShortLinks.FirstOrDefaultAsync(sl => sl.Slug == slug); 
            if (link is not null)
                return Redirect(link.Destination);

            return NoContent();
        }
    }
}