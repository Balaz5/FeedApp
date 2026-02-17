using FeedApp.Application.DTOs.Feeds;

namespace FeedApp.Application.Interfaces
{
    public interface IRssFeedClient
    {
        Task<IReadOnlyList<RssFeedItemDto>> FetchRssFeedsAsync(CancellationToken cancellationToken = default);
    }
}
