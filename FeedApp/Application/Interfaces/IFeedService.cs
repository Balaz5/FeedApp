using Application.DTOs.Common;
using Application.DTOs.Feeds;

namespace Application.Interfaces
{
    public interface IFeedService
    {
        Task<PagedResponse<FeedResponseDto>> GetFeedsAsync(FeedListRequest request, CancellationToken ct = default);
        Task<FeedResponseDto> GetFeedByIdAsync(Guid id, CancellationToken ct = default);
        Task<FeedResponseDto> CreateFeedAsync(CreateFeedRequest request, Guid userId, CancellationToken ct = default);
        Task<FeedResponseDto> UpdateFeedAsync(Guid id, UpdateFeedRequest request, Guid userId, CancellationToken ct = default);
        Task DeleteFeedAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task<FeedResponseDto> UploadFeedImageAsync(Guid id, Stream imageStream, string contentType, Guid userId, CancellationToken ct = default);
        Task<(byte[] data, string contentType)> GetFeedImageAsync(Guid id, CancellationToken ct = default);
    }
}
