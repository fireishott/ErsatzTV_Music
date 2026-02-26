#nullable enable

using ErsatzTV.Core.Interfaces.Streaming;
using ErsatzTV.Core.Api.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace ErsatzTV.Controllers.Api;

[ApiController]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
    private readonly ISessionTracker _sessionTracker;
    private readonly ILogger<SessionController> _logger;

    public SessionController(ISessionTracker sessionTracker, ILogger<SessionController> logger)
    {
        _sessionTracker = sessionTracker;
        _logger = logger;
    }

    [HttpGet("now/{channelId}")]
    public IActionResult GetNowPlaying(int channelId)
    {
        var nowPlaying = _sessionTracker.GetNowPlayingForChannel(channelId);
        
        if (nowPlaying == null)
            return NotFound(new { message = $"No active stream for channel {channelId}" });

        return Ok(nowPlaying);
    }

    [HttpGet("now")]
    public IActionResult GetAllNowPlaying()
    {
        var sessions = _sessionTracker.GetActiveSessions();
        var nowPlaying = sessions
            .Where(s => s.NowPlaying != null)
            .Select(s => s.NowPlaying!)
            .GroupBy(np => np.ChannelId)
            .Select(g => g.OrderByDescending(np => np.StartedAt).First())
            .ToList();

        return Ok(nowPlaying);
    }

    [HttpGet("active")]
    public IActionResult GetActiveSessions()
    {
        return Ok(_sessionTracker.GetActiveSessions());
    }

    [HttpDelete("session/{sessionId}")]
    public IActionResult EndSession(string sessionId)
    {
        _sessionTracker.EndSession(sessionId);
        return Ok(new { message = $"Session {sessionId} ended" });
    }
}
