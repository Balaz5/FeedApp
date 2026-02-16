using FeedApp.Domain.Entities;
using FeedApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeedApp.Infrastructure.Data.Seed
{
    public static class DataSeeder
    {
        private static readonly Guid User1Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001");
        private static readonly Guid User2Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002");
        private static readonly Guid User3Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003");

        private static readonly Guid Feed1Id = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000001");
        private static readonly Guid Feed2Id = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000002");
        private static readonly Guid Feed3Id = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000003");
        private static readonly Guid Feed4Id = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000004");
        private static readonly Guid Feed5Id = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000005");

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrated successfully");

                if (await context.Users.AnyAsync())
                {
                    logger.LogInformation("Database already seeded, skipping");
                    return;
                }

                await SeedUsersAsync(context);
                await SeedFeedsAsync(context);
                await SeedLikesAsync(context);
                await SeedCommentsAsync(context);

                logger.LogInformation("Database seeded successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private static async Task SeedUsersAsync(AppDbContext context)
        {
            // Password for all seed users: "123456"
            const string passwordHash = "$2a$11$oMfIjuSrA3FuxOtN7.46b.E7AGhwbYlqGoDfHL6sIGR.zVMDJPqou";

            var users = new[]
            {
            new User
            {
                Id = User1Id,
                Username = "johndoe",
                Email = "john@example.com",
                PasswordHash = passwordHash,
                CreatedAtUtc = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = User2Id,
                Username = "janedoe",
                Email = "jane@example.com",
                PasswordHash = passwordHash,
                CreatedAtUtc = new DateTime(2025, 1, 16, 12, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = User3Id,
                Username = "bobsmith",
                Email = "bob@example.com",
                PasswordHash = passwordHash,
                CreatedAtUtc = new DateTime(2025, 2, 1, 8, 0, 0, DateTimeKind.Utc)
            }
        };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        private static async Task SeedFeedsAsync(AppDbContext context)
        {
            var feeds = new Feed[]
            {
            new TextFeed
            {
                Id = Feed1Id,
                Title = "Getting Started with .NET 10",
                Description = "A comprehensive guide to the new features in .NET 10, including improved minimal APIs, enhanced performance, and new language features in C# 14.",
                FeedType = FeedType.Text,
                UserId = User1Id,
                CreatedAtUtc = new DateTime(2025, 3, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new TextFeed
            {
                Id = Feed2Id,
                Title = "Clean Architecture Best Practices",
                Description = "Exploring the principles of Clean Architecture and how to apply them effectively in modern .NET applications.",
                FeedType = FeedType.Text,
                UserId = User1Id,
                CreatedAtUtc = new DateTime(2025, 3, 5, 14, 30, 0, DateTimeKind.Utc)
            },
            new ImageFeed
            {
                Id = Feed3Id,
                Title = "Beautiful Sunset Photo",
                Description = "Captured this amazing sunset at Lake Balaton last weekend.",
                FeedType = FeedType.Image,
                UserId = User2Id,
                ImageData = null, // No actual image in seed data
                ImageMimeType = null,
                CreatedAtUtc = new DateTime(2025, 3, 10, 18, 0, 0, DateTimeKind.Utc)
            },
            new VideoFeed
            {
                Id = Feed4Id,
                Title = "Docker Tutorial for .NET Developers",
                Description = "Step-by-step video tutorial on containerizing .NET applications with Docker and Docker Compose.",
                FeedType = FeedType.Video,
                UserId = User2Id,
                ImageData = null,
                ImageMimeType = null,
                VideoUrl = "https://www.youtube.com/watch?v=example123",
                CreatedAtUtc = new DateTime(2025, 3, 12, 10, 0, 0, DateTimeKind.Utc)
            },
            new TextFeed
            {
                Id = Feed5Id,
                Title = "Entity Framework Core Tips",
                Description = "Top 10 tips for optimizing Entity Framework Core queries and improving database performance in production applications.",
                FeedType = FeedType.Text,
                UserId = User3Id,
                CreatedAtUtc = new DateTime(2025, 3, 15, 11, 0, 0, DateTimeKind.Utc)
            }
            };

            context.Feeds.AddRange(feeds);
            await context.SaveChangesAsync();
        }

        private static async Task SeedLikesAsync(AppDbContext context)
        {
            var likes = new[]
            {
            new Like
            {
                Id = Guid.NewGuid(),
                UserId = User2Id,
                FeedId = Feed1Id,
                CreatedAtUtc = new DateTime(2025, 3, 2, 10, 0, 0, DateTimeKind.Utc)
            },
            new Like
            {
                Id = Guid.NewGuid(),
                UserId = User3Id,
                FeedId = Feed1Id,
                CreatedAtUtc = new DateTime(2025, 3, 2, 12, 0, 0, DateTimeKind.Utc)
            },
            new Like
            {
                Id = Guid.NewGuid(),
                UserId = User1Id,
                FeedId = Feed3Id,
                CreatedAtUtc = new DateTime(2025, 3, 11, 8, 0, 0, DateTimeKind.Utc)
            },
            new Like
            {
                Id = Guid.NewGuid(),
                UserId = User3Id,
                FeedId = Feed4Id,
                CreatedAtUtc = new DateTime(2025, 3, 13, 9, 0, 0, DateTimeKind.Utc)
            },
            new Like
            {
                Id = Guid.NewGuid(),
                UserId = User1Id,
                FeedId = Feed5Id,
                CreatedAtUtc = new DateTime(2025, 3, 16, 7, 0, 0, DateTimeKind.Utc)
            }
        };

            context.Likes.AddRange(likes);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCommentsAsync(AppDbContext context)
        {
            var comments = new[]
            {
            new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Great article! Very helpful for getting started.",
                UserId = User2Id,
                FeedId = Feed1Id,
                CreatedAtUtc = new DateTime(2025, 3, 2, 11, 0, 0, DateTimeKind.Utc)
            },
            new Comment
            {
                Id = Guid.NewGuid(),
                Content = "I love the clean architecture approach.",
                UserId = User3Id,
                FeedId = Feed2Id,
                CreatedAtUtc = new DateTime(2025, 3, 6, 9, 0, 0, DateTimeKind.Utc)
            },
            new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Stunning photo! Lake Balaton is beautiful this time of year.",
                UserId = User1Id,
                FeedId = Feed3Id,
                CreatedAtUtc = new DateTime(2025, 3, 11, 9, 30, 0, DateTimeKind.Utc)
            }
        };

            context.Comments.AddRange(comments);
            await context.SaveChangesAsync();
        }
    }
}
