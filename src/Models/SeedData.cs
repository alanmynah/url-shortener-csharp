using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace url_shortener_csharp
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<AppDbContext>>()))
            {
                // Look for any movies.
                if (context.ShortLinks.Any())
                {
                    return;   // DB has been seeded
                }

                context.ShortLinks.AddRange(
                    new ShortLink
                    {
                        Slug = "abc",
                        Destination = "https://google.com"
                    },
                    new ShortLink
                    {
                        Slug = "123",
                        Destination = "https://google.com"
                    },
                    new ShortLink
                    {
                        Slug = "a12b",
                        Destination = "https://google.com"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}