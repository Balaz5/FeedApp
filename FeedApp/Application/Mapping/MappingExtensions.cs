using FeedApp.Application.DTOs.Comments;
using FeedApp.Application.DTOs.Users;
using FeedApp.Domain.Entities;

namespace FeedApp.Application.Mapping
{
    public static class MappingExtensions
    {
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
