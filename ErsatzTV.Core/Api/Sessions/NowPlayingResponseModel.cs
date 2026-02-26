#nullable enable

using System.Text.Json.Serialization;

namespace ErsatzTV.Core.Api.Sessions;

public record NowPlayingResponseModel(
    int ChannelId,
    string ChannelName,
    string ChannelNumber,
    NowPlayingMediaInfo? CurrentMedia,
    DateTime? StartedAt,
    TimeSpan? PlaybackPosition,
    TimeSpan? Duration,
    TimeSpan? TimeRemaining,
    double? ProgressPercentage,
    string? StreamUrl,
    PlaybackState State);

public record NowPlayingMediaInfo(
    int Id,
    string Type,
    string Title,
    string? Artist,
    string? Album,
    string? FilePath,
    string? Poster,
    Dictionary<string, object>? Metadata);

public enum PlaybackState
{
    Playing,
    Paused,
    Buffering,
    Ended,
    Unknown
}
