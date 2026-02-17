using FeedApp.Api.Extensions;
using FeedApp.Application.DTOs.Common;
using FeedApp.Application.DTOs.Feeds;
using FeedApp.Application.Interfaces;
using FeedApp.Domain.Enums;
using System.Security.Claims;

namespace FeedApp.Api.Endpoints
{
    public static class FeedEndpoints
    {
        public static RouteGroupBuilder MapFeedEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/feeds")
                .WithTags("Feeds")
                .RequireAuthorization();

            group.MapGet("/", GetFeeds)
                .WithName("GetFeeds")
                .WithSummary("List feeds with pagination and optional filters")
                .Produces<PagedResponse<FeedResponseDto>>(StatusCodes.Status200OK);

            group.MapGet("/{id:guid}", GetFeedById)
                .WithName("GetFeedById")
                .WithSummary("Get a feed by ID")
                .Produces<FeedResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", CreateFeed)
                .WithName("CreateFeed")
                .WithSummary("Create a new feed")
                .Produces<FeedResponseDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("/{id:guid}", UpdateFeed)
                .WithName("UpdateFeed")
                .WithSummary("Update an existing feed (owner only)")
                .Produces<FeedResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/{id:guid}", DeleteFeed)
                .WithName("DeleteFeed")
                .WithSummary("Soft-delete a feed (owner only)")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut("/{id:guid}/image", UploadImage)
                .WithName("UploadFeedImage")
                .WithSummary("Upload an image for an Image or Video feed (owner only)")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces<FeedResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .DisableAntiforgery();

            group.MapGet("/{id:guid}/image", GetImage)
                .WithName("GetFeedImage")
                .WithSummary("Get the image of an Image or Video feed")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            return group;
        }

        private static async Task<IResult> GetFeeds(
            IFeedService feedService,
            int page = 1,
            int pageSize = 20,
            Guid? userId = null,
            FeedType? feedType = null,
            CancellationToken ct = default)
        {
            var request = new FeedListRequest
            {
                Page = page,
                PageSize = pageSize,
                UserId = userId,
                FeedType = feedType
            };

            var result = await feedService.GetFeedsAsync(request, ct);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetFeedById(
            Guid id,
            IFeedService feedService,
            CancellationToken ct = default)
        {
            var result = await feedService.GetFeedByIdAsync(id, ct);
            return Results.Ok(result);
        }

        private static async Task<IResult> CreateFeed(
            CreateFeedRequest request,
            ClaimsPrincipal user,
            IFeedService feedService,
            CancellationToken ct = default)
        {
            var userId = user.GetUserId();
            var result = await feedService.CreateFeedAsync(request, userId, ct);
            return Results.Created($"/api/feeds/{result.Id}", result);
        }

        private static async Task<IResult> UpdateFeed(
            Guid id,
            UpdateFeedRequest request,
            ClaimsPrincipal user,
            IFeedService feedService,
            CancellationToken ct = default)
        {
            var userId = user.GetUserId();
            var result = await feedService.UpdateFeedAsync(id, request, userId, ct);
            return Results.Ok(result);
        }

        private static async Task<IResult> DeleteFeed(
            Guid id,
            ClaimsPrincipal user,
            IFeedService feedService,
            CancellationToken ct = default)
        {
            var userId = user.GetUserId();
            await feedService.DeleteFeedAsync(id, userId, ct);
            return Results.NoContent();
        }

        private static async Task<IResult> UploadImage(
            Guid id,
            IFormFile file,
            ClaimsPrincipal user,
            IFeedService feedService,
            CancellationToken ct = default)
        {
            var userId = user.GetUserId();

            if (file.Length == 0)
                return Results.BadRequest(new { ErrorCode = "VALIDATION_ERROR", Message = "File is empty." });

            if (!file.ContentType.StartsWith("image/"))
                return Results.BadRequest(new { ErrorCode = "VALIDATION_ERROR", Message = "File must be an image." });

            using var stream = file.OpenReadStream();
            var result = await feedService.UploadFeedImageAsync(id, stream, file.ContentType, userId, ct);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetImage(
            Guid id,
            IFeedService feedService,
            CancellationToken ct = default)
        {
            var (data, contentType) = await feedService.GetFeedImageAsync(id, ct);
            return Results.File(data, contentType);
        }
    }
}
