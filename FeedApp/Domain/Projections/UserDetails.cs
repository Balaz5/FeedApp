namespace FeedApp.Domain.Projections
{
    public sealed record UserDetails(
        Guid Id,
        string Username,
        string Email,
        DateTime CreatedAtUtc,
        int FeedCount);
}
