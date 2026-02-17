using FeedApp.Application.DTOs.Common;
using FeedApp.Application.DTOs.Users;
using FeedApp.Application.Interfaces;

namespace FeedApp.Api.Endpoints
{
    public static class UserEndpoints
    {
        public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/users")
                .WithTags("Users")
                .RequireAuthorization();

            group.MapGet("/", GetUsers)
                .WithName("GetUsers")
                .WithSummary("List all users")
                .Produces<PagedResponse<UserResponseDto>>(StatusCodes.Status200OK);

            group.MapGet("/{id:guid}", GetUserById)
                .WithName("GetUserById")
                .WithSummary("Get a user by ID")
                .Produces<UserResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            return group;
        }

        private static async Task<IResult> GetUsers(
            IUserService userService,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default)
        {
            var request = new PagedRequest { Page = page, PageSize = pageSize };
            var result = await userService.GetUsersAsync(request, ct);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetUserById(
            Guid id,
            IUserService userService,
            CancellationToken ct = default)
        {
            var result = await userService.GetUserByIdAsync(id, ct);
            return Results.Ok(result);
        }
    }
}
