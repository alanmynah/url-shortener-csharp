using System;

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
        public string Destination  { get; }
    }
}