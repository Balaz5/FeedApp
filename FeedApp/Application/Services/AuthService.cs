using FeedApp.Application.DTOs.Auth;
using FeedApp.Application.Interfaces;
using FeedApp.Domain.Entities;
using FeedApp.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FeedApp.Application.Services
{
    public class AuthService(
        IAppDbContext context,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            logger.LogInformation("Registering user {Username}", request.Username);

            // Check for duplicate username
            if (await context.Users.AnyAsync(u => u.Username == request.Username, ct))
                throw new ConflictException("USERNAME_TAKEN", $"Username '{request.Username}' is already taken.");

            // Check for duplicate email
            if (await context.Users.AnyAsync(u => u.Email == request.Email, ct))
                throw new ConflictException("EMAIL_TAKEN", $"Email '{request.Email}' is already registered.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAtUtc = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync(ct);

            logger.LogInformation("User {UserId} registered successfully", user.Id);

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token.token,
                ExpiresAtUtc = token.expiresAt
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            logger.LogInformation("Login attempt for user {Username}", request.Username);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username, ct)
                ?? throw new UnauthorizedException("INVALID_CREDENTIALS", "Invalid username or password.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedException("INVALID_CREDENTIALS", "Invalid username or password.");

            logger.LogInformation("User {UserId} logged in successfully", user.Id);

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token.token,
                ExpiresAtUtc = token.expiresAt
            };
        }

        private (string token, DateTime expiresAt) GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
