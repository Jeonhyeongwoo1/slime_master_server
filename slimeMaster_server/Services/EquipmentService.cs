using Google.Cloud.Firestore;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Key;
using SlimeMaster.Managers;
using SlimeMaster.Server;

namespace slimeMaster_server.Services;

public class EquipmentService : IEquipmentService
{
    private readonly DataManager _dataManager;
    private readonly FirebaseService _firebaseService;

    public EquipmentService(DataManager dataManager, FirebaseService firebaseService)
    {
        _dataManager = dataManager;
        _firebaseService = firebaseService;
    }
    
    public async Task<EquipmentLevelUpResponseBase> EquipmentLevelUpRequest(EquipmentLevelUpRequestBase request)
    {
        string equipmentDataId = request.equipmentDataId;
        string equipmentUID = request.equipmentUID;
        int level = request.level;
        bool isEquipped = request.isEquipped;
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
            return new EquipmentLevelUpResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
        {
            return new EquipmentLevelUpResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData
            };
        }

        EquipmentData equipmentData = _dataManager.EquipmentDataDict[equipmentDataId];
        EquipmentLevelData equipmentLevelData = _dataManager.EquipmentLevelDataDict[level];
        if (!userData.ItemDataDict.TryGetValue(Const.ID_GOLD.ToString(), out DBItemData dbGoldItemData))
        {
            return new EquipmentLevelUpResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData
            };
        }

        if (!userData.ItemDataDict.TryGetValue(equipmentData.LevelupMaterialID.ToString(),
                out DBItemData dbMaterialItemData))
        {
            return new EquipmentLevelUpResponseBase()
            {
                responseCode = ServerErrorCode.NotEnoughMaterialAmount
            };
        }

        DBEquipmentData dbEquipmentData = isEquipped
            ? userData.EquippedItemDataList.Find(v => v.UID == equipmentUID)
            : userData.UnEquippedItemDataList.Find(v => v.UID == equipmentUID);

        if (dbEquipmentData == null)
        {
            return new EquipmentLevelUpResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetEquipment
            };
        }

        if (dbGoldItemData.ItemValue < equipmentLevelData.UpgradeCost)
        {
            return new EquipmentLevelUpResponseBase()
            {
                responseCode = ServerErrorCode.NotEnoughGold
            };
        }

        if (dbMaterialItemData.ItemValue < equipmentLevelData.UpgradeRequiredItems)
        {
            return new EquipmentLevelUpResponseBase()
            {
                responseCode = ServerErrorCode.NotEnoughMaterialAmount
            };
        }

        userData.MissionContainerData = ServerMissionHelper.UpdateMissionAccumulatedValue(
            MissionTarget.EquipmentLevelUp, userData.MissionContainerData, 1, _dataManager);

        dbEquipmentData.Level++;
        dbGoldItemData.ItemValue -= equipmentLevelData.UpgradeCost;
        dbMaterialItemData.ItemValue -= equipmentLevelData.UpgradeRequiredItems;
        userDict.Add(nameof(DBUserData), userData);

        try
        {
            await docRef.SetAsync(userDict, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
            return new EquipmentLevelUpResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        return new EquipmentLevelUpResponseBase()
        {
            DBUserData = userData,
            responseCode = ServerErrorCode.Success,
        };
    }

    public async Task<UnequipResponseBase> UnequipRequest(UnequipRequestBase request)
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
            return new UnequipResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
        {
            return new UnequipResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData
            };
        }

        DBEquipmentData dbEquipmentData = dbUserData.EquippedItemDataList.Find(v => v.UID == request.equipmentUID);
        if (dbEquipmentData == null)
        {
            return new UnequipResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetEquipment
            };
        }

        dbUserData.EquippedItemDataList.Remove(dbEquipmentData);
        dbUserData.UnEquippedItemDataList ??= new List<DBEquipmentData>();
        dbUserData.UnEquippedItemDataList.Add(dbEquipmentData);
        userDict.Add(nameof(DBUserData), dbUserData);

        try
        {
            await docRef.SetAsync(userDict, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
            return new UnequipResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        return new UnequipResponseBase()
        {
            responseCode = ServerErrorCode.Success,
            EquipmentDataList = dbUserData.EquippedItemDataList,
            UnEquipmentDataList = dbUserData.UnEquippedItemDataList
        };
    }

    public async Task<EquipResponseBase> EquipRequest(EquipRequestBase request)
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
            return new EquipResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
        {
            return new EquipResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData
            };
        }

        DBEquipmentData unEquipmentData =
            dbUserData.UnEquippedItemDataList.Find(v => v.UID == request.unequippedItemUID);
        if (unEquipmentData == null)
        {
            return new EquipResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetEquipment
            };
        }

        if (!string.IsNullOrEmpty(request.equippedItemUID))
        {
            DBEquipmentData equippedItemData =
                dbUserData.EquippedItemDataList.Find(v => v.UID == request.equippedItemUID);
            if (equippedItemData != null)
            {
                dbUserData.EquippedItemDataList.Remove(equippedItemData);
                dbUserData.UnEquippedItemDataList.Add(equippedItemData);
            }
        }

        dbUserData.UnEquippedItemDataList.Remove(unEquipmentData);
        dbUserData.EquippedItemDataList.Add(unEquipmentData);
        userDict.Add(nameof(DBUserData), dbUserData);
        try
        {
            await docRef.SetAsync(userDict, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
            return new EquipResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        return new EquipResponseBase()
        {
            responseCode = ServerErrorCode.Success,
            EquipmentDataList = dbUserData.EquippedItemDataList,
            UnEquipmentDataList = dbUserData.UnEquippedItemDataList
        };
    }

    public async Task<MergeEquipmentResponseBase> MergeEquipmentRequest(MergeEquipmentRequestBase request)
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
            return new MergeEquipmentResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
        {
            return new MergeEquipmentResponseBase()
            {
                responseCode = ServerErrorCode.FailedGetUserData
            };
        }

        List<string> uidList = new List<string>();
        foreach (AllMergeEquipmentRequestData requestData in request.equipmentList)
        {
            string selectedEquipItemUid = requestData.selectedEquipItemUid;
            string firstCostItemUID = requestData.firstCostItemUID;
            string secondCostItemUID = requestData.secondCostItemUID;

            DBEquipmentData selectedDBEquipmentData =
                dbUserData.UnEquippedItemDataList.Find(v => v.UID == selectedEquipItemUid);
            if (selectedDBEquipmentData == null)
            {
                Console.WriteLine("selectedDBEquipmentData is null" + selectedEquipItemUid);
                return new MergeEquipmentResponseBase()
                {
                    responseCode = ServerErrorCode.FailedGetEquipment
                };
            }

            DBEquipmentData firstDBEquipmentData = null;
            if (!string.IsNullOrEmpty(firstCostItemUID))
            {
                firstDBEquipmentData =
                    dbUserData.UnEquippedItemDataList.Find(v => v.UID == firstCostItemUID);
                if (firstDBEquipmentData == null)
                {
                    Console.WriteLine("firstDBEquipmentData is null");
                    return new MergeEquipmentResponseBase()
                    {
                        responseCode = ServerErrorCode.FailedGetEquipment
                    };
                }
            }

            DBEquipmentData secondDBEquipmentData = null;
            if (!string.IsNullOrEmpty(secondCostItemUID))
            {
                secondDBEquipmentData =
                    dbUserData.UnEquippedItemDataList.Find(v => v.UID == secondCostItemUID);
                if (secondDBEquipmentData == null)
                {
                    Console.WriteLine("secondDBEquipmentData is null");
                    return new MergeEquipmentResponseBase()
                    {
                        responseCode = ServerErrorCode.FailedGetEquipment
                    };
                }
            }

            string id = selectedDBEquipmentData.DataId;
            EquipmentData equipmentData = _dataManager.EquipmentDataDict[id];
            string mergeItemCode = equipmentData.MergedItemCode;

            if (!_dataManager.EquipmentDataDict.TryGetValue(mergeItemCode, out EquipmentData upgradedEquipmentData))
            {
                Console.WriteLine("upgradedEquipmentData is null");
                return new MergeEquipmentResponseBase()
                {
                    responseCode = ServerErrorCode.FailedGetEquipment
                };
            }

            dbUserData.UnEquippedItemDataList.Remove(selectedDBEquipmentData);
            if (firstDBEquipmentData != null)
            {
                dbUserData.UnEquippedItemDataList.Remove(firstDBEquipmentData);
            }

            if (secondDBEquipmentData != null)
            {
                dbUserData.UnEquippedItemDataList.Remove(selectedDBEquipmentData);
            }

            var newData = new DBEquipmentData
            {
                DataId = upgradedEquipmentData.DataId,
                Level = 1,
                EquipmentType = (int)upgradedEquipmentData.EquipmentType,
                UID = selectedEquipItemUid
            };

            dbUserData.UnEquippedItemDataList.Add(newData);
            uidList.Add(selectedEquipItemUid);
        }

        dbUserData.MissionContainerData = ServerMissionHelper.UpdateMissionAccumulatedValue(
            MissionTarget.EquipmentMerge,
            dbUserData.MissionContainerData, 1, _dataManager);

        userDict.Add(nameof(DBUserData), dbUserData);

        try
        {
            await docRef.SetAsync(userDict, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Console.WriteLine("failed firebase set " + e.Message);
            return new MergeEquipmentResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }

        return new MergeEquipmentResponseBase()
        {
            responseCode = ServerErrorCode.Success,
            UnEquipmentDataList = dbUserData.UnEquippedItemDataList,
            NewItemUIDList = uidList
        };
    }
}