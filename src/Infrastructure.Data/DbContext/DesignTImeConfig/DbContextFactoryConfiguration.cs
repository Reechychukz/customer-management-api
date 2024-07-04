using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Infrastructure.Data.DbContext.DesignTImeConfig
{
    //internal class DbContextFactoryConfiguration : IDesignTimeDbContextFactory<AppDbContext>
    //{
    //    public AppDbContext CreateDbContext(string[] args)
    //    {

    //        //Console.WriteLine("Creating DBContext");

    //        //// Set up a logger
    //        //using var loggerFactory = LoggerFactory.Create(builder =>
    //        //{
    //        //    builder.AddConsole();
    //        //});
    //        //var logger = loggerFactory.CreateLogger<DbContextFactoryConfiguration>();

    //        // Log the current directory
    //        //var currentDirectory = Directory.GetCurrentDirectory();
    //        //Console.WriteLine(currentDirectory);
    //        //logger.LogInformation($"Current Directory: {currentDirectory}");

    //        //IConfigurationRoot config = new ConfigurationBuilder()
    //        //    .SetBasePath(Directory.GetCurrentDirectory())
    //        //    .AddJsonFile($"appsettings.json")
    //        //    .AddJsonFile($"appsettings.Development.json")
    //        //    .Build();
    //        //Console.WriteLine($"Connection String: {config}");
    //        //var conn = config.GetConnectionString("DefaultConnection");
    //        var conn = "Server=localhost;Database=WemaDB;User Id=SA;Password=Chukwuma1#";
    //        //logger.LogInformation($"Connection String: {conn}");

    //        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    //        optionsBuilder.UseSqlServer(conn);

    //        return new AppDbContext(optionsBuilder.Options);
    //    }
    //}
}
