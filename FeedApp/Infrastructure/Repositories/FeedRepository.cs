using FeedApp.Application.Interfaces.Repositories;
using FeedApp.Domain.Entities;
using FeedApp.Domain.Enums;
using FeedApp.Domain.Projections;
using FeedApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeedApp.Infrastructure.Repositories
{
    public sealed class FeedRepository(AppDbContext dbContext) : IFeedRepository
    {
        public async Task<Feed?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await dbContext.Feeds.FirstOrDefaultAsync(f => f.Id == id, ct);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            return await dbContext.Feeds.AnyAsync(f => f.Id == id, ct);
        }

        public async Task<FeedDetails?> GetDetailsByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await ProjectToDetails(dbContext.Feeds.Where(f => f.Id == id))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<(IReadOnlyList<FeedDetails> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, Guid? userId, FeedType? feedType, string? searchTerm, CancellationToken ct = default)
        {
            var query = dbContext.Feeds.AsQueryable();

            if (userId.HasValue)
                query = query.Where(f => f.UserId == userId.Value);

            if (feedType.HasValue)
                query = query.Where(f => f.FeedType == feedType.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm;
                query = query.Where(f => EF.Functions.Like(f.Title, $"%{term}%") ||
                    EF.Functions.Like(f.Description, $"%{term}%"));
            }

            var totalCount = await query.CountAsync(ct);

            var items = await ProjectToDetails(
                    query.OrderByDescending(f => f.CreatedAtUtc)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize))
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<List<Feed>> GetSoftDeletedAsync(CancellationToken ct = default)
        {
            return await dbContext.Feeds
                .IgnoreQueryFilters()
                .Where(f => f.IsDeleted)
                .ToListAsync(ct);
        }

        public void Add(Feed feed)
        {
            dbContext.Feeds.Add(feed);
        }

        public void RemoveRange(IEnumerable<Feed> feeds)
        {
            dbContext.Feeds.RemoveRange(feeds);
        }

        private static IQueryable<FeedDetails> ProjectToDetails(IQueryable<Feed> query)
        {
            return query.Select(f => new FeedDetails(
                f.Id,
                f.Title,
                f.Description,
                f.FeedType,
                f.UserId,
                f.User.Username,
                f.CreatedAtUtc,
                f.UpdatedAtUtc,
                f.Likes.Count,
                f.Comments.Count,
                (f is ImageFeed)
                    ? ((ImageFeed)f).ImageData != null
                    : (f is VideoFeed) && ((VideoFeed)f).ImageData != null,
                (f is VideoFeed) ? ((VideoFeed)f).VideoUrl : null));
        }
    }
}
