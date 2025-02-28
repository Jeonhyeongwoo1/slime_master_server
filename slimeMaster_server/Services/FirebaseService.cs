using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using slimeMaster_server.Models;

public class FirebaseService
{
    private readonly FirestoreDb _firestoreDb;

    public FirebaseService(IConfiguration config)
    {
        string credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/firebase-config.json");
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
        _firestoreDb = FirestoreDb.Create(config["Firebase:ProjectId"]);
        Console.WriteLine("✅ Firestore 연결 성공");
    }

    public FirestoreDb GetFirestoreDb() => _firestoreDb;
    
    public async Task<string> CreateOrGetUID(string UID)
    { 
        try
        {
            if (string.IsNullOrEmpty(UID))
            {
                UserRecord? userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs());
                return userRecord?.Uid;
            }
            else
            {
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(UID);
                if (userRecord == null)
                {
                    return null;
                }

                return UID;
            }
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
