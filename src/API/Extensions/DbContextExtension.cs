//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.EntityFrameworkCore.Diagnostics;
//using Infrastructure.Data.DbContext;

//namespace API.Extensions
//{
//    /// <summary>
//    /// Service Collection extension object for dbcontext and design time service for migration
//    /// </summary>
//    public static class DbContextExtension
//    {
//        public static readonly ILoggerFactory contextLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

//        /// <summary>
//        /// Add Client Database Context
//        /// </summary>
//        /// <param name="services"></param>
//        /// <param name="configuration"></param>
//        /// <returns></returns>
//        public static IServiceCollection AddClientDbContext(this IServiceCollection services, IConfiguration configuration)
//        {
//            services.AddSingleton(typeof(IDesignTimeDbContextFactory<AppDbContext>), typeof(DbContextFactoryConfiguration));

//            services.AddDbContext<AppDbContext>((option) =>
//            {
//                option.UseSqlServer(configuration.GetConnectionString("ClientDbConnectionString"), b =>
//                {
//                    b.MigrationsAssembly("VastPayClientDbContext");
//                    b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
//                });
//                option.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
//                option.EnableSensitiveDataLogging();
//                //option.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
//                option.ConfigureWarnings(c => c.Log(CoreEventId.DetachedLazyLoadingWarning));
//            }, ServiceLifetime.Singleton);

//            return services;
//        }
//    }
//}

