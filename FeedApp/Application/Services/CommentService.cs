using FeedApp.Application.DTOs.Comments;
using FeedApp.Application.DTOs.Common;
using FeedApp.Application.Interfaces;
using FeedApp.Application.Mapping;
using FeedApp.Domain.Entities;
using FeedApp.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FeedApp.Application.Services
{
    public class CommentService(IAppDbContext context, ILogger<CommentService> logger) : ICommentService
    {
        public async Task<PagedResponse<CommentResponseDto>> GetCommentsByFeedIdAsync(
            Guid feedId, PagedRequest request, CancellationToken ct = default)
        {
            logger.LogInformation("Fetching comments for feed {FeedId}", feedId);

            var feedExists = await context.Feeds.AnyAsync(f => f.Id == feedId, ct);
            if (!feedExists)
                throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{feedId}' was not found.");

            var query = context.Comments
                .Include(c => c.User)
                .Where(c => c.FeedId == feedId);

            var totalCount = await query.CountAsync(ct);

            var comments = await query
                .OrderByDescending(c => c.CreatedAtUtc)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            return new PagedResponse<CommentResponseDto>
            {
                Items = comments.Select(c => c.ToResponseDto()).ToList(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<CommentResponseDto> CreateCommentAsync(
            Guid feedId, CreateCommentRequest request, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("User {UserId} creating comment on feed {FeedId}", userId, feedId);

            var feedExists = await context.Feeds.AnyAsync(f => f.Id == feedId, ct);
            if (!feedExists)
                throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{feedId}' was not found.");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = request.Content,
                UserId = userId,
                FeedId = feedId,
                CreatedAtUtc = DateTime.UtcNow
            };

            context.Comments.Add(comment);
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Comment {CommentId} created on feed {FeedId}", comment.Id, feedId);

            // Reload with User for response mapping
            var created = await context.Comments
                .Include(c => c.User)
                .FirstAsync(c => c.Id == comment.Id, ct);

            return created.ToResponseDto();
        }

        public async Task<CommentResponseDto> UpdateCommentAsync(
            Guid commentId, UpdateCommentRequest request, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("User {UserId} updating comment {CommentId}", userId, commentId);

            var comment = await context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId, ct)
                ?? throw new NotFoundException("COMMENT_NOT_FOUND", $"Comment with ID '{commentId}' was not found.");

            if (comment.UserId != userId)
                throw new ForbiddenException("COMMENT_FORBIDDEN", "You can only modify your own comments.");

            comment.Content = request.Content;
            comment.UpdatedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);

            logger.LogInformation("Comment {CommentId} updated", commentId);

            return comment.ToResponseDto();
        }

        public async Task DeleteCommentAsync(Guid commentId, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("User {UserId} deleting comment {CommentId}", userId, commentId);

            var comment = await context.Comments.FindAsync([commentId], ct)
                ?? throw new NotFoundException("COMMENT_NOT_FOUND", $"Comment with ID '{commentId}' was not found.");

            if (comment.UserId != userId)
                throw new ForbiddenException("COMMENT_FORBIDDEN", "You can only delete your own comments.");

            context.Comments.Remove(comment);
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Comment {CommentId} deleted", commentId);
        }
    }
}
