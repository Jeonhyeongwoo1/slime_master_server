using SlimeMaster.Enum;

namespace slimeMaster_server.Utils;

public static class Utils
{
    public static EquipmentGrade GetRandomEquipmentGrade(float[] prob)
    {
        Random random = new Random();
        float value = random.NextSingle();
        if (value < prob[(int)EquipmentGrade.Epic])
        {
            return EquipmentGrade.Epic;
        }

        if (value < prob[(int)EquipmentGrade.Rare])
        {
            return EquipmentGrade.Rare;
        }

        if (value < prob[(int)EquipmentGrade.Uncommon])
        {
            return EquipmentGrade.Uncommon;
        }

        return EquipmentGrade.Common;
    }
    
    public static TimeSpan GetOfflineRewardTime(DateTime lastOfflineGetRewardTime)
    {
        DateTime dateTime = lastOfflineGetRewardTime;
        TimeSpan timeSpan = DateTime.UtcNow - dateTime;
        if (timeSpan > TimeSpan.FromHours(24))
        {
            return TimeSpan.FromHours(23.9);
        }

        return timeSpan;
    }
}