namespace Domain.Entities
{
    public class ImageFeed : Feed
    {
        public byte[]? ImageData { get; set; }
        public string? ImageMimeType { get; set; }
    }
}
