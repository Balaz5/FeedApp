using FeedApp.Application.Interfaces;
using FeedApp.Infrastructure.Data;

namespace FeedApp.Infrastructure
{
    public sealed class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
