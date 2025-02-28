using slimeMaster_server.Models;

namespace slimeMaster_server.Interface;

public interface IMissionService
{
    Task<GetMissionRewardResponseBase> GetMissionRewardRequest(GetMissionRewardRequest request);
}