namespace slimeMaster_server.Models;

public class EquipmentLevelUpResponseBase : ResponseBase
{
    public DBUserData DBUserData { get; set; }
}
    
public class EquipmentLevelUpRequestBase : RequestBase
{
    public string equipmentDataId { get; set; }
    public string equipmentUID { get; set; }
    public int level { get; set; }
    public bool isEquipped { get; set; }
}
    
public class UnequipResponseBase : ResponseBase
{
    public List<DBEquipmentData> EquipmentDataList { get; set; }
    public List<DBEquipmentData> UnEquipmentDataList { get; set; }
}

public class UnequipRequestBase : RequestBase
{
    public string equipmentUID { get; set; }
}

public class EquipRequestBase : RequestBase
{
    public string unequippedItemUID { get; set; }
    public string equippedItemUID { get; set; }
}

public class EquipResponseBase : ResponseBase
{
    public List<DBEquipmentData> EquipmentDataList { get; set; }
    public List<DBEquipmentData> UnEquipmentDataList { get; set; }
}

public class MergeEquipmentResponseBase : ResponseBase
{
    public List<DBEquipmentData> UnEquipmentDataList { get; set; }
    public List<string> NewItemUIDList { get; set; }
}

public class MergeEquipmentRequestBase : RequestBase
{
    public List<AllMergeEquipmentRequestData> equipmentList { get; set; }
}

[Serializable]
public class AllMergeEquipmentRequestData
{
    public string selectedEquipItemUid;
    public string firstCostItemUID;
    public string secondCostItemUID;

    public string id1;
    public string id2;
    public string id3;
}