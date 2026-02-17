namespace FeedApp.Application.DTOs.Users
{
    public record UserResponseDto
    {
        public required Guid Id { get; init; }
        public required string Username { get; init; }
        public required string Email { get; init; }
        public required DateTime CreatedAtUtc { get; init; }
        public required int FeedCount { get; init; }
    }
}
