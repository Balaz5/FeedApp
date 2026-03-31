using FeedApp.Domain.Entities;
using FeedApp.Domain.Enums;
using FeedApp.Domain.Projections;

namespace FeedApp.Application.Interfaces.Repositories
{
    public interface IFeedRepository
    {
        Task<Feed?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
        Task<FeedDetails?> GetDetailsByIdAsync(Guid id, CancellationToken ct = default);
        Task<(IReadOnlyList<FeedDetails> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, Guid? userId, FeedType? feedType, string? searchTerm, CancellationToken ct = default);
        Task<List<Feed>> GetSoftDeletedAsync(CancellationToken ct = default);
        void Add(Feed feed);
        void RemoveRange(IEnumerable<Feed> feeds);
    }
}
