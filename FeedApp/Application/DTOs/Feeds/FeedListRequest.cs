using FeedApp.Application.DTOs.Common;
using FeedApp.Domain.Enums;

namespace FeedApp.Application.DTOs.Feeds
{
    public record FeedListRequest : PagedRequest
    {
        /// <summary>
        /// Filter feeds by a specific user.
        /// </summary>
        public Guid? UserId { get; init; }

        /// <summary>
        /// Filter feeds by type.
        /// </summary>
        public FeedType? FeedType { get; init; }
    }
}
