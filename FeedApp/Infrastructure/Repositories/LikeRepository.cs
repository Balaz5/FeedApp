using FeedApp.Application.Interfaces.Repositories;
using FeedApp.Domain.Entities;
using FeedApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeedApp.Infrastructure.Repositories
{
    public sealed class LikeRepository(AppDbContext dbContext) : ILikeRepository
    {
        public async Task<bool> ExistsAsync(Guid userId, Guid feedId, CancellationToken ct = default)
        {
            return await dbContext.Likes.AnyAsync(l => l.UserId == userId && l.FeedId == feedId, ct);
        }

        public async Task<Like?> GetByUserAndFeedAsync(Guid userId, Guid feedId, CancellationToken ct = default)
        {
            return await dbContext.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.FeedId == feedId, ct);
        }

        public void Add(Like like)
        {
            dbContext.Likes.Add(like);
        }

        public void Remove(Like like)
        {
            dbContext.Likes.Remove(like);
        }
    }
}
