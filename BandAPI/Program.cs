using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BandAPI.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BandAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();


            // Do something before the host run.
            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    // Try to add the migration
                    var context = scope.ServiceProvider.GetService<BandAlbumContext>(); // BandAlbumContext is  part of Service
                    context.Database.EnsureDeleted(); // Delete the database every time.
                    context.Database.Migrate(); // Re-seed the database.
                }
                catch (Exception e)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>(); // get the logger ready
                    logger.LogError(e, "An error occured while migration was in progress");
                }
            }


            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
