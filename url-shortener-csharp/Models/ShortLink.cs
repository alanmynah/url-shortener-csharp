using System;
using System.ComponentModel.DataAnnotations;

namespace url_shortener_csharp
{
    public class ShortLink
    {
        
        public ShortLink()
        {
        }
        
        public ShortLink(string slug, string destination)
        {
            Slug = slug;
            Destination = destination;
        }

        public int Id { get; set; } // tbh, slug could've been used as ID as it's intended to be unique,
                                    // but i can't be bothered to think through why this is a bad idea now,
                                    // so just adding id to save some unexpected headache and get on with this example
        [Required]
        public string Slug { get; set; }

        [Required]
        public string Destination  { get; set; }

        public static string GenerateRandomSlug()
        {
            return "123";
        }
    }
}