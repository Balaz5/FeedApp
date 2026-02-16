namespace FeedApp.Application.DTOs.Auth
{
    public record AuthResponseDto
    {
        public required Guid UserId { get; init; }
        public required string Username { get; init; }
        public required string Email { get; init; }
        public required string Token { get; init; }
        public required DateTime ExpiresAtUtc { get; init; }
    }
}
