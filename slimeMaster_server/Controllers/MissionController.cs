using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Enum;

namespace slimeMaster_server.Controllers;

[Route("api/mission")]
[ApiController]
public class MissionController : ControllerBase
{
    private IMissionService _missionService;

    public MissionController(IMissionService missionService)
    {
        _missionService = missionService;
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(GetMissionRewardRequest)}")]
    public async Task<GetMissionRewardResponseBase> GetMissionRewardRequest(GetMissionRewardRequest request)
    {
        try
        {
            return await _missionService.GetMissionRewardRequest(request);
        }
        catch (Exception e)
        {
            return new GetMissionRewardResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }
}