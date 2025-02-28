using slimeMaster_server.Models;

namespace slimeMaster_server.Interface;

public interface IShopService
{
    public Task<ShopPurchaseResponseBase> PurchaseItemRequest(ShopPurchaseRequestBase request);
}