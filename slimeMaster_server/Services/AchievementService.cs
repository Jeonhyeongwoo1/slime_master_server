using Google.Cloud.Firestore;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Key;
using SlimeMaster.Managers;

namespace slimeMaster_server.Services;

public class AchievementService : IAchievementService
{
    private readonly DataManager _dataManager;
    private readonly FirebaseService _firebaseService;

    public AchievementService(DataManager dataManager, FirebaseService firebaseService)
    {
        _dataManager = dataManager;
        _firebaseService = firebaseService;
    }
    
    public async Task<AchievementResponseBase> GetAchievementRewardRequest(AchievementRequestBase request)
    {
        string userID = request.userId;
        FirestoreDb db = _firebaseService.GetFirestoreDb();
        Dictionary<string, object> userDict = new Dictionary<string, object>();
        DocumentReference docRef = db.Collection(DBKey.UserDB).Document(userID);
        DocumentSnapshot snapshot = null;
        int achievementId = request.achievementId;
        try
        {
            snapshot = await docRef.GetSnapshotAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
            return new AchievementResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
        {
            return new AchievementResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData
            };
        }

        if (dbUserData.AchievementContainerData.DBRewardAchievementDataList != null)
        {
            DBAchievementData dbRewardedAchievementData =
                dbUserData.AchievementContainerData.DBRewardAchievementDataList.Find(v =>
                    v.AchievementId == achievementId);
            if (dbRewardedAchievementData != null)
            {
                return new AchievementResponseBase()
                {
                    responseCode = ServerErrorCode.AlreadyClaimed
                };
            }
        }

        DBAchievementData dbAchievementData =
            dbUserData.AchievementContainerData.DBAchievementDataList.Find(v => v.AchievementId == achievementId);
        AchievementData achievementData = _dataManager.AchievementDataDict[achievementId];
        if (achievementData.MissionTargetValue > dbAchievementData.AccumulatedValue)
        {
            return new AchievementResponseBase()
            {
                responseCode = ServerErrorCode.NotEnoughAccumulatedValue
            };
        }

        dbUserData.AchievementContainerData.DBRewardAchievementDataList ??= new List<DBAchievementData>();
        dbUserData.AchievementContainerData.DBRewardAchievementDataList.Add(new DBAchievementData()
        {
            AchievementId = dbAchievementData.AchievementId,
            AccumulatedValue = dbAchievementData.AccumulatedValue,
            MissionTarget = dbAchievementData.MissionTarget
        });

        List<AchievementData> achievementDataList = _dataManager.AchievementDataDict.Values
            .Where(x => x.MissionTarget == achievementData.MissionTarget).ToList();
        AchievementData nextAchievementData =
            achievementDataList.Find(v => v.AchievementID == dbAchievementData.AchievementId + 1);
        if (nextAchievementData != null)
        {
            dbAchievementData.AchievementId = nextAchievementData.AchievementID;
        }

        if (!dbUserData.ItemDataDict.TryGetValue(achievementData.ClearRewardItmeId.ToString(),
                out DBItemData dbItemData))
        {
            dbItemData = new DBItemData();
            dbItemData.ItemId = achievementData.ClearRewardItmeId;
        }

        dbItemData.ItemValue += achievementData.RewardValue;
        userDict.Add(nameof(DBUserData), dbUserData);

        try
        {
            await docRef.SetAsync(userDict, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
            return new AchievementResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        return new AchievementResponseBase()
        {
            RewardItemData = dbItemData,
            DBAchievementContainerData = dbUserData.AchievementContainerData
        };
    }
}