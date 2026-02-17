using FeedApp.Application.DTOs.Comments;
using FeedApp.Application.DTOs.Feeds;
using FeedApp.Application.DTOs.Users;
using FeedApp.Domain.Entities;

namespace FeedApp.Application.Mapping
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

        public static UserResponseDto ToResponseDto(this User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAtUtc = user.CreatedAtUtc,
                FeedCount = user.Feeds?.Count ?? 0
            };
        }

        public static CommentResponseDto ToResponseDto(this Comment comment)
        {
            return new CommentResponseDto
            {
                Id = comment.Id,
                Content = comment.Content,
                UserId = comment.UserId,
                Username = comment.User?.Username ?? string.Empty,
                FeedId = comment.FeedId,
                CreatedAtUtc = comment.CreatedAtUtc,
                UpdatedAtUtc = comment.UpdatedAtUtc
            };
        }
    }
}
