using FeedApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeedApp.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Feed> Feeds { get; }
        DbSet<Like> Likes { get; }
        DbSet<Comment> Comments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
