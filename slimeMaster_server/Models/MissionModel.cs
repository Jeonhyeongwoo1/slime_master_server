using SlimeMaster.Enum;

namespace slimeMaster_server.Models;

public class GetMissionRewardResponseBase : ResponseBase
{
    public DBMissionContainerData DBMissionContainerData { get; set; }
    public DBItemData DBRewardItemData { get; set; }
}

public class GetMissionRewardRequest : RequestBase
{
    public int missionId { get; set; }
    public MissionType missionType
    {
        get;
        set;
    }
}