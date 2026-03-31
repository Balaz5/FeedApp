using FeedApp.Application.DTOs.Comments;
using FeedApp.Application.DTOs.Common;
using FeedApp.Application.Interfaces;
using FeedApp.Application.Interfaces.Repositories;
using FeedApp.Application.Mapping;
using FeedApp.Domain.Entities;
using FeedApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FeedApp.Application.Services
{
    public class CommentService(
        ICommentRepository commentRepository,
        IFeedRepository feedRepository,
        IUnitOfWork unitOfWork,
        ILogger<CommentService> logger) : ICommentService
    {
        public async Task<PagedResponse<CommentResponseDto>> GetCommentsByFeedIdAsync(
            Guid feedId, PagedRequest request, CancellationToken ct = default)
        {
            logger.LogInformation("Fetching comments for feed {FeedId}", feedId);

            if (!await feedRepository.ExistsAsync(feedId, ct))
                throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{feedId}' was not found.");

            var (items, totalCount) = await commentRepository.GetPagedByFeedIdAsync(
                feedId, request.Page, request.PageSize, ct);

            return new PagedResponse<CommentResponseDto>
            {
                Items = items.Select(c => c.ToDto()).ToList(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<CommentResponseDto> CreateCommentAsync(
            Guid feedId, CreateCommentRequest request, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("User {UserId} creating comment on feed {FeedId}", userId, feedId);

            if (!await feedRepository.ExistsAsync(feedId, ct))
                throw new NotFoundException("FEED_NOT_FOUND", $"Feed with ID '{feedId}' was not found.");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = request.Content,
                UserId = userId,
                FeedId = feedId,
                CreatedAtUtc = DateTime.UtcNow
            };

            commentRepository.Add(comment);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Comment {CommentId} created on feed {FeedId}", comment.Id, feedId);

            var details = await commentRepository.GetDetailsByIdAsync(comment.Id, ct);
            return details!.ToDto();
        }

        public async Task<CommentResponseDto> UpdateCommentAsync(
            Guid commentId, UpdateCommentRequest request, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("User {UserId} updating comment {CommentId}", userId, commentId);

            var comment = await commentRepository.GetByIdAsync(commentId, ct)
                ?? throw new NotFoundException("COMMENT_NOT_FOUND", $"Comment with ID '{commentId}' was not found.");

            if (comment.UserId != userId)
                throw new ForbiddenException("COMMENT_FORBIDDEN", "You can only modify your own comments.");

            comment.Content = request.Content;
            comment.UpdatedAtUtc = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Comment {CommentId} updated", commentId);

            var details = await commentRepository.GetDetailsByIdAsync(commentId, ct);
            return details!.ToDto();
        }

        public async Task DeleteCommentAsync(Guid commentId, Guid userId, CancellationToken ct = default)
        {
            logger.LogInformation("User {UserId} deleting comment {CommentId}", userId, commentId);

            var comment = await commentRepository.GetByIdAsync(commentId, ct)
                ?? throw new NotFoundException("COMMENT_NOT_FOUND", $"Comment with ID '{commentId}' was not found.");

            if (comment.UserId != userId)
                throw new ForbiddenException("COMMENT_FORBIDDEN", "You can only delete your own comments.");

            commentRepository.Remove(comment);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Comment {CommentId} deleted", commentId);
        }
    }
}
