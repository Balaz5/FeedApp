using System.Text.Json.Serialization;

namespace FeedApp.Application.DTOs.Common
{
    public record ErrorResponse
    {
        public required string ErrorCode { get; init; }
        public required string Message { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Details { get; init; }

        public required string TraceId { get; init; }
    }
}
