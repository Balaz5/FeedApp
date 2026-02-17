using FeedApp.Application.DTOs.Feeds;
using FeedApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace FeedApp.Infrastructure.Services
{
    public class RssFeedClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<RssFeedClient> logger) : IRssFeedClient
    {
        public async Task<IReadOnlyList<RssFeedItemDto>> FetchRssFeedsAsync(CancellationToken cancellationToken = default)
        {
            var feedUrl = configuration["RssFeed:Url"];

            if (string.IsNullOrWhiteSpace(feedUrl))
            {
                logger.LogWarning("RSS feed URL is not configured. Set 'RssFeed:Url' in appsettings.json");
                return [];
            }

            try
            {
                logger.LogInformation("Fetching RSS feed from {Url}", feedUrl);

                var response = await httpClient.GetStringAsync(feedUrl, cancellationToken);
                var doc = XDocument.Parse(response);

                var items = doc.Descendants("item")
                    .Select(item => new RssFeedItemDto
                    {
                        Title = item.Element("title")?.Value ?? string.Empty,
                        Description = StripHtml(item.Element("description")?.Value ?? string.Empty),
                        Link = item.Element("link")?.Value,
                        PublishedAt = TryParseDate(item.Element("pubDate")?.Value),
                        Source = ExtractSource(feedUrl)
                    })
                    .ToList();

                logger.LogInformation("Fetched {Count} items from RSS feed", items.Count);

                return items;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Failed to fetch RSS feed from {Url}", feedUrl);
                throw;
            }
            catch (System.Xml.XmlException ex)
            {
                logger.LogError(ex, "Failed to parse RSS feed XML from {Url}", feedUrl);
                throw;
            }
        }

        private static DateTime? TryParseDate(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            return DateTime.TryParse(dateString, out var date) ? date.ToUniversalTime() : null;
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return html;

            // Simple HTML tag removal — adequate for RSS descriptions
            return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", string.Empty).Trim();
        }

        private static string ExtractSource(string url)
        {
            try
            {
                return new Uri(url).Host;
            }
            catch
            {
                return "RSS";
            }
        }
    }
}
