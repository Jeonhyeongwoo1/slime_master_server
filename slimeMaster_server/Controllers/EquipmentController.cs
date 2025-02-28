using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Enum;

namespace slimeMaster_server.Controllers;

[Route("api/equip")]
[ApiController]
public class EquipmentController : ControllerBase
{
    private IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }
    
    [HttpPost]
    [Authorize]
    [Route($"{nameof(EquipmentLevelUpRequest)}")]
    public async Task<EquipmentLevelUpResponseBase> EquipmentLevelUpRequest(EquipmentLevelUpRequestBase request)
    {
        try
        {
            return await _equipmentService.EquipmentLevelUpRequest(request);
        }
        catch (Exception e)
        {
            return new EquipmentLevelUpResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(UnequipRequest)}")]
    public async Task<UnequipResponseBase> UnequipRequest(UnequipRequestBase request)
    {   
        try
        {
            return await _equipmentService.UnequipRequest(request);
        }
        catch (Exception e)
        {
            return new UnequipResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(EquipRequest)}")]
    public async Task<EquipResponseBase> EquipRequest(EquipRequestBase request)
    {
        try
        {
            return await _equipmentService.EquipRequest(request);
        }
        catch (Exception e)
        {
            return new EquipResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(MergeEquipmentRequest)}")]
    public async Task<MergeEquipmentResponseBase> MergeEquipmentRequest(MergeEquipmentRequestBase request)
    {
        try
        {
            return await _equipmentService.MergeEquipmentRequest(request);
        }
        catch (Exception e)
        {
            return new MergeEquipmentResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }
}