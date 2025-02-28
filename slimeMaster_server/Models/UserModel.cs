using Google.Cloud.Firestore;
using SlimeMaster.Enum;

namespace slimeMaster_server.Models;

public class UserResponseBase : ResponseBase
{
    public DBUserData DBUserData { get; set; }
    public DBCheckoutData DBCheckoutData { get; set; }
    public DBMissionContainerData DBMissionContainerData { get; set; }
    public DBAchievementContainerData DBAchievementContainerData { get; set; }
    public DateTime LastLoginTime = DateTime.UtcNow;
    public DateTime LastOfflineGetRewardTime = DateTime.UtcNow;
}

public class UserRequest : RequestBase
{
    
}

public class RewardResponseBase : ResponseBase
{
    public DBItemData DBItemData { get; set; }
    public DBStageData DBStageData { get; set; }
}

public class UseStaminaRequest : RequestBase
{
    public int staminaCount { get; set; }
}

public class StageClearRequest : RequestBase
{
    public int stageIndex { get; set; }
}

public class GetWaveClearRewardRequest : RequestBase
{
    public int stageIndex;
    public WaveClearType waveClearType;
}

public class StageClearResponseBase : ResponseBase
{
    public DBItemData ItemData;
    public DBStageData StageData;
}

[FirestoreData]
public class DBUserData
{
    [FirestoreProperty] public string UserId { get; set; }
    [FirestoreProperty] public DateTime LastLoginTime { get; set; } = DateTime.UtcNow;
    [FirestoreProperty] public DateTime LastGetOfflineRewardTime { get; set; } = DateTime.UtcNow;
    [FirestoreProperty] public Dictionary<string, DBItemData> ItemDataDict { get; set; }
    [FirestoreProperty] public Dictionary<string, DBStageData> StageDataDict { get; set; }
    [FirestoreProperty] public List<DBEquipmentData> EquippedItemDataList { get; set; }
    [FirestoreProperty] public List<DBEquipmentData> UnEquippedItemDataList { get; set; }
    [FirestoreProperty] public DBMissionContainerData MissionContainerData { get; set; }
    [FirestoreProperty] public DBAchievementContainerData AchievementContainerData { get; set; }
    
    public DBUserData()
    {
        ItemDataDict = new();
        StageDataDict = new Dictionary<string, DBStageData>();
        MissionContainerData = new DBMissionContainerData();
    }
}

[FirestoreData]
public class DBStageData
{
    [FirestoreProperty] public int StageIndex { get; set; }
    [FirestoreProperty] public bool IsOpened { get; set; }
    [FirestoreProperty] public List<DBWaveData> WaveDataList { get; set; }

    public void Initialize(int stageIndex, params DBWaveData[] dbWaveDataArray)
    {
        StageIndex = stageIndex;
        WaveDataList = new();
        IsOpened = stageIndex == 1;
        foreach (DBWaveData dbWaveData in dbWaveDataArray)
        {
            WaveDataList.Add(dbWaveData);
        }
    }
}

[FirestoreData]
public class DBWaveData
{
    [FirestoreProperty] public int WaveIndex { get; set; }
    [FirestoreProperty] public bool IsGet { get; set; }
    [FirestoreProperty] public bool IsClear { get; set; }

    public void Initialize(int waveIndex)
    {
        WaveIndex = waveIndex;
        IsGet = false;
        IsClear = false;
    }
}

[FirestoreData]
public class DBItemData
{
    [FirestoreProperty] public int ItemId { get; set; }
    [FirestoreProperty] public int ItemValue { get; set; }
}

[FirestoreData]
public class DBEquipmentData
{
    [FirestoreProperty] public string DataId { get; set; }
    [FirestoreProperty] public string UID { get; set; }
    [FirestoreProperty] public int Level { get; set; }
    [FirestoreProperty] public int EquipmentType { get; set; }
}
    
[FirestoreData]
public class DBCheckoutData
{
    [FirestoreProperty] public List<DBCheckoutDayData> DBCheckoutDayDataList { get; set; }
    [FirestoreProperty] public int TotalAttendanceDays { get; set; }
}

[FirestoreData]
public class DBCheckoutDayData
{
    [FirestoreProperty] public int Day { get; set; }
    [FirestoreProperty] public bool IsGet { get; set; }
}

[FirestoreData]
public class DBMissionContainerData
{   
    [FirestoreProperty] public List<DBMissionData> DBDailyMissionDataList { get; set; }
    [FirestoreProperty] public int TotalDailyMissionClearCount { get; set; }
}

[FirestoreData]
public class DBMissionData
{
    [FirestoreProperty] public int MissionId { get; set; }
    [FirestoreProperty] public int MissionType { get; set; }
    [FirestoreProperty] public int MissionTarget { get; set; }
    [FirestoreProperty] public int AccumulatedValue { get; set; }
    [FirestoreProperty] public bool IsGet { get; set; }
}

[FirestoreData]
public class DBAchievementContainerData
{
    [FirestoreProperty] public List<DBAchievementData> DBAchievementDataList { get; set; }
    [FirestoreProperty] public List<DBAchievementData> DBRewardAchievementDataList { get; set; }
}

[FirestoreData]
public class DBAchievementData
{
    [FirestoreProperty] public int AchievementId { get; set; }
    [FirestoreProperty] public int MissionTarget { get; set; }
    [FirestoreProperty] public int AccumulatedValue { get; set; }
}