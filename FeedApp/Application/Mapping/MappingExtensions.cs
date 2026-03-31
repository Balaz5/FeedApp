using FeedApp.Application.DTOs.Comments;
using FeedApp.Application.DTOs.Feeds;
using FeedApp.Application.DTOs.Users;
using FeedApp.Domain.Projections;

namespace FeedApp.Application.Mapping
{
    public static class MappingExtensions
    {
        public static FeedResponseDto ToDto(this FeedDetails details)
        {
            return new FeedResponseDto
            {
                Id = details.Id,
                Title = details.Title,
                Description = details.Description,
                FeedType = details.FeedType,
                UserId = details.UserId,
                Username = details.Username,
                CreatedAtUtc = details.CreatedAtUtc,
                UpdatedAtUtc = details.UpdatedAtUtc,
                LikeCount = details.LikeCount,
                CommentCount = details.CommentCount,
                HasImage = details.HasImage,
                VideoUrl = details.VideoUrl
            };
        }

        public static UserResponseDto ToDto(this UserDetails details)
        {
            return new UserResponseDto
            {
                Id = details.Id,
                Username = details.Username,
                Email = details.Email,
                CreatedAtUtc = details.CreatedAtUtc,
                FeedCount = details.FeedCount
            };
        }

        public static CommentResponseDto ToDto(this CommentDetails details)
        {
            return new CommentResponseDto
            {
                Id = details.Id,
                Content = details.Content,
                UserId = details.UserId,
                Username = details.Username,
                FeedId = details.FeedId,
                CreatedAtUtc = details.CreatedAtUtc,
                UpdatedAtUtc = details.UpdatedAtUtc
            };
        }
    }
}
