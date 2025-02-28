using Google.Cloud.Firestore;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Key;
using SlimeMaster.Managers;

namespace slimeMaster_server.Services;

public class MissionService : IMissionService
{
    private DataManager _dataManager;
    private FirebaseService _firebaseService;

    public MissionService(DataManager dataManager, FirebaseService firebaseService)
    {
        _dataManager = dataManager;
        _firebaseService = firebaseService;
    }

    public async Task<GetMissionRewardResponseBase> GetMissionRewardRequest(GetMissionRewardRequest request)
    {
        int missionId = request.missionId;
        MissionType missionType = request.missionType;
        string userID = request.userId;
        FirestoreDb db = _firebaseService.GetFirestoreDb();
        Dictionary<string, object> userDict = new Dictionary<string, object>();
        DocumentReference docRef = db.Collection(DBKey.UserDB).Document(userID);
        DocumentSnapshot snapshot = null;

        try
        {
            snapshot = await docRef.GetSnapshotAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error :" + e.Message);
            return new GetMissionRewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
        {
            return new GetMissionRewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData
            };
        }

        DBMissionData dbMissionData = null;
        if (missionType == MissionType.Daily)
        {
            dbMissionData =
                dbUserData.MissionContainerData.DBDailyMissionDataList.Find(v => v.MissionId == missionId);
        }

        if (dbMissionData == null)
        {
            return new GetMissionRewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetMissionData
            };
        }

        if (dbMissionData.IsGet)
        {
            return new GetMissionRewardResponseBase()
            {
                responseCode = ServerErrorCode.AlreadyClaimed
            };
        }

        MissionData missionData = _dataManager.MissionDataDict[missionId];
        if (dbMissionData.AccumulatedValue < missionData.MissionTargetValue)
        {
            return new GetMissionRewardResponseBase()
            {
                responseCode = ServerErrorCode.NotEnoughAccumulatedValue
            };
        }

        dbMissionData.IsGet = true;
        int rewardItemId = missionData.ClearRewardItmeId;
        if (!dbUserData.ItemDataDict.TryGetValue(rewardItemId.ToString(), out DBItemData dbItemData))
        {
            dbItemData = new DBItemData();
            dbItemData.ItemId = rewardItemId;
        }

        dbItemData.ItemValue += missionData.RewardValue;
        userDict.Add(nameof(DBUserData), dbUserData);

        try
        {
            await docRef.SetAsync(userDict, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error :" + e.Message);
            return new GetMissionRewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        return new GetMissionRewardResponseBase()
        {
            responseCode = ServerErrorCode.Success,
            DBMissionContainerData = dbUserData.MissionContainerData,
            DBRewardItemData = dbItemData
        };
    }
}