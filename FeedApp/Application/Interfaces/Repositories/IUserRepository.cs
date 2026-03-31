using FeedApp.Domain.Entities;
using FeedApp.Domain.Projections;

namespace FeedApp.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<UserDetails?> GetDetailsByIdAsync(Guid id, CancellationToken ct = default);
        Task<(IReadOnlyList<UserDetails> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        void Add(User user);
    }
}
