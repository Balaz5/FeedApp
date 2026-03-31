namespace FeedApp.Domain.Projections
{
    public sealed record CommentDetails(
        Guid Id,
        string Content,
        Guid UserId,
        string Username,
        Guid FeedId,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc);
}
