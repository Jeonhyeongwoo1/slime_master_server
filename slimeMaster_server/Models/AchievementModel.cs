namespace slimeMaster_server.Models;

public class AchievementResponseBase : ResponseBase
{
    public DBItemData RewardItemData { get; set; }
    public DBAchievementContainerData DBAchievementContainerData { get; set; }
}

public class AchievementRequestBase : RequestBase
{
    public int achievementId { get; set; }
}