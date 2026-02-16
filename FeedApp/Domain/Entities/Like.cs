namespace FeedApp.Domain.Entities
{
    public class Like
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FeedId { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Feed Feed { get; set; } = null!;
    }
}
