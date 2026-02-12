namespace Domain.Entities
{
    public class VideoFeed : Feed
    {
        public byte[]? ImageData { get; set; }
        public string? ImageMimeType { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
    }
}
