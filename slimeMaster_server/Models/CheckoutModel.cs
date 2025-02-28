namespace slimeMaster_server.Models;

public class GetCheckoutRewardResponseBase : ResponseBase
{
    public DBCheckoutData DBCheckoutData { get; set; }
    public DBItemData DBItemData { get; set; }
    public DBEquipmentData DBEquipmentData { get; set; }
}

public class GetCheckoutRewardRequestBase : RequestBase
{
    public int day { get; set; }
}