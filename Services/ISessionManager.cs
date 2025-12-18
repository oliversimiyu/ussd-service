using UssdInsuranceService.Models;

namespace UssdInsuranceService.Services;

public interface ISessionManager
{
    UssdSession GetOrCreateSession(string sessionId, string phoneNumber);
    UssdSession? GetSession(string sessionId);
    void UpdateSession(UssdSession session);
    void EndSession(string sessionId);
    void CleanupExpiredSessions();
}
