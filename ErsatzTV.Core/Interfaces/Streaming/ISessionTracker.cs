#nullable enable

using ErsatzTV.Core.Domain;
using ErsatzTV.Core.Api.Sessions;

namespace ErsatzTV.Core.Interfaces.Streaming;

public interface ISessionTracker : IDisposable
{
    void TrackSession(string sessionId, int channelId, string clientIp, string userAgent);
    void UpdateNowPlaying(string sessionId, MediaItem mediaItem, DateTime startedAt, TimeSpan duration);
    void UpdatePosition(string sessionId, TimeSpan position);
    void EndSession(string sessionId);
    NowPlayingResponseModel? GetNowPlayingForChannel(int channelId);
    List<ActiveSessionModel> GetActiveSessions();
    void CleanupStaleSessions(TimeSpan staleThreshold);
}

public record ActiveSessionModel(
    string SessionId,
    int ChannelId,
    string ClientIp,
    string UserAgent,
    DateTime StartedAt,
    DateTime LastActivityAt,
    NowPlayingResponseModel? NowPlaying);
