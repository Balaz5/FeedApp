using FeedApp.Application.DTOs.Common;
using FeedApp.Application.DTOs.Users;
using FeedApp.Application.Interfaces;
using FeedApp.Application.Mapping;
using FeedApp.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FeedApp.Application.Services
{
    public class UserService(IAppDbContext context, ILogger<UserService> logger) : IUserService
    {
        public async Task<PagedResponse<UserResponseDto>> GetUsersAsync(PagedRequest request, CancellationToken ct = default)
        {
            logger.LogInformation("Fetching users - Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);

            var query = context.Users
                .Include(u => u.Feeds)
                .AsQueryable();

            var totalCount = await query.CountAsync(ct);

            var users = await query
                .OrderBy(u => u.Username)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            return new PagedResponse<UserResponseDto>
            {
                Items = users.Select(u => u.ToResponseDto()).ToList(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<UserResponseDto> GetUserByIdAsync(Guid id, CancellationToken ct = default)
        {
            logger.LogInformation("Fetching user {UserId}", id);

            var user = await context.Users
                .Include(u => u.Feeds)
                .FirstOrDefaultAsync(u => u.Id == id, ct)
                ?? throw new NotFoundException("USER_NOT_FOUND", $"User with ID '{id}' was not found.");

            return user.ToResponseDto();
        }
    }
}
