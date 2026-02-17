namespace FeedApp.Application.Interfaces
{
    public interface ILikeService
    {
        Task LikeFeedAsync(Guid feedId, Guid userId, CancellationToken ct = default);
        Task UnlikeFeedAsync(Guid feedId, Guid userId, CancellationToken ct = default);
    }
}
