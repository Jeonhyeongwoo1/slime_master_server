using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Enum;

namespace slimeMaster_server.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(LoadUserDataRequest)}")]
    public async Task<UserResponseBase> LoadUserDataRequest([FromBody] UserRequest request)
    {
        try
        {
            return await _userService.LoadUserDataRequest(request);
        }
        catch (Exception e)
        {
            return new UserResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(UseStaminaRequest)}")]
    public async Task<UserResponseBase> UseStaminaRequest(UseStaminaRequest request)
    {
        try
        {
            return await _userService.UseStaminaRequest(request);
        }
        catch (Exception e)
        {
            return new UserResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(StageClearRequest)}")]
    public async Task<StageClearResponseBase> StageClearRequest(StageClearRequest request)
    {
        
        try
        {
            return await _userService.StageClearRequest(request);
        }
        catch (Exception e)
        {
            return new StageClearResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(GetWaveClearRewardRequest)}")]
    public async Task<RewardResponseBase> GetWaveClearRewardRequest(GetWaveClearRewardRequest request)
    {
        try
        {
            return await _userService.GetWaveClearRewardRequest(request);
        }
        catch (Exception e)
        {
            return new RewardResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(CopyNewUser)}")]
    public async Task<ResponseBase> CopyNewUser(RequestBase request)
    {
        try
        {
            return await _userService.CopyNewUser(request);
        }
        catch (Exception e)
        {
            return new RewardResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }
}