using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Infrastructure.Data.DbContext.Configurations;
using Domain.Common;
using Domain.Enums;
using Infrastructure.Data.Data;
using System.Reflection.Emit;

namespace Infrastructure.Data.DbContext
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid,
        UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// This overrides the base SaveChanges Async to perform basic Auditing business logic
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // Auditable details entity pre-processing
            Audit();

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void Audit()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is IAuditableEntity
                                                             && (x.State == EntityState.Modified
                                                             || x.State == EntityState.Added));
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((IAuditableEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                }
                ((IAuditableEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<LGA> LGAs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Role>().HasData(new List<Role>
                {
                    new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{ERole.ADMIN}",
                        NormalizedName = $"{ERole.ADMIN}"
                    },
                    new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{ERole.USER}",
                        NormalizedName = $"{ERole.USER}",
                    }
                }
            );

            builder.Entity<State>()
                .HasMany(s => s.LGAs)
                .WithOne(l => l.State)
                .HasForeignKey(l => l.StateId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<State>()
                .HasMany(s => s.Users)
                .WithOne(u => u.State)
                .HasForeignKey(u => u.StateId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<LGA>()
                .HasMany(l => l.Users)
                .WithOne(u => u.LGA)
                .HasForeignKey(u => u.LGAId)
                .OnDelete(DeleteBehavior.NoAction);


            var states = StateLGAData.GetStates();
            builder.Entity<State>().HasData(states);

            var lgas = StateLGAData.GetLGAs();
            builder.Entity<LGA>().HasData(lgas);
        }
    }

}