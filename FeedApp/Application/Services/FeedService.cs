using Application.DTOs.Common;
using Application.DTOs.Feeds;
using Application.Interfaces;
using Application.Mapping;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class FeedService(IAppDbContext context, ILogger<FeedService> logger) : IFeedService
    {
        public async Task<PagedResponse<FeedResponseDto>> GetFeedsAsync(FeedListRequest request, CancellationToken ct = default)
        {
            logger.LogInformation("Fetching feeds - Page: {Page}, PageSize: {PageSize}, UserId: {UserId}, FeedType: {FeedType}",
                request.Page, request.PageSize, request.UserId, request.FeedType);

            var query = context.Feeds
                .Include(f => f.User)
                .Include(f => f.Likes)
                .Include(f => f.Comments)
                .AsQueryable();

            if (request.UserId.HasValue)
                query = query.Where(f => f.UserId == request.UserId.Value);

            if (request.FeedType.HasValue)
                query = query.Where(f => f.FeedType == request.FeedType.Value);

            var totalCount = await query.CountAsync(ct);

            var feeds = await query
                .OrderByDescending(f => f.CreatedAtUtc)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            return new PagedResponse<FeedResponseDto>
            {
                Items = feeds.Select(f => f.ToResponseDto()).ToList(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<FeedResponseDto> GetFeedByIdAsync(Guid id, CancellationToken ct = default)
        {
            logger.LogInformation("Fetching feed {FeedId}", id);

            var feed = await context.Feeds
                .Include(f => f.User)
                .Include(f => f.Likes)
                .Include(f => f.Comments)
                .FirstOrDefaultAsync(f => f.Id == id, ct)
                ?? throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{id}' was not found.");

            return feed.ToResponseDto();
        }

        public async Task<FeedResponseDto> CreateFeedAsync(CreateFeedRequest request, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("Creating {FeedType} feed for user {UserId}", request.FeedType, userId);

            // Verify user exists
            var userExists = await context.Users.AnyAsync(u => u.Id == userId, ct);
            if (!userExists)
                throw new NotFoundException("USER_NOT_FOUND", $"User with ID '{userId}' was not found.");

            // Business validation: Video feeds must have a VideoUrl
            if (request.FeedType == FeedType.Video && string.IsNullOrWhiteSpace(request.VideoUrl))
                throw new ValidationException("VALIDATION_ERROR", "VideoUrl is required for Video feeds.",
                    new Dictionary<string, string[]> { { "VideoUrl", ["VideoUrl is required for Video feeds."] } });

            Feed feed = request.FeedType switch
            {
                FeedType.Text => new TextFeed(),
                FeedType.Image => new ImageFeed(),
                FeedType.Video => new VideoFeed
                {
                    VideoUrl = request.VideoUrl!
                },
                _ => throw new ValidationException("VALIDATION_ERROR", $"Invalid feed type: {request.FeedType}.")
            };

            feed.Id = Guid.NewGuid();
            feed.Title = request.Title;
            feed.Description = request.Description;
            feed.FeedType = request.FeedType;
            feed.UserId = userId;
            feed.CreatedAtUtc = DateTime.UtcNow;

            context.Feeds.Add(feed);
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Feed {FeedId} created successfully", feed.Id);

            return await GetFeedByIdAsync(feed.Id, ct);
        }

        public async Task<FeedResponseDto> UpdateFeedAsync(Guid id, UpdateFeedRequest request, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("Updating feed {FeedId} by user {UserId}", id, userId);

            var feed = await context.Feeds
                .Include(f => f.User)
                .Include(f => f.Likes)
                .Include(f => f.Comments)
                .FirstOrDefaultAsync(f => f.Id == id, ct)
                ?? throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{id}' was not found.");

            if (feed.UserId != userId)
                throw new ForbiddenException("FEED_FORBIDDEN", "You can only modify your own feeds.");

            feed.Title = request.Title;
            feed.Description = request.Description;
            feed.UpdatedAtUtc = DateTime.UtcNow;

            // Update video URL if it's a video feed
            if (feed is VideoFeed videoFeed)
            {
                if (string.IsNullOrWhiteSpace(request.VideoUrl))
                    throw new ValidationException("VALIDATION_ERROR", "VideoUrl is required for Video feeds.",
                        new Dictionary<string, string[]> { { "VideoUrl", ["VideoUrl is required for Video feeds."] } });

                videoFeed.VideoUrl = request.VideoUrl;
            }

            await context.SaveChangesAsync(ct);

            logger.LogInformation("Feed {FeedId} updated successfully", id);

            return feed.ToResponseDto();
        }

        public async Task DeleteFeedAsync(Guid id, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("Soft-deleting feed {FeedId} by user {UserId}", id, userId);

            var feed = await context.Feeds.FindAsync([id], ct)
                ?? throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{id}' was not found.");

            if (feed.UserId != userId)
                throw new ForbiddenException("FEED_FORBIDDEN", "You can only delete your own feeds.");

            feed.IsDeleted = true;
            feed.DeletedAtUtc = DateTime.UtcNow;
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Feed {FeedId} soft-deleted successfully", id);
        }

        public async Task<FeedResponseDto> UploadFeedImageAsync(Guid id, Stream imageStream, string contentType, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("Uploading image for feed {FeedId} by user {UserId}", id, userId);

            var feed = await context.Feeds
                .Include(f => f.User)
                .Include(f => f.Likes)
                .Include(f => f.Comments)
                .FirstOrDefaultAsync(f => f.Id == id, ct)
                ?? throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{id}' was not found.");

            if (feed.UserId != userId)
                throw new ForbiddenException("FEED_FORBIDDEN", "You can only modify your own feeds.");

            if (feed is not ImageFeed && feed is not VideoFeed)
                throw new ValidationException("VALIDATION_ERROR",
                    "Images can only be uploaded to Image or Video feeds.");

            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream, ct);
            var imageData = memoryStream.ToArray();

            switch (feed)
            {
                case ImageFeed imageFeed:
                    imageFeed.ImageData = imageData;
                    imageFeed.ImageMimeType = contentType;
                    break;
                case VideoFeed videoFeed:
                    videoFeed.ImageData = imageData;
                    videoFeed.ImageMimeType = contentType;
                    break;
            }

            feed.UpdatedAtUtc = DateTime.UtcNow;
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Image uploaded for feed {FeedId} ({Size} bytes)", id, imageData.Length);

            return feed.ToResponseDto();
        }

        public async Task<(byte[] data, string contentType)> GetFeedImageAsync(Guid id, CancellationToken ct = default)
        {
            logger.LogInformation("Fetching image for feed {FeedId}", id);

            var feed = await context.Feeds.FindAsync([id], ct)
                ?? throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{id}' was not found.");

            if (feed is not ImageFeed && feed is not VideoFeed)
                throw new ValidationException("VALIDATION_ERROR",
                    "The feed is not an Image or Video feed.");

            byte[]? imageData = null;
            string? mimeType = null;

            switch (feed)
            {
                case ImageFeed imageFeed:
                    imageData = imageFeed.ImageData;
                    mimeType = imageFeed.ImageMimeType;
                    break;
                case VideoFeed videoFeed:
                    imageData = videoFeed.ImageData;
                    mimeType = videoFeed.ImageMimeType;
                    break;
            }

            if (imageData is null || mimeType is null)
                throw new NotFoundException("IMAGE_NOT_FOUND", $"No image found for feed with ID '{id}'.");

            return (imageData, mimeType);
        }
    }
}
