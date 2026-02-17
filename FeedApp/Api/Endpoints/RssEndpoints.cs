using FeedApp.Application.DTOs.Feeds;
using FeedApp.Application.Interfaces;

namespace FeedApp.Api.Endpoints
{
    public static class RssEndpoints
    {
        public static RouteGroupBuilder MapRssEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/rss")
                .WithTags("RSS")
                .RequireAuthorization();

            group.MapGet("/", GetRssFeed)
                .WithName("GetRssFeed")
                .WithSummary("Fetch items from the configured RSS feed")
                .Produces<IReadOnlyList<RssFeedItemDto>>(StatusCodes.Status200OK);

            return group;
        }

        private static async Task<IResult> GetRssFeed(
            IRssFeedClient rssFeedClient,
            CancellationToken ct = default)
        {
            var items = await rssFeedClient.FetchRssFeedsAsync(ct);
            return Results.Ok(items);
        }
    }
}
