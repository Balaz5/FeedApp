namespace FeedApp.Application.DTOs
{
    public class FileUploadSettings
    {
        public const string SectionName = "FileUpload";

        public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024; // 5 MB
    }
}
