using FeedApp.Application.Interfaces;
using FeedApp.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FeedApp.Infrastructure.Services
{
    public class SoftDeleteCleanupJob(
        IServiceScopeFactory scopeFactory,
        ILogger<SoftDeleteCleanupJob> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("SoftDeleteCleanupJob started. Runs daily at midnight UTC");

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilMidnightUtc();
                logger.LogDebug("Next cleanup scheduled in {Delay}", delay);

                await Task.Delay(delay, stoppingToken);

                try
                {
                    await CleanupAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during soft-delete cleanup");
                }
            }
        }

        private static TimeSpan GetDelayUntilMidnightUtc()
        {
            var now = DateTime.UtcNow;
            var nextMidnight = now.Date.AddDays(1);
            return nextMidnight - now;
        }

        private async Task CleanupAsync(CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();
            var feedRepository = scope.ServiceProvider.GetRequiredService<IFeedRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var deletedFeeds = await feedRepository.GetSoftDeletedAsync(ct);

            if (deletedFeeds.Count == 0)
            {
                logger.LogDebug("No soft-deleted feeds found");
                return;
            }

            feedRepository.RemoveRange(deletedFeeds);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Permanently deleted {Count} soft-deleted feeds", deletedFeeds.Count);
        }
    }
}
