using slimeMaster_server.Models;

namespace slimeMaster_server.Interface;

public interface IUserService
{
    Task<UserResponseBase> LoadUserDataRequest(UserRequest request);
    Task<UserResponseBase> UseStaminaRequest(UseStaminaRequest request);
    Task<StageClearResponseBase> StageClearRequest(StageClearRequest request);
    Task<RewardResponseBase> GetWaveClearRewardRequest(GetWaveClearRewardRequest request);
    Task<ResponseBase> CopyNewUser(RequestBase requestBase);
}