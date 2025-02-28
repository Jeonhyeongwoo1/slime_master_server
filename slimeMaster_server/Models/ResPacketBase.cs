using SlimeMaster.Enum;

namespace slimeMaster_server.Models;

public class ResponseBase
{
    public ServerErrorCode responseCode { get; set; }
    public string errorMessage { get; set; }
}

public class RequestBase
{
    public string userId { get; set; }
}