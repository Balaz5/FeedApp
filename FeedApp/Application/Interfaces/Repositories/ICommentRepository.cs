using FeedApp.Domain.Entities;
using FeedApp.Domain.Projections;

namespace FeedApp.Application.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<CommentDetails?> GetDetailsByIdAsync(Guid id, CancellationToken ct = default);
        Task<(IReadOnlyList<CommentDetails> Items, int TotalCount)> GetPagedByFeedIdAsync(
            Guid feedId, int page, int pageSize, CancellationToken ct = default);
        void Add(Comment comment);
        void Remove(Comment comment);
    }
}
