using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using slimeMaster_server.Interface;
using slimeMaster_server.Models;
using slimeMaster_server.Services;
using SlimeMaster.Managers;

public class Program
{
    public static void Main(string[] args)
    {
        // Firebase 초기화
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile("wwwroot/firebase-config.json") // 🔥 Firebase 서비스 계정 키 경로
        });

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<DataManager>();
        builder.Services.Configure<FirebaseService>(builder.Configuration.GetSection("Firebase"));
        builder.Services.AddSingleton<FirebaseService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddScoped<IAchievementService, AchievementService>();
        builder.Services.AddScoped<ICheckoutService, CheckoutService>();
        builder.Services.AddScoped<IEquipmentService, EquipmentService>();
        builder.Services.AddScoped<IMissionService, MissionService>();
        builder.Services.AddScoped<IOfflineService, OfflineService>();
        builder.Services.AddScoped<IShopService, ShopService>();
        builder.Services.AddScoped<IUserService, UserService>();


        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                string jsonData = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/config.json"));
                var configData = JsonConvert.DeserializeObject<ConfigData>(jsonData);
                string key = configData.jwtKey;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
            });

        builder.Services.AddAuthorization();
        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        using (var scope = app.Services.CreateScope())
        {
            var dataCenter = scope.ServiceProvider.GetRequiredService<DataManager>();
            dataCenter.Initialize();
        }

        app.Run();
    }
}