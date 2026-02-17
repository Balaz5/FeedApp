using FeedApp.Application.DTOs.Common;
using FeedApp.Application.DTOs.Users;

namespace FeedApp.Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<UserResponseDto>> GetUsersAsync(PagedRequest request, CancellationToken ct = default);
        Task<UserResponseDto> GetUserByIdAsync(Guid id, CancellationToken ct = default);
    }
}
