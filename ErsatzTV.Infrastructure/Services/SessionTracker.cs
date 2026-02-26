#nullable enable

using System.Collections.Concurrent;
using ErsatzTV.Core.Domain;
using ErsatzTV.Core.Interfaces.Streaming;
using ErsatzTV.Core.Api.Sessions;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ErsatzTV.Infrastructure.Services;

public class SessionTracker : ISessionTracker, IDisposable
{
    private readonly ConcurrentDictionary<string, SessionInfo> _sessions = new();
    private readonly ILogger<SessionTracker> _logger;
    private readonly Timer _cleanupTimer;
    private bool _disposed;

    public SessionTracker(ILogger<SessionTracker> logger)
    {
        _logger = logger;
        _cleanupTimer = new Timer(_ => CleanupStaleSessions(TimeSpan.FromMinutes(5)), 
            null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    private class SessionInfo
    {
        public string SessionId { get; set; } = string.Empty;
        public int ChannelId { get; set; }
        public string ClientIp { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public int? CurrentMediaId { get; set; }
        public string? CurrentMediaType { get; set; }
        public DateTime? MediaStartedAt { get; set; }
        public TimeSpan? MediaDuration { get; set; }
        public TimeSpan? LastKnownPosition { get; set; }
    }

    public void TrackSession(string sessionId, int channelId, string clientIp, string userAgent)
    {
        var session = new SessionInfo
        {
            SessionId = sessionId,
            ChannelId = channelId,
            ClientIp = clientIp,
            UserAgent = userAgent,
            StartedAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow
        };

        _sessions.TryAdd(sessionId, session);
        _logger.LogInformation("Session tracked: {SessionId} for channel {ChannelId}", sessionId, channelId);
    }

    public void UpdateNowPlaying(string sessionId, MediaItem mediaItem, DateTime startedAt, TimeSpan duration)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            session.CurrentMediaId = mediaItem.Id;
            session.CurrentMediaType = mediaItem.GetType().Name;
            session.MediaStartedAt = startedAt;
            session.MediaDuration = duration;
            session.LastActivityAt = DateTime.UtcNow;
        }
    }

    public void UpdatePosition(string sessionId, TimeSpan position)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            session.LastKnownPosition = position;
            session.LastActivityAt = DateTime.UtcNow;
        }
    }

    public void EndSession(string sessionId)
    {
        _sessions.TryRemove(sessionId, out _);
        _logger.LogInformation("Session ended: {SessionId}", sessionId);
    }

    public NowPlayingResponseModel? GetNowPlayingForChannel(int channelId)
    {
        var session = _sessions.Values
            .Where(s => s.ChannelId == channelId && s.CurrentMediaId != null)
            .OrderByDescending(s => s.LastActivityAt)
            .FirstOrDefault();

        if (session?.CurrentMediaId == null || session.MediaStartedAt == null) 
            return null;

        var now = DateTime.UtcNow;
        var elapsed = now - session.MediaStartedAt.Value;
        var position = session.LastKnownPosition ?? elapsed;
        var duration = session.MediaDuration ?? TimeSpan.Zero;
        var remaining = duration > position ? duration - position : TimeSpan.Zero;
        var progress = duration.TotalSeconds > 0 ? (position.TotalSeconds / duration.TotalSeconds) * 100 : 0;

        return new NowPlayingResponseModel(
            session.ChannelId,
            $"Channel {session.ChannelId}",
            session.ChannelId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            new NowPlayingMediaInfo(
                session.CurrentMediaId.Value,
                session.CurrentMediaType ?? "Unknown",
                "Unknown Title",
                null,
                null,
                null,
                null,
                null
            ),
            session.MediaStartedAt,
            position,
            duration,
            remaining,
            progress,
            $"/iptv/channel/{session.ChannelId}.m3u8",
            PlaybackState.Playing
        );
    }

    public List<ActiveSessionModel> GetActiveSessions()
    {
        return _sessions.Values.Select(s => new ActiveSessionModel(
            s.SessionId,
            s.ChannelId,
            s.ClientIp,
            s.UserAgent,
            s.StartedAt,
            s.LastActivityAt,
            s.CurrentMediaId != null ? GetNowPlayingForChannel(s.ChannelId) : null
        )).ToList();
    }

    public void CleanupStaleSessions(TimeSpan staleThreshold)
    {
        var cutoff = DateTime.UtcNow - staleThreshold;
        var staleSessions = _sessions.Values
            .Where(s => s.LastActivityAt < cutoff)
            .Select(s => s.SessionId)
            .ToList();

        foreach (var sessionId in staleSessions)
        {
            _sessions.TryRemove(sessionId, out _);
            _logger.LogInformation("Cleaned up stale session: {SessionId}", sessionId);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cleanupTimer?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
