using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Enum;

namespace slimeMaster_server.Controllers;

[Route("api/achievement")]
[ApiController]
public class AchievementController : ControllerBase
{
    private IAchievementService _achievementService;
    
    public AchievementController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }
    
    [HttpPost]
    [Authorize]
    [Route($"{nameof(GetAchievementRewardRequest)}")]
    public async Task<AchievementResponseBase> GetAchievementRewardRequest(AchievementRequestBase request)
    {
        try
        {
            return await _achievementService.GetAchievementRewardRequest(request);
        }
        catch (Exception e)
        {
            return new AchievementResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }
}