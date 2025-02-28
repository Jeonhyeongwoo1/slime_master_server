using slimeMaster_server.Models;
using SlimeMaster.Enum;
using SlimeMaster.Managers;

namespace SlimeMaster.Server
{
    public class ServerAchievementHelper
    {
        public static DBAchievementContainerData InitializeAchievementData(DataManager dataManager)
        {
            var dbMissionContainerData = new DBAchievementContainerData()
            {
                DBAchievementDataList = new List<DBAchievementData>()
            };

            foreach (var (key, achievementData) in dataManager.AchievementDataDict)
            {
                List<DBAchievementData> dbAchievementDataList = dbMissionContainerData.DBAchievementDataList;
                var dbAchievementData =
                    dbAchievementDataList.Find(v => v.MissionTarget == (int)achievementData.MissionTarget);
                if (dbAchievementData != null)
                {
                    continue;
                }
                
                DBAchievementData newData = new DBAchievementData
                {
                    AchievementId = achievementData.AchievementID,
                    MissionTarget = (int)achievementData.MissionTarget,
                    AccumulatedValue = 0,
                };
                
                dbMissionContainerData.DBAchievementDataList.Add(newData);
            }
            
            return dbMissionContainerData;
        }


        public static DBAchievementContainerData UpdateDBAchievementContainerData(MissionTarget missionTarget,
            DBAchievementContainerData dbAchievementContainerData, int addValue, DataManager dataManager)
        {
            if (dbAchievementContainerData == null)
            {
                dbAchievementContainerData = InitializeAchievementData(dataManager);
            }

            DBAchievementData dbAchievementData =
                dbAchievementContainerData.DBAchievementDataList.Find(v => v.MissionTarget == (int)missionTarget);
            if (dbAchievementData == null)
            {
                return dbAchievementContainerData;
            }

            dbAchievementData.AccumulatedValue += addValue;
            return dbAchievementContainerData;
        }
    }
}