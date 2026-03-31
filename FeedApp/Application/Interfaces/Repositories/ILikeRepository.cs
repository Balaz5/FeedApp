using FeedApp.Domain.Entities;

namespace FeedApp.Application.Interfaces.Repositories
{
    public interface ILikeRepository
    {
        Task<bool> ExistsAsync(Guid userId, Guid feedId, CancellationToken ct = default);
        Task<Like?> GetByUserAndFeedAsync(Guid userId, Guid feedId, CancellationToken ct = default);
        void Add(Like like);
        void Remove(Like like);
    }
}
