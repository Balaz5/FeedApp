using Domain.Enums;

namespace Domain.Entities
{
    public abstract class Feed
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeedType FeedType { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Like> Likes { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}
