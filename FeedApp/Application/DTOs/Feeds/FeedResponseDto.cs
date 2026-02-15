using Domain.Enums;

namespace Application.DTOs.Feeds
{
    public record FeedResponseDto
    {
        public required Guid Id { get; init; }
        public required string Title { get; init; }
        public required string Description { get; init; }
        public required FeedType FeedType { get; init; }
        public required Guid UserId { get; init; }
        public required string Username { get; init; }
        public required DateTime CreatedAtUtc { get; init; }
        public DateTime? UpdatedAtUtc { get; init; }
        public required int LikeCount { get; init; }
        public required int CommentCount { get; init; }

        // Image-specific (null for TextFeed)
        public bool HasImage { get; init; }

        // Video-specific (null for non-video feeds)
        public string? VideoUrl { get; init; }
    }
}
