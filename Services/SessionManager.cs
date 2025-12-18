using System.Collections.Concurrent;
using UssdInsuranceService.Models;

namespace UssdInsuranceService.Services;

public class SessionManager : ISessionManager
{
    private readonly ConcurrentDictionary<string, UssdSession> _sessions = new();
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(5);

    public UssdSession GetOrCreateSession(string sessionId, string phoneNumber)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            session.LastAccessedAt = DateTime.UtcNow;
            return session;
        }

        var newSession = new UssdSession
        {
            SessionId = sessionId,
            PhoneNumber = phoneNumber,
            CurrentMenu = "main",
            CreatedAt = DateTime.UtcNow,
            LastAccessedAt = DateTime.UtcNow
        };

        _sessions.TryAdd(sessionId, newSession);
        return newSession;
    }

    public UssdSession? GetSession(string sessionId)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return session;
    }

    public void UpdateSession(UssdSession session)
    {
        session.LastAccessedAt = DateTime.UtcNow;
        _sessions[session.SessionId] = session;
    }

    public void EndSession(string sessionId)
    {
        _sessions.TryRemove(sessionId, out _);
    }

    public void CleanupExpiredSessions()
    {
        var now = DateTime.UtcNow;
        var expiredSessions = _sessions
            .Where(s => now - s.Value.LastAccessedAt > _sessionTimeout)
            .Select(s => s.Key)
            .ToList();

        foreach (var sessionId in expiredSessions)
        {
            _sessions.TryRemove(sessionId, out _);
        }
    }
}
