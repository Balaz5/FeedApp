using System.ComponentModel.DataAnnotations;

namespace FeedApp.Application.DTOs.Auth
{
    public record RegisterRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Username { get; init; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; init; } = string.Empty;

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; init; } = string.Empty;
    }
}
