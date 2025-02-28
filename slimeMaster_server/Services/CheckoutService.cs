using Google.Cloud.Firestore;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Key;
using SlimeMaster.Managers;
using SlimeMaster.Server;

namespace slimeMaster_server.Services;

public class CheckoutService : ICheckoutService
{
    private readonly DataManager _dataManager;
    private readonly FirebaseService _firebaseService;

    public CheckoutService(DataManager dataManager, FirebaseService firebaseService)
    {
        _dataManager = dataManager;
        _firebaseService = firebaseService;
    }

    public async Task<GetCheckoutRewardResponseBase> GetCheckoutRewardRequest(GetCheckoutRewardRequestBase request)
    {
        string userID = request.userId;
        FirestoreDb db = _firebaseService.GetFirestoreDb();
        Dictionary<string, object> userDict = new Dictionary<string, object>();
        Dictionary<string, object> checkoutDict = new Dictionary<string, object>();
        DocumentReference userDocRef = db.Collection(DBKey.UserDB).Document(userID);
        DocumentReference checkoutDocRef = db.Collection(DBKey.CheckoutDB).Document(userID);
        int day = request.day;

        GetCheckoutRewardResponseBase getCheckoutRewardResponseBase = null;
        try
        {
            getCheckoutRewardResponseBase = await db.RunTransactionAsync(async transaction =>
            {
                Task<DocumentSnapshot> userTask = transaction.GetSnapshotAsync(userDocRef);
                Task<DocumentSnapshot> checkoutTask = transaction.GetSnapshotAsync(checkoutDocRef);

                await Task.WhenAll(userTask, checkoutTask);

                DocumentSnapshot userSnapshot = userTask.Result;
                DocumentSnapshot checkoutSnapshot = checkoutTask.Result;

                if (!checkoutSnapshot.TryGetValue(nameof(DBCheckoutData), out DBCheckoutData dbCheckoutData) ||
                    !userSnapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
                {
                    return new GetCheckoutRewardResponseBase()
                    {
                        responseCode = ServerErrorCode.FailedGetUserData
                    };
                }

                if (dbCheckoutData.TotalAttendanceDays < day)
                {
                    return new GetCheckoutRewardResponseBase()
                    {
                        responseCode = ServerErrorCode.NotEnoughTime
                    };
                }

                DBCheckoutDayData dbCheckoutDayData = dbCheckoutData.DBCheckoutDayDataList.Find(v => v.Day == day);
                if (dbCheckoutDayData.IsGet)
                {
                    return new GetCheckoutRewardResponseBase()
                    {
                        responseCode = ServerErrorCode.AlreadyClaimed
                    };
                }

                dbCheckoutDayData.IsGet = true;

                //AllClear
                if (ServerCheckoutHelper.IsAllClear(dbCheckoutData))
                {
                    dbCheckoutData.TotalAttendanceDays = 0;
                    dbCheckoutData.DBCheckoutDayDataList.ForEach(v => v.IsGet = false);
                }

                DBEquipmentData dbEquipmentData = null;
                DBItemData dbItemData = null;
                CheckOutData checkoutData = _dataManager.CheckOutDataDict[day];
                if (checkoutData.RewardItemId >= (int)MaterialType.AllRandomEquipmentBox &&
                    checkoutData.RewardItemId <= (int)MaterialType.LegendaryEquipmentBox)
                {
                    EquipmentData selectedEquipmentData = null;
                    MaterialType type = (MaterialType)checkoutData.RewardItemId;
                    EquipmentGrade grade = EquipmentGrade.Common;
                    Random ran = new Random();
                    switch (type)
                    {
                        case MaterialType.AllRandomEquipmentBox:
                            int random = ran.Next((int)EquipmentGrade.Common, (int)EquipmentGrade.Epic);
                            grade = (EquipmentGrade)random;
                            break;
                        case MaterialType.CommonEquipmentBox:
                            grade = EquipmentGrade.Common;
                            break;
                        case MaterialType.UncommonEquipmentBox:
                            grade = EquipmentGrade.Uncommon;
                            break;
                        case MaterialType.RareEquipmentBox:
                            grade = EquipmentGrade.Rare;
                            break;
                        case MaterialType.EpicEquipmentBox:
                            grade = EquipmentGrade.Epic;
                            break;
                        case MaterialType.LegendaryEquipmentBox:
                            grade = EquipmentGrade.Legendary;
                            break;
                    }

                    List<EquipmentData> equipmentDataList = _dataManager.EquipmentDataDict.Values.Where(x =>
                        x.EquipmentGrade == grade).ToList();

                    int select = ran.Next(0, equipmentDataList.Count);
                    selectedEquipmentData = equipmentDataList[select];

                    dbEquipmentData = new DBEquipmentData()
                    {
                        DataId = selectedEquipmentData.DataId,
                        EquipmentType = (int)selectedEquipmentData.EquipmentType,
                        Level = 1,
                        UID = Guid.NewGuid().ToString()
                    };

                    dbUserData.UnEquippedItemDataList ??= new List<DBEquipmentData>();
                    dbUserData.UnEquippedItemDataList.Add(dbEquipmentData);
                }
                else
                {
                    if (!dbUserData.ItemDataDict.TryGetValue(checkoutData.RewardItemId.ToString(),
                            out dbItemData))
                    {
                        dbItemData = new DBItemData();
                        dbItemData.ItemId = checkoutData.RewardItemId;
                    }

                    dbItemData.ItemValue += checkoutData.MissionTarRewardItemValuegetValue;
                }


                checkoutDict.Add(nameof(DBCheckoutData), dbCheckoutData);
                userDict.Add(nameof(DBUserData), dbUserData);
                transaction.Set(userDocRef, userDict, SetOptions.MergeAll);
                transaction.Set(checkoutDocRef, checkoutDict, SetOptions.MergeAll);

                return new GetCheckoutRewardResponseBase()
                {
                    DBCheckoutData = dbCheckoutData,
                    DBItemData = dbItemData,
                    DBEquipmentData = dbEquipmentData
                };
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
            return new GetCheckoutRewardResponseBase()
            {
                responseCode = ServerErrorCode.FailedFirebaseError,
                errorMessage = e.Message
            };
        }

        return getCheckoutRewardResponseBase;
    }
}