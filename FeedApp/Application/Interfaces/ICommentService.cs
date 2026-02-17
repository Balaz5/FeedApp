using FeedApp.Application.DTOs.Comments;
using FeedApp.Application.DTOs.Common;

namespace FeedApp.Application.Interfaces
{
    public interface ICommentService
    {
        Task<PagedResponse<CommentResponseDto>> GetCommentsByFeedIdAsync(Guid feedId, PagedRequest request, CancellationToken ct = default);
        Task<CommentResponseDto> CreateCommentAsync(Guid feedId, CreateCommentRequest request, Guid userId, CancellationToken ct = default);
        Task<CommentResponseDto> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request, Guid userId, CancellationToken ct = default);
        Task DeleteCommentAsync(Guid commentId, Guid userId, CancellationToken ct = default);
    }
}
