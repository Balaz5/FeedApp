using FeedApp.Api.Extensions;
using FeedApp.Application.DTOs.Comments;
using FeedApp.Application.DTOs.Common;
using FeedApp.Application.Interfaces;
using System.Security.Claims;

namespace FeedApp.Api.Endpoints
{
    public static class CommentEndpoints
    {
        public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder routes)
        {
            // Comments nested under feeds
            var feedComments = routes.MapGroup("/api/feeds/{feedId:guid}/comments")
                .WithTags("Comments")
                .RequireAuthorization();

            feedComments.MapGet("/", GetComments)
                .WithName("GetComments")
                .WithSummary("List comments for a feed")
                .Produces<PagedResponse<CommentResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            feedComments.MapPost("/", CreateComment)
                .WithName("CreateComment")
                .WithSummary("Add a comment to a feed")
                .Produces<CommentResponseDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);

            // Standalone comment operations (edit/delete by comment ID)
            var comments = routes.MapGroup("/api/comments")
                .WithTags("Comments")
                .RequireAuthorization();

            comments.MapPut("/{id:guid}", UpdateComment)
                .WithName("UpdateComment")
                .WithSummary("Update a comment (owner only)")
                .Produces<CommentResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound);

            comments.MapDelete("/{id:guid}", DeleteComment)
                .WithName("DeleteComment")
                .WithSummary("Delete a comment (owner only)")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound);

            return routes;
        }

        private static async Task<IResult> GetComments(
            Guid feedId,
            ICommentService commentService,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default)
        {
            var request = new PagedRequest { Page = page, PageSize = pageSize };
            var result = await commentService.GetCommentsByFeedIdAsync(feedId, request, ct);
            return Results.Ok(result);
        }

        private static async Task<IResult> CreateComment(
            Guid feedId,
            CreateCommentRequest request,
            ClaimsPrincipal user,
            ICommentService commentService,
            CancellationToken ct = default)
        {
            var userId = user.GetUserId();
            var result = await commentService.CreateCommentAsync(feedId, request, userId, ct);
            return Results.Created($"/api/comments/{result.Id}", result);
        }

        private static async Task<IResult> UpdateComment(
            Guid id,
            UpdateCommentRequest request,
            ClaimsPrincipal user,
            ICommentService commentService,
            CancellationToken ct = default)
        {
            var userId = user.GetUserId();
            var result = await commentService.UpdateCommentAsync(id, request, userId, ct);
            return Results.Ok(result);
        }

        private static async Task<IResult> DeleteComment(
            Guid id,
            ClaimsPrincipal user,
            ICommentService commentService,
            CancellationToken ct = default)
        {
            var userId = user.GetUserId();
            await commentService.DeleteCommentAsync(id, userId, ct);
            return Results.NoContent();
        }
    }
}
