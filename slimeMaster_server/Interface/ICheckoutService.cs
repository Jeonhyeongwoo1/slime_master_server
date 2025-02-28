using slimeMaster_server.Models;

namespace slimeMaster_server.Interface;

public interface ICheckoutService
{
    public Task<GetCheckoutRewardResponseBase> GetCheckoutRewardRequest(GetCheckoutRewardRequestBase requestBase);
}