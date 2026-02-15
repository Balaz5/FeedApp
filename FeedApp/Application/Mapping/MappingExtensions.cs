using Application.DTOs.Feeds;
using Domain.Entities;

namespace Application.Mapping
{
    public static class MappingExtensions
    {
        public static FeedResponseDto ToResponseDto(this Feed feed)
        {
            return new FeedResponseDto
            {
                Id = feed.Id,
                Title = feed.Title,
                Description = feed.Description,
                FeedType = feed.FeedType,
                UserId = feed.UserId,
                Username = feed.User?.Username ?? string.Empty,
                CreatedAtUtc = feed.CreatedAtUtc,
                UpdatedAtUtc = feed.UpdatedAtUtc,
                LikeCount = feed.Likes?.Count ?? 0,
                CommentCount = feed.Comments?.Count ?? 0,
                HasImage = feed switch
                {
                    ImageFeed img => img.ImageData != null,
                    VideoFeed vid => vid.ImageData != null,
                    _ => false
                },
                VideoUrl = feed is VideoFeed videoFeed ? videoFeed.VideoUrl : null
            };
        }
    }
}
