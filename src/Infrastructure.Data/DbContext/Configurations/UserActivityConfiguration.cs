using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
    {
        public void Configure(EntityTypeBuilder<UserActivity> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserActivities)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Details).IsRequired();
            builder.Property(x => x.EventType).IsRequired();
            builder.Property(x => x.ObjectClass).IsRequired();
        }
    }
}

