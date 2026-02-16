using System.ComponentModel.DataAnnotations;

namespace FeedApp.Application.DTOs.Auth
{
    public record LoginRequest
    {
        [Required]
        public string Username { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;
    }
}
