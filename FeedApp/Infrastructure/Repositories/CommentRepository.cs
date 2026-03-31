using FeedApp.Application.Interfaces.Repositories;
using FeedApp.Domain.Entities;
using FeedApp.Domain.Projections;
using FeedApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeedApp.Infrastructure.Repositories
{
    public sealed class CommentRepository(AppDbContext dbContext) : ICommentRepository
    {
        public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<CommentDetails?> GetDetailsByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await dbContext.Comments
                .Where(c => c.Id == id)
                .Select(c => new CommentDetails(
                    c.Id,
                    c.Content,
                    c.UserId,
                    c.User.Username,
                    c.FeedId,
                    c.CreatedAtUtc,
                    c.UpdatedAtUtc))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<(IReadOnlyList<CommentDetails> Items, int TotalCount)> GetPagedByFeedIdAsync(
            Guid feedId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = dbContext.Comments.Where(c => c.FeedId == feedId);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(c => c.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CommentDetails(
                    c.Id,
                    c.Content,
                    c.UserId,
                    c.User.Username,
                    c.FeedId,
                    c.CreatedAtUtc,
                    c.UpdatedAtUtc))
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public void Add(Comment comment)
        {
            dbContext.Comments.Add(comment);
        }

        public void Remove(Comment comment)
        {
            dbContext.Comments.Remove(comment);
        }
    }
}
