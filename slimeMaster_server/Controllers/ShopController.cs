using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Enum;

namespace slimeMaster_server.Controllers;

[Route("api/shop")]
[ApiController]
public class ShopController : ControllerBase
{
    private IShopService _shopService;

    public ShopController(IShopService shopService)
    {
        _shopService = shopService;
    }
    
    [HttpPost]
    [Authorize]
    [Route($"{nameof(PurchaseItemRequest)}")]
    public async Task<ShopPurchaseResponseBase> PurchaseItemRequest(ShopPurchaseRequestBase request)
    {
        try
        {
            return await _shopService.PurchaseItemRequest(request);
        }
        catch (Exception e)
        {
            return new ShopPurchaseResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }
}