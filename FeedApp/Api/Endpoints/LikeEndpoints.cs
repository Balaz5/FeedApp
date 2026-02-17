using FeedApp.Api.Extensions;
using FeedApp.Application.Interfaces;
using System.Security.Claims;

namespace FeedApp.Api.Endpoints
{
    public static class LikeEndpoints
    {
        public static RouteGroupBuilder MapLikeEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/feeds/{feedId:guid}/like")
                .WithTags("Likes")
                .RequireAuthorization();

            group.MapPost("/", LikeFeed)
                .WithName("LikeFeed")
                .WithSummary("Like a feed")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapDelete("/", UnlikeFeed)
                .WithName("UnlikeFeed")
                .WithSummary("Remove like from a feed")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            return group;
        }

        private static async Task<IResult> LikeFeed(
            Guid feedId,
            ClaimsPrincipal user,
            ILikeService likeService,
            CancellationToken ct = default)
        {
            var userId = user.GetUserId();
            await likeService.LikeFeedAsync(feedId, userId, ct);
            return Results.NoContent();
        }

        private static async Task<IResult> UnlikeFeed(
            Guid feedId,
            ClaimsPrincipal user,
            ILikeService likeService,
            CancellationToken ct = default)
        {
            var userId = user.GetUserId();
            await likeService.UnlikeFeedAsync(feedId, userId, ct);
            return Results.NoContent();
        }
    }
}
