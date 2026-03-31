using FeedApp.Application.DTOs.Common;
using FeedApp.Application.DTOs.Users;
using FeedApp.Application.Interfaces;
using FeedApp.Application.Interfaces.Repositories;
using FeedApp.Application.Mapping;
using FeedApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FeedApp.Application.Services
{
    public class UserService(IUserRepository userRepository, ILogger<UserService> logger) : IUserService
    {
        public async Task<PagedResponse<UserResponseDto>> GetUsersAsync(PagedRequest request, CancellationToken ct = default)
        {
            logger.LogInformation("Fetching users - Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);

            var (items, totalCount) = await userRepository.GetPagedAsync(request.Page, request.PageSize, ct);

            return new PagedResponse<UserResponseDto>
            {
                Items = items.Select(u => u.ToDto()).ToList(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<UserResponseDto> GetUserByIdAsync(Guid id, CancellationToken ct = default)
        {
            logger.LogInformation("Fetching user {UserId}", id);

            var details = await userRepository.GetDetailsByIdAsync(id, ct)
                ?? throw new NotFoundException("USER_NOT_FOUND", $"User with ID '{id}' was not found.");

            return details.ToDto();
        }
    }
}
