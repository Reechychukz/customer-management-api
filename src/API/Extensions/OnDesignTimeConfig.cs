using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Infrastructure.Data.DbContext;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace API.Extensions
{
    internal class DbContextFactoryConfiguration : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Create a logger factory
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = loggerFactory.CreateLogger<DbContextFactoryConfiguration>();

            // Log the current directory
            var currentDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine($"Current Directory: {currentDirectory}");
            logger.LogInformation($"Current Directory: {currentDirectory}");


            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json")
                .AddJsonFile($"appsettings.Development.json")
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();

            // Log the connection string
            var conn = config.GetConnectionString("DefaultConnection");
            Console.WriteLine($"Connection String: {conn}");
            logger.LogInformation($"Connection String: {conn}");

            // Configure DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(conn);

            return new AppDbContext(optionsBuilder.Options);

            //var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            //optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            //return new AppDbContext(optionsBuilder.Options);
        }
    }
}

