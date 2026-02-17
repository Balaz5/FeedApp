namespace FeedApp.Application.DTOs.Feeds
{
    public record RssFeedItemDto
    {
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string? Link { get; init; }
        public DateTime? PublishedAt { get; init; }
        public string Source { get; init; } = "RSS";
    }
}
