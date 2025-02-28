using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Enum;

namespace slimeMaster_server.Controllers;

[Route("api/offline")]
[ApiController]
public class OfflineController : ControllerBase
{
    private IOfflineService _offlineService;

    public OfflineController(IOfflineService offlineService)
    {
        _offlineService = offlineService;
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(GetOfflineRewardRequest)}")]
    public async Task<OfflineRewardResponseBase> GetOfflineRewardRequest(RequestBase request)
    {
        try
        {
            return await _offlineService.GetOfflineRewardRequest(request);
        }
        catch (Exception e)
        {
            return new OfflineRewardResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(GetFastRewardRequest)}")]
    public async Task<FastRewardResponseBase> GetFastRewardRequest(RequestBase request)
    {
        try
        {
            return await _offlineService.GetFastRewardRequest(request);
        }
        catch (Exception e)
        {
            return new FastRewardResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }
}