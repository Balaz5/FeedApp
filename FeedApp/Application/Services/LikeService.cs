using FeedApp.Application.Interfaces;
using FeedApp.Application.Interfaces.Repositories;
using FeedApp.Domain.Entities;
using FeedApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FeedApp.Application.Services
{
    public class LikeService(
        ILikeRepository likeRepository,
        IFeedRepository feedRepository,
        IUnitOfWork unitOfWork,
        ILogger<LikeService> logger) : ILikeService
    {
        public async Task LikeFeedAsync(Guid feedId, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("User {UserId} liking feed {FeedId}", userId, feedId);

            if (!await feedRepository.ExistsAsync(feedId, ct))
                throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{feedId}' was not found.");

            if (await likeRepository.ExistsAsync(userId, feedId, ct))
                throw new ConflictException("DUPLICATE_LIKE", "You have already liked this feed.");

            var like = new Like
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FeedId = feedId,
                CreatedAtUtc = DateTime.UtcNow
            };

            likeRepository.Add(like);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("User {UserId} liked feed {FeedId}", userId, feedId);
        }

        public async Task UnlikeFeedAsync(Guid feedId, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("User {UserId} unliking feed {FeedId}", userId, feedId);

            var like = await likeRepository.GetByUserAndFeedAsync(userId, feedId, ct)
                ?? throw new NotFoundException("LIKE_NOT_FOUND", "You have not liked this feed.");

            likeRepository.Remove(like);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("User {UserId} unliked feed {FeedId}", userId, feedId);
        }
    }
}
