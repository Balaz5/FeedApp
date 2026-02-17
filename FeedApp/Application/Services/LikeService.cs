using FeedApp.Application.Interfaces;
using FeedApp.Domain.Entities;
using FeedApp.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FeedApp.Application.Services
{
    public class LikeService(IAppDbContext context, ILogger<LikeService> logger) : ILikeService
    {
        public async Task LikeFeedAsync(Guid feedId, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("User {UserId} liking feed {FeedId}", userId, feedId);

            var feedExists = await context.Feeds.AnyAsync(f => f.Id == feedId, ct);
            if (!feedExists)
                throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{feedId}' was not found.");

            var alreadyLiked = await context.Likes
                .AnyAsync(l => l.UserId == userId && l.FeedId == feedId, ct);

            if (alreadyLiked)
                throw new ConflictException("DUPLICATE_LIKE", "You have already liked this feed.");

            var like = new Like
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FeedId = feedId,
                CreatedAtUtc = DateTime.UtcNow
            };

            context.Likes.Add(like);
            await context.SaveChangesAsync(ct);

            logger.LogInformation("User {UserId} liked feed {FeedId}", userId, feedId);
        }

        public async Task UnlikeFeedAsync(Guid feedId, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("User {UserId} unliking feed {FeedId}", userId, feedId);

            var like = await context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.FeedId == feedId, ct)
                ?? throw new NotFoundException("LIKE_NOT_FOUND", "You have not liked this feed.");

            context.Likes.Remove(like);
            await context.SaveChangesAsync(ct);

            logger.LogInformation("User {UserId} unliked feed {FeedId}", userId, feedId);
        }
    }
}
