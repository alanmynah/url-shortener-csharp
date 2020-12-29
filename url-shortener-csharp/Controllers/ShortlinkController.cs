using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace url_shortener_csharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShortlinkController : ControllerBase
    {
        private readonly ILogger<ShortlinkController> _logger;

        public ShortlinkController(ILogger<ShortlinkController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post()
        {
            _logger.LogInformation("Creating new short URL");
            return Ok("URL created");
        }
    }
}