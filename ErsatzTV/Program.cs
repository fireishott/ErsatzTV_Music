using ErsatzTV.Infrastructure.Data;
using ErsatzTV.Infrastructure.Services;
using ErsatzTV.Core.Interfaces.Streaming;
using Microsoft.EntityFrameworkCore;

namespace ErsatzTV;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        // Run migrations
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TvContext>();
            context.Database.Migrate();
        }
        
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://*:8410");
            })
            .ConfigureServices((context, services) =>
            {
                // Register our custom services
                services.AddSingleton<ISessionTracker, SessionTracker>();
            });
}
