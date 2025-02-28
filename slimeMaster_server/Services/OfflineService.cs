using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Key;
using SlimeMaster.Managers;

namespace slimeMaster_server.Services;

public class OfflineService : IOfflineService
{
    private DataManager _dataManager;
    private FirebaseService _firebaseService;

    public OfflineService(DataManager dataManager, FirebaseService firebaseService)
    {
        _dataManager = dataManager;
        _firebaseService = firebaseService;
    }
    
    public async Task<OfflineRewardResponseBase> GetOfflineRewardRequest(RequestBase request)
    {
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
            Console.WriteLine($"Error {e.Message}");
            return new OfflineRewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
        {
            return new OfflineRewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData
            };
        }

        TimeSpan rewardTime = Utils.Utils.GetOfflineRewardTime(dbUserData.LastGetOfflineRewardTime);
        if (rewardTime.Minutes < Const.MIN_OFFLINE_REWARD_MINUTE)
        {
            return new OfflineRewardResponseBase()
            {
                responseCode = ServerErrorCode.NotEnoughRewardTime
            };
        }

        int lastClearStageIndex = GetLastStageIndex(dbUserData);
        int rewardGold = _dataManager.OfflineRewardDataDict[lastClearStageIndex].Reward_Gold;
        int gold = CalculateRewardGold(rewardGold, rewardTime);
        if (!dbUserData.ItemDataDict.TryGetValue(Const.ID_GOLD.ToString(), out DBItemData dbItemData))
        {
            dbItemData = new DBItemData();
            dbItemData.ItemId = Const.ID_GOLD;
        }

        dbUserData.LastGetOfflineRewardTime = DateTime.UtcNow;
        dbItemData.ItemValue += gold;
        userDict.Add(nameof(DBUserData), dbUserData);

        try
        {
            await docRef.SetAsync(userDict, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error " + e.Message);
            return new OfflineRewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        return new OfflineRewardResponseBase()
        {
            DBRewardItemData = dbItemData,
            LastGetOfflineRewardTime = dbUserData.LastGetOfflineRewardTime
        };
    }

    [HttpPost]
    [Route($"{nameof(GetFastRewardRequest)}")]
    public async Task<FastRewardResponseBase> GetFastRewardRequest(RequestBase request)
    {
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
                Console.WriteLine($"Error {e.Message}");
                return new FastRewardResponseBase()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
            {
                return new FastRewardResponseBase()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            var rewardItemDataList = new List<DBItemData>();
            
            int lastClearStageIndex = GetLastStageIndex(dbUserData);
            OfflineRewardData offlineRewardData = _dataManager.OfflineRewardDataDict[lastClearStageIndex];
            int rewardGold = offlineRewardData.Reward_Gold * Const.FAST_REWARD_GOLD_MULITPILIER;
            if (!dbUserData.ItemDataDict.TryGetValue(Const.ID_GOLD.ToString(), out DBItemData dbGoldItemData))
            {
                dbGoldItemData = new DBItemData();
                dbGoldItemData.ItemId = Const.ID_GOLD;
            }

            dbGoldItemData.ItemValue += rewardGold;
            rewardItemDataList.Add(dbGoldItemData);
            
            Random random = new Random();
            int scrollIndex = random.Next((int)MaterialType.WeaponScroll, (int)MaterialType.BootsScroll);
            if (!dbUserData.ItemDataDict.TryGetValue(scrollIndex.ToString(), out DBItemData dbScrollItemData))
            {
                dbScrollItemData = new DBItemData();
                dbScrollItemData.ItemId = scrollIndex;
            }

            dbScrollItemData.ItemValue += scrollIndex;
            rewardItemDataList.Add(dbScrollItemData);
            
            EquipmentGrade equipmentGrade = Utils.Utils.GetRandomEquipmentGrade(Const.COMMON_GACHA_GRADE_PROB);
            var equipmentDataList = _dataManager.EquipmentDataDict.Values.Where(x => x.EquipmentGrade == equipmentGrade)
                .ToList();
            EquipmentData equipmentData = equipmentDataList[random.Next(0, equipmentDataList.Count)];
            DBEquipmentData rewardEquipmentData = new DBEquipmentData
            {
                DataId = equipmentData.DataId,
                EquipmentType = (int)equipmentData.EquipmentType,
                Level = 1,
                UID = Guid.NewGuid().ToString()
            };

            dbUserData.UnEquippedItemDataList ??= new();
            dbUserData.UnEquippedItemDataList.Add(rewardEquipmentData);
            userDict.Add(nameof(DBUserData), dbUserData);
            
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
                return new FastRewardResponseBase()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            return new FastRewardResponseBase()
            {
                responseCode = ServerErrorCode.Success,
                DBEquipmentData = rewardEquipmentData,
                DBItemDataList = rewardItemDataList,
            };
    }

    private int CalculateRewardGold(int gold, TimeSpan lastOfflineGetRewardTime)
    {
        double minute = lastOfflineGetRewardTime.Minutes;
        return (int)(gold / 60f * minute);
    }

    private int GetLastStageIndex(DBUserData dbUserData)
    {
        foreach (var (key, dbStageData) in dbUserData.StageDataDict)
        {
            bool? isClear = dbStageData.WaveDataList.LastOrDefault()?.IsClear;
            if (isClear.HasValue)
            {
                return dbStageData.StageIndex;
            }
        }

        return 1;
    }
}