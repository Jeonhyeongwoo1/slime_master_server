using slimeMaster_server.Models;

namespace slimeMaster_server.Interface;

public interface IEquipmentService
{
    Task<EquipmentLevelUpResponseBase> EquipmentLevelUpRequest(EquipmentLevelUpRequestBase request);
    Task<UnequipResponseBase> UnequipRequest(UnequipRequestBase request);
    Task<EquipResponseBase> EquipRequest(EquipRequestBase request);
    Task<MergeEquipmentResponseBase> MergeEquipmentRequest(MergeEquipmentRequestBase request);
}