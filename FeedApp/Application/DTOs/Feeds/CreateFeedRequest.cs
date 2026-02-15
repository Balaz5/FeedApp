using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Feeds
{
    public record CreateFeedRequest
    {
        [Required]
        public FeedType FeedType { get; init; }

        [Required]
        [MaxLength(200)]
        public string Title { get; init; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Required for Video feeds. Must be a valid URL.
        /// </summary>
        [MaxLength(2000)]
        [Url]
        public string? VideoUrl { get; init; }
    }
}
