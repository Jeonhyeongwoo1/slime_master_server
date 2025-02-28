using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Enum;

namespace slimeMaster_server.Controllers;

[Route("api/checkout")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost]
    [Authorize]
    [Route($"{nameof(GetCheckoutRewardRequest)}")]
    public async Task<GetCheckoutRewardResponseBase> GetCheckoutRewardRequest(GetCheckoutRewardRequestBase request)
    {
        try
        {
            return await _checkoutService.GetCheckoutRewardRequest(request);
        }
        catch (Exception e)
        {
            return new GetCheckoutRewardResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }
}