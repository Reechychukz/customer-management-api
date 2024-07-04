using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.OTPToken).IsRequired();
        }
    }
}

