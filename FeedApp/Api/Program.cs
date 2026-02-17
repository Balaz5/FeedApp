using FeedApp.Api.Endpoints;
using FeedApp.Api.Middleware;
using FeedApp.Application.DTOs.Auth;
using FeedApp.Application.Interfaces;
using FeedApp.Application.Services;
using FeedApp.Infrastructure.Data;
using FeedApp.Infrastructure.Data.Seed;
using FeedApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Text;

namespace FeedApp.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Serilog Bootstrap
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting FeedApp API");

                var builder = WebApplication.CreateBuilder(args);

                // Replace default logging with Serilog
                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "FeedApp")
                    .WriteTo.Console(outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}  {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File("logs/feedapp-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 30,
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}"));

                // Database
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

                // Application Services
                builder.Services.AddScoped<IFeedService, FeedService>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<ILikeService, LikeService>();
                builder.Services.AddScoped<ICommentService, CommentService>();
                builder.Services.AddHttpClient<IRssFeedClient, RssFeedClient>();

                // Built-in validation support for Minimal APIs
                builder.Services.AddValidation();

                // JWT Authentication
                var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName);
                builder.Services.Configure<JwtSettings>(jwtSettings);

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.UseSecurityTokenValidators = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

                builder.Services.AddAuthorization();

                // Swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "FeedApp API",
                        Version = "v1",
                        Description = "REST API for managing users, feeds, likes, and comments"
                    });

                    // Add JWT auth support in Swagger UI
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter your JWT token"
                    });

                    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
                });

                var app = builder.Build();

                // Seed Database
                await DataSeeder.SeedAsync(app.Services);

                // Middleware Pipeline
                app.UseSerilogRequestLogging();
                app.UseMiddleware<ExceptionHandlingMiddleware>();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthentication();
                app.UseAuthorization();

                // Endpoints
                app.MapAuthEndpoints();
                app.MapUserEndpoints();
                app.MapFeedEndpoints();
                app.MapLikeEndpoints();
                app.MapCommentEndpoints();
                app.MapRssEndpoints();

                // Minimal health check endpoint
                app.MapGet("/", () => Results.Ok(new { Status = "Running", Timestamp = DateTime.UtcNow }))
                    .WithName("HealthCheck")
                    .WithTags("Health");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
