using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class FeedConfiguration : IEntityTypeConfiguration<Feed>
    {
        public void Configure(EntityTypeBuilder<Feed> builder)
        {
            builder.ToTable("Feeds");

            builder.HasKey(f => f.Id);

            builder.HasDiscriminator(f => f.FeedType)
                .HasValue<TextFeed>(FeedType.Text)
                .HasValue<ImageFeed>(FeedType.Image)
                .HasValue<VideoFeed>(FeedType.Video);

            builder.Property(f => f.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(f => f.CreatedAtUtc)
                .IsRequired();

            builder.Property(f => f.IsDeleted)
                .HasDefaultValue(false);

            // Global query filter for soft delete
            builder.HasQueryFilter(f => !f.IsDeleted);

            // Relationships
            builder.HasMany(f => f.Likes)
                .WithOne(l => l.Feed)
                .HasForeignKey(l => l.FeedId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.Comments)
                .WithOne(c => c.Feed)
                .HasForeignKey(c => c.FeedId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(f => f.UserId);
            builder.HasIndex(f => f.IsDeleted);
        }
    }

    public class ImageFeedConfiguration : IEntityTypeConfiguration<ImageFeed>
    {
        public void Configure(EntityTypeBuilder<ImageFeed> builder)
        {
            builder.Property(f => f.ImageMimeType).HasMaxLength(100);
        }
    }

    public class VideoFeedConfiguration : IEntityTypeConfiguration<VideoFeed>
    {
        public void Configure(EntityTypeBuilder<VideoFeed> builder)
        {
            builder.Property(f => f.ImageMimeType).HasMaxLength(100);
            builder.Property(f => f.VideoUrl).HasMaxLength(2000);
        }
    }
}
