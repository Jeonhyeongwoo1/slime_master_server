using slimeMaster_server.Models;

namespace slimeMaster_server.Interface;

public interface IOfflineService
{
    Task<OfflineRewardResponseBase> GetOfflineRewardRequest(RequestBase request);
    Task<FastRewardResponseBase> GetFastRewardRequest(RequestBase request);
}