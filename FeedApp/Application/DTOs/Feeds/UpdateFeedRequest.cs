using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Feeds
{
    public record UpdateFeedRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; init; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Only applicable for Video feeds.
        /// </summary>
        [MaxLength(2000)]
        [Url]
        public string? VideoUrl { get; init; }
    }
}
