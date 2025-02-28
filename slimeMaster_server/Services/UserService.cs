using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Key;
using SlimeMaster.Managers;
using SlimeMaster.Server;

namespace slimeMaster_server.Services;

public class UserService : IUserService
{
    private readonly DataManager _dataManager;
    private readonly FirebaseService _firebaseService;

    public UserService(DataManager dataManager, FirebaseService firebaseService)
    {
        _dataManager = dataManager;
        _firebaseService = firebaseService;
    }

    public async Task<UserResponseBase> LoadUserDataRequest([FromBody] UserRequest request)
    {
        string userID = request.userId;
        FirestoreDb db = _firebaseService.GetFirestoreDb();
        Dictionary<string, object> userDict = new Dictionary<string, object>();
        Dictionary<string, object> checkoutDict = new Dictionary<string, object>();
        DocumentReference userDocRef = db.Collection(DBKey.UserDB).Document(userID);
        DocumentReference checkoutDocRef = db.Collection(DBKey.CheckoutDB).Document(userID);
        DateTime lastLoginTime = DateTime.UtcNow;

        UserResponseBase userResponseBase = null;
        try
        {
            userResponseBase = await db.RunTransactionAsync(async transaction =>
            {
                Task<DocumentSnapshot> userTask = transaction.GetSnapshotAsync(userDocRef);
                Task<DocumentSnapshot> checkoutTask = transaction.GetSnapshotAsync(checkoutDocRef);
                await Task.WhenAll(userTask, checkoutTask);

                DocumentSnapshot userSnapshot = userTask.Result;
                DocumentSnapshot checkoutSnapshot = checkoutTask.Result;

                if (!userSnapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
                {
                    userData = ServerUserHelper.MakeNewUser(_dataManager, userID);
                }

                lastLoginTime = userData.LastLoginTime;
                userData.LastLoginTime = DateTime.UtcNow;

                //미션 데이터가 없거나 같은 날이 아닌경우에는 초기화작업
                if (userData.MissionContainerData.DBDailyMissionDataList == null ||
                    (DateTime.Now.Day != lastLoginTime.Day))
                {
                    userData.MissionContainerData = ServerMissionHelper.InitializeMissionData(_dataManager);
                }

                userData.AchievementContainerData = ServerAchievementHelper.UpdateDBAchievementContainerData(
                    MissionTarget.Login,
                    userData.AchievementContainerData, 1, _dataManager);

                userDict.Add(nameof(DBUserData), userData);
                transaction.Set(userDocRef, userDict, SetOptions.MergeAll);
                DBCheckoutData dbCheckoutData = null;
                if (!checkoutSnapshot.Exists)
                {
                    dbCheckoutData = ServerCheckoutHelper.MakeNewCheckOutData(_dataManager);
                    dbCheckoutData.TotalAttendanceDays++;
                    checkoutDict.Add(nameof(DBCheckoutData), dbCheckoutData);
                    transaction.Set(checkoutDocRef, checkoutDict, SetOptions.MergeAll);
                }
                else
                {
                    if (!checkoutSnapshot.TryGetValue(nameof(DBCheckoutData), out dbCheckoutData))
                    {
                        dbCheckoutData = ServerCheckoutHelper.MakeNewCheckOutData(_dataManager);
                        checkoutDict.Add(nameof(DBCheckoutData), dbCheckoutData);
                        transaction.Set(checkoutDocRef, checkoutDict, SetOptions.MergeAll);
                    }

                    if ((DateTime.UtcNow - lastLoginTime).TotalHours > 24)
                    {
                        dbCheckoutData.TotalAttendanceDays++;
                        checkoutDict.Add(nameof(DBCheckoutData), dbCheckoutData);
                        transaction.Set(checkoutDocRef, checkoutDict, SetOptions.MergeAll);
                    }
                }

                return new UserResponseBase()
                {
                    responseCode = ServerErrorCode.Success,
                    DBUserData = userData,
                    DBCheckoutData = dbCheckoutData,
                    DBMissionContainerData = userData.MissionContainerData,
                    DBAchievementContainerData = userData.AchievementContainerData,
                    LastLoginTime = lastLoginTime,
                    LastOfflineGetRewardTime = userData.LastGetOfflineRewardTime
                };
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
            return new UserResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        return userResponseBase;
    }
    
     public async Task<UserResponseBase> UseStaminaRequest(UseStaminaRequest request)
    {
        string userID = request.userId;
        FirestoreDb db = _firebaseService.GetFirestoreDb();
        Dictionary<string, object> userDict = new Dictionary<string, object>();
        DocumentReference docRef = db.Collection(DBKey.UserDB).Document(userID);
        DocumentSnapshot snapshot = null;
        int staminaCount = request.staminaCount;

        try
        {
            snapshot = await docRef.GetSnapshotAsync();
        }
        catch (Exception e)
        {
            return new UserResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
        {
            return new UserResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData
            };
        }

        if (!userData.ItemDataDict.TryGetValue(Const.ID_STAMINA.ToString(), out DBItemData itemData))
        {
            return new UserResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData
            };
        }

        if (itemData.ItemValue < staminaCount)
        {
            return new UserResponseBase()
            {
                responseCode = ServerErrorCode.NotEnoughStamina
            };
        }

        userData.MissionContainerData = ServerMissionHelper.UpdateMissionAccumulatedValue(MissionTarget.StageEnter,
            userData.MissionContainerData, 1, _dataManager);

        itemData.ItemValue -= staminaCount;
        userDict.Add(nameof(DBUserData), userData);

        try
        {
            await docRef.SetAsync(userDict, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Console.WriteLine($"error {e.Message}");
            return new UserResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        return new UserResponseBase()
        {
            DBUserData = userData,
            responseCode = ServerErrorCode.Success,
        };
    }

    public async Task<StageClearResponseBase> StageClearRequest(StageClearRequest request)
    {
        string userID = request.userId;
        FirestoreDb db = _firebaseService.GetFirestoreDb();
        Dictionary<string, object> userDict = new Dictionary<string, object>();
        DocumentReference docRef = db.Collection(DBKey.UserDB).Document(userID);
        DocumentSnapshot snapshot = null;
        int stageIndex = request.stageIndex;
        try
        {
            snapshot = await docRef.GetSnapshotAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"error {e.Message}");
            return new StageClearResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError,
                errorMessage = e.Message
            };
        }

        if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
        {
            return new StageClearResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData,
            };
        }

        if (userData.StageDataDict.TryGetValue(request.stageIndex.ToString(), out DBStageData dbStageData))
        {
            foreach (DBWaveData dbWaveData in dbStageData.WaveDataList)
            {
                dbWaveData.IsClear = true;
            }
        }

        int nextStage = request.stageIndex + 1;
        if (userData.StageDataDict.TryGetValue(nextStage.ToString(), out DBStageData dbNextStageData))
        {
            dbNextStageData.IsOpened = true;
        }

        if (userData.ItemDataDict.TryGetValue(Const.ID_GOLD.ToString(), out DBItemData itemData))
        {
            var stageData = _dataManager.StageDict[stageIndex];
            var rewardGold = stageData.ClearReward_Gold;
            itemData.ItemValue += rewardGold;
        }

        userData.MissionContainerData = ServerMissionHelper.UpdateMissionAccumulatedValue(MissionTarget.StageClear,
            userData.MissionContainerData, 1, _dataManager);
        userData.AchievementContainerData =
            ServerAchievementHelper.UpdateDBAchievementContainerData(MissionTarget.StageClear,
                userData.AchievementContainerData, 1, _dataManager);
        userDict.Add(nameof(DBUserData), userData);

        try
        {
            await docRef.SetAsync(userDict, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Console.WriteLine($"error {e.Message}");
            return new StageClearResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError,
                errorMessage = e.Message
            };
        }

        return new StageClearResponseBase()
        {
            ItemData = itemData,
            StageData = dbStageData,
            responseCode = ServerErrorCode.Success,
        };
    }

    public async Task<RewardResponseBase> GetWaveClearRewardRequest(GetWaveClearRewardRequest request)
    {
        int itemId = -1;
        int itemValue = -1;
        int stageIndex = request.stageIndex;
        WaveClearType waveClearType = request.waveClearType;
        StageData stageData = _dataManager.StageDict[stageIndex];
        int waveIndex = 0;
        switch (waveClearType)
        {
            case WaveClearType.FirstWaveClear:
                itemId = stageData.FirstWaveClearRewardItemId;
                itemValue = stageData.FirstWaveClearRewardItemValue;
                waveIndex = stageData.FirstWaveCountValue;
                break;
            case WaveClearType.SecondWaveClear:
                itemId = stageData.SecondWaveClearRewardItemId;
                itemValue = stageData.SecondWaveClearRewardItemValue;
                waveIndex = stageData.SecondWaveCountValue;
                break;
            case WaveClearType.ThirdWaveClear:
                itemId = stageData.ThirdWaveClearRewardItemId;
                itemValue = stageData.ThirdWaveClearRewardItemValue;
                waveIndex = stageData.ThirdWaveCountValue;
                break;
        }

        if (itemId == (int)MaterialType.RandomScroll)
        {
            Random random = new Random();
            itemId = random.Next((int)MaterialType.WeaponScroll, (int)MaterialType.BootsScroll);
        }

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
            Console.WriteLine("error :" + e.Message);
            return new RewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError,
                errorMessage = e.Message
            };
        }

        if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
        {
            return new RewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData,
            };
        }

