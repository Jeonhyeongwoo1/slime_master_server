using slimeMaster_server.Models;

namespace slimeMaster_server.Interface;

public interface IAchievementService
{
    Task<AchievementResponseBase> GetAchievementRewardRequest(AchievementRequestBase request);
}