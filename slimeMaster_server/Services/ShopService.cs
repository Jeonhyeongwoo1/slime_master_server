using Google.Cloud.Firestore;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Key;
using SlimeMaster.Managers;
using SlimeMaster.Server;

namespace slimeMaster_server.Services;

public class ShopService : IShopService
{
    private readonly DataManager _dataManager;
    private readonly FirebaseService _firebaseService;
    private readonly List<DBItemData> _costItemList = new();
    private readonly List<DBItemData> _rewardItemList = new();
    private readonly List<DBEquipmentData> _rewardEquipmentItemList = new();

    public ShopService(DataManager dataManager, FirebaseService firebaseService)
    {
        _dataManager = dataManager;
        _firebaseService = firebaseService;
    }

    public async Task<ShopPurchaseResponseBase> PurchaseItemRequest(ShopPurchaseRequestBase request)
    {
        string userID = request.userId;
        FirestoreDb db = _firebaseService.GetFirestoreDb();
        IDictionary<string, object> userDict = new Dictionary<string, object>();
        Dictionary<string, object> shopDict = new Dictionary<string, object>();
        DocumentReference userDocRef = db.Collection(DBKey.UserDB).Document(userID);
        DocumentReference shopDocRef = db.Collection(DBKey.ShopDB).Document(userID);

        try
        {
            ShopPurchaseResponseBase shopPurchaseResponseBase = await db.RunTransactionAsync(async transaction =>
            {
                Task<DocumentSnapshot> userTask = transaction.GetSnapshotAsync(userDocRef);
                Task<DocumentSnapshot> shopTask = transaction.GetSnapshotAsync(shopDocRef);

                try
                {
                    await Task.WhenAll(userTask, shopTask);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error {e.Message}");
                    return new ShopPurchaseResponseBase()
                    {
                        responseCode = ServerErrorCode.FailedFirebaseError
                    };
                }

                DocumentSnapshot userSnapshot = userTask.Result;
                DocumentSnapshot shopSnapshot = shopTask.Result;

                if (!userSnapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
                {
                    return new ShopPurchaseResponseBase()
                    {
                        responseCode = ServerErrorCode.FailedGetUserData
                    };
                }

                ShopData shopData = _dataManager.ShopDataDict[request.shopId];
                int capacity = shopData.RewardItemValue;
                _costItemList.Clear();
                _rewardItemList.Clear();
                _rewardEquipmentItemList.Clear();
                _costItemList.Capacity = capacity;
                _rewardItemList.Capacity = capacity;
                _rewardEquipmentItemList.Capacity = capacity;
                if (!userData.ItemDataDict.TryGetValue(shopData.CostItemType.ToString(),
                        out DBItemData dbCostItemData))
                {
                    return new ShopPurchaseResponseBase()
                    {
                        responseCode = ServerErrorCode.FailedGetItemData
                    };
                }

                if (dbCostItemData.ItemValue < shopData.CostValue)
                {
                    return new ShopPurchaseResponseBase()
                    {
                        responseCode = ServerErrorCode.NotEnoughShopCostItemValue
                    };
                }

                dbCostItemData.ItemValue -= shopData.CostValue;
                _costItemList.Add(dbCostItemData);

                switch (shopData.ShopType)
                {
                    case ShopType.CommonItem:
                    case ShopType.Gold:
                        string rewardItemName = shopData.RewardItemType.ToString();
                        if (!userData.ItemDataDict.TryGetValue(rewardItemName, out DBItemData dbRewardItemData))
                        {
                            dbRewardItemData = new();
                            dbRewardItemData.ItemId = shopData.RewardItemType;
                            userData.ItemDataDict.Add(rewardItemName, dbRewardItemData);
                        }

                        dbRewardItemData.ItemValue += shopData.RewardItemValue;
                        userDict.Add(nameof(DBUserData), userData);

                        _rewardItemList.Add(dbRewardItemData);

                        break;
                    case ShopType.Advance_EquipmentItem:
                    case ShopType.Normal_EquipmentItem:
                        int count = shopData.RewardItemValue;
                        for (int i = 0; i < count; i++)
                        {
                            EquipmentGrade grade = Utils.Utils.GetRandomEquipmentGrade(Const.ADVENCED_GACHA_GRADE_PROB);
                            List<GachaRateData> gachaList = _dataManager.GachaTableDataDict[shopData.GachaType]
                                .GachaRateTable.FindAll(x => x.EquipGrade == grade);
                            Random random = new Random();
                            int selectedIndex = random.Next(0, gachaList.Count);
                            GachaRateData gachaRateData = gachaList[selectedIndex];
                            string equipmentID = gachaRateData.EquipmentID;
                            EquipmentData equipmentData = _dataManager.EquipmentDataDict[equipmentID];
                            var dbEquipmentData = new DBEquipmentData
                            {
                                DataId = equipmentID,
                                Level = 1,
                                UID = Guid.NewGuid().ToString(),
                                EquipmentType = (int)equipmentData.EquipmentType
                            };

                            userData.UnEquippedItemDataList ??= new List<DBEquipmentData>();
                            userData.UnEquippedItemDataList.Add(dbEquipmentData);
                            _rewardEquipmentItemList.Add(dbEquipmentData);
                        }

                        userData.MissionContainerData = ServerMissionHelper.UpdateMissionAccumulatedValue(
                            MissionTarget.GachaOpen,
                            userData.MissionContainerData, 1, _dataManager);

                        userDict.Add(nameof(DBUserData), userData);
                        break;
                }

                if (!shopSnapshot.TryGetValue(nameof(DBShopData), out DBShopData dbShopData))
                {
                    dbShopData = new DBShopData();
                }

                dbShopData.ShopHistoryDataList ??= new List<DBShopHistoryData>();
                var dbShopHistoryData = new DBShopHistoryData
                {
                    CostItemType = shopData.CostItemType,
                    CostValue = shopData.CostValue,
                    PurchaseItemId = shopData.ID,
                    PurchaseTime = DateTime.UtcNow
                };

                dbShopData.ShopHistoryDataList.Add(dbShopHistoryData);
                shopDict.Add(nameof(DBShopData), dbShopData);

                try
                {
                    transaction.Set(shopDocRef, shopDict, SetOptions.MergeAll);
                    transaction.Set(userDocRef, userDict, SetOptions.MergeAll);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error {e.Message}");
                    return new ShopPurchaseResponseBase()
                    {
                        responseCode = ServerErrorCode.FailedFirebaseError
                    };
                }

                return new ShopPurchaseResponseBase()
                {
                    CostItemList = _costItemList,
                    RewardItemList = _rewardItemList,
                    RewardEquipmentDataList = _rewardEquipmentItemList
                };
            });

            return shopPurchaseResponseBase;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
            return new ShopPurchaseResponseBase()
            {
                errorMessage = e.Message,
                responseCode = ServerErrorCode.FailedFirebaseError
            };
        }
    }
}