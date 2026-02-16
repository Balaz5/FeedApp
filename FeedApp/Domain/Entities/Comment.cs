namespace FeedApp.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid FeedId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Feed Feed { get; set; } = null!;
    }
}
