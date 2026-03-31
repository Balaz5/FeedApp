using FeedApp.Domain.Enums;

namespace FeedApp.Domain.Projections
{
    public sealed record FeedDetails(
        Guid Id,
        string Title,
        string Description,
        FeedType FeedType,
        Guid UserId,
        string Username,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc,
        int LikeCount,
        int CommentCount,
        bool HasImage,
        string? VideoUrl);
}
