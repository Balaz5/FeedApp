using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.ToTable("Likes");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.CreatedAtUtc)
                .IsRequired();

            // One like per user per feed
            builder.HasIndex(l => new { l.UserId, l.FeedId })
                .IsUnique();

            // Match the Feed soft-delete filter — exclude likes on deleted feeds
            builder.HasQueryFilter(l => !l.Feed.IsDeleted);
        }
    }
}
