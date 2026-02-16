using FeedApp.Application.DTOs.Auth;
using FeedApp.Application.Interfaces;

namespace FeedApp.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/auth")
                .WithTags("Auth")
                .AllowAnonymous();

            group.MapPost("/register", Register)
                .WithName("Register")
                .WithSummary("Register a new user")
                .Produces<AuthResponseDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPost("/login", Login)
                .WithName("Login")
                .WithSummary("Login with username and password")
                .Produces<AuthResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);

            return group;
        }

        private static async Task<IResult> Register(
            RegisterRequest request,
            IAuthService authService,
            CancellationToken ct = default)
        {
            var result = await authService.RegisterAsync(request, ct);
            return Results.Created($"/api/users/{result.UserId}", result);
        }

        private static async Task<IResult> Login(
            LoginRequest request,
            IAuthService authService,
            CancellationToken ct = default)
        {
            var result = await authService.LoginAsync(request, ct);
            return Results.Ok(result);
        }
    }
}
