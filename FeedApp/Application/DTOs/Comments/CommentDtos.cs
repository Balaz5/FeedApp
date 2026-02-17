using System.ComponentModel.DataAnnotations;

namespace FeedApp.Application.DTOs.Comments
{
    public record CommentResponseDto
    {
        public required Guid Id { get; init; }
        public required string Content { get; init; }
        public required Guid UserId { get; init; }
        public required string Username { get; init; }
        public required Guid FeedId { get; init; }
        public required DateTime CreatedAtUtc { get; init; }
        public DateTime? UpdatedAtUtc { get; init; }
    }

    public record CreateCommentRequest
    {
        [Required]
        [MaxLength(1000)]
        public string Content { get; init; } = string.Empty;
    }

    public record UpdateCommentRequest
    {
        [Required]
        [MaxLength(1000)]
        public string Content { get; init; } = string.Empty;
    }
}
