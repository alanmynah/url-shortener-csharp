using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace url_shortener_csharp
{
    public static class ContainerConfiguration
    {
        public static void AddRedis(this IServiceCollection services, string connection)
        {
            services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = connection;
            });
            
            services.AddTransient<IShortLinkCache, ShortLinkCache>();
        }
    }
}