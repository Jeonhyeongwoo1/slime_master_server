using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Models;
using slimeMaster_server.Services;
using SlimeMaster.Enum;

namespace slimeMaster_server.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private AuthService _authService;
    private FirebaseService _firebaseService;

    public AuthController(AuthService authService, FirebaseService firebaseService)
    {
        _authService = authService;
        _firebaseService = firebaseService;
    }
    
    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    public async Task<DBAuthResponseBase> Login([FromBody] DBAuthRequest request)
    { 
        try
        {
            Console.WriteLine("Login :" + request.uid);
            string uid = await _firebaseService.CreateOrGetUID(request.uid);
            if (string.IsNullOrEmpty(uid))
            {
                return new DBAuthResponseBase() { responseCode = ServerErrorCode.FailedFirebaseError };
            }
            
            string token = _authService.GenerateJwtToken(uid);
            return string.IsNullOrEmpty(token)
                ? new DBAuthResponseBase() { responseCode = ServerErrorCode.FailedGenerateToken }
                : new DBAuthResponseBase() { responseCode = ServerErrorCode.Success, uid = uid, token = token };
        }
        catch (Exception e)
        {
            return new DBAuthResponseBase() { responseCode = ServerErrorCode.FailedFirebaseError };
        }
    }
}