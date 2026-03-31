using FeedApp.Application.Interfaces.Repositories;
using FeedApp.Domain.Entities;
using FeedApp.Domain.Projections;
using FeedApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeedApp.Infrastructure.Repositories
{
    public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
    {
        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await dbContext.Users.AnyAsync(u => u.Id == id, ct);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await dbContext.Users.AnyAsync(u => u.Username == username, ct);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        {
            return await dbContext.Users.AnyAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username, ct);
        }

        public async Task<UserDetails?> GetDetailsByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await dbContext.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDetails(
                    u.Id,
                    u.Username,
                    u.Email,
                    u.CreatedAtUtc,
                    u.Feeds.Count))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<(IReadOnlyList<UserDetails> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, CancellationToken ct = default)
        {
            var query = dbContext.Users.AsQueryable();

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderBy(u => u.Username)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDetails(
                    u.Id,
                    u.Username,
                    u.Email,
                    u.CreatedAtUtc,
                    u.Feeds.Count))
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public void Add(User user)
        {
            dbContext.Users.Add(user);
        }
    }
}
