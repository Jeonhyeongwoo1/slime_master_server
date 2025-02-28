namespace slimeMaster_server.Models;

public class OfflineRewardResponseBase : ResponseBase
{
    public DBItemData DBRewardItemData { get; set; }
    public DateTime LastGetOfflineRewardTime { get; set; }
}
    
public class FastRewardResponseBase : ResponseBase
{
    public List<DBItemData> DBItemDataList { get; set; }
    public DBEquipmentData DBEquipmentData { get; set; }
}
