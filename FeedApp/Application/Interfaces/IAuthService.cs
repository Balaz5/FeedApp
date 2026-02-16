using FeedApp.Application.DTOs.Auth;

namespace FeedApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken ct = default);
    }
}
