namespace slimeMaster_server.Models;

public class DBAuthRequest
{
    public string uid { get; set; }
}

public class DBAuthResponseBase : ResponseBase
{
    public string uid { get; set; }
    public string token { get; set; }
}