        if (!userData.ItemDataDict.TryGetValue(itemId.ToString(), out DBItemData dbItemData))
        {
            dbItemData = new DBItemData();
            dbItemData.ItemId = itemId;
        }

        dbItemData.ItemValue += itemValue;
        if (!userData.StageDataDict.TryGetValue(stageIndex.ToString(), out DBStageData dbStageData))
        {
            return new RewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetStage
            };
        }

        var waveData = dbStageData.WaveDataList.Find(v => v.WaveIndex == waveIndex);
        waveData.IsGet = true;
        userDict.Add(nameof(DBUserData), userData);

        try
        {
            await docRef.SetAsync(userDict, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Console.WriteLine("error :" + e.Message);
            return new RewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError,
                errorMessage = e.Message
            };
        }

        return new RewardResponseBase()
        {
            responseCode = ServerErrorCode.Success,
            DBItemData = dbItemData,
            DBStageData = dbStageData
        };
    }

    [HttpPost]
    [Route($"{nameof(CopyNewUser)}")]
    public async Task<ResponseBase> CopyNewUser(RequestBase requestBase)
    {
        string userID = requestBase.userId;
        FirestoreDb db = _firebaseService.GetFirestoreDb();
        string newUserId = "ss";

        List<string> dbKeyList = new List<string>() { DBKey.CheckoutDB, DBKey.UserDB, DBKey.ShopDB };
        List<Task<DocumentSnapshot>> taskList = new(dbKeyList.Count);
        ResponseBase responseBase = await db.RunTransactionAsync(async transaction =>
        {
            foreach (string dbKey in dbKeyList)
            {
                DocumentReference docRef = db.Collection(dbKey).Document(userID);
                Task<DocumentSnapshot> userTask = transaction.GetSnapshotAsync(docRef);
                taskList.Add(userTask);
            }

            DocumentSnapshot[] result = await Task.WhenAll(taskList.ToArray());
            foreach (DocumentSnapshot snapshot in result)
            {
                string collectionName = snapshot.Reference.Parent.Id;
                DocumentReference reference = db.Collection(collectionName).Document(newUserId);
                var dict = snapshot.ToDictionary();
                var newDataDict = new Dictionary<string, object>();
                foreach (var (key, value) in dict)
                {
                    newDataDict.Add(key, value);
                }

                transaction.Set(reference, newDataDict);
            }

            return new ResponseBase()
            {
                responseCode = ServerErrorCode.Success
            };
        });

        return responseBase;
    }
}