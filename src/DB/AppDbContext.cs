using Microsoft.EntityFrameworkCore;

namespace url_shortener_csharp
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ShortLink> ShortLinks { get; set; }
    }
}