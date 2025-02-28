using Google.Cloud.Firestore;

namespace slimeMaster_server.Models;

public class ShopPurchaseResponseBase : ResponseBase
{
    public List<DBItemData> CostItemList { get; set; }
    public List<DBItemData> RewardItemList { get; set; }
    public List<DBEquipmentData> RewardEquipmentDataList { get; set; }
}

public class ShopPurchaseRequestBase : RequestBase
{
    public int shopId { get; set; }
}


[FirestoreData]
public class DBShopData
{
    [FirestoreProperty] public List<DBShopHistoryData> ShopHistoryDataList { get; set; }
}
    
[FirestoreData]
public class DBShopHistoryData
{
    [FirestoreProperty] public DateTime PurchaseTime { get; set; }
    [FirestoreProperty] public int PurchaseItemId { get; set; }
    [FirestoreProperty] public int CostItemType { get; set; }
    [FirestoreProperty] public float CostValue { get; set; }
}
