using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Feeds
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
