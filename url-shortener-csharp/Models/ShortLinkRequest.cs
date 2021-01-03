using System;
using System.ComponentModel.DataAnnotations;

namespace url_shortener_csharp
{
    public class ShortLinkRequest
    {
        public ShortLinkRequest(string slug, string destination)
        {
            Slug = slug;
            Destination = destination;
        }
        
        public string Slug { get; }
        [Required]
        public string Destination  { get; }
    }
}