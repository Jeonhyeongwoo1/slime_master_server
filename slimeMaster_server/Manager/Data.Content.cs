using System;
using System.Collections.Generic;
using SlimeMaster.Enum;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

namespace SlimeMaster.Data
{
    
    #region LevelData
    [Serializable]
    public class LevelData
    {
        public int Level;
        public int TotalExp;
        public int RequiredExp;
    }

    [Serializable]
    public class LevelDataLoader : ILoader<int, LevelData>
    {
        public List<LevelData> levels = new List<LevelData>();
        public Dictionary<int, LevelData> MakeDict()
        {
            Dictionary<int, LevelData> dict = new Dictionary<int, LevelData>();
            foreach (LevelData levelData in levels)
                dict.Add(levelData.Level, levelData);
            return dict;
        }
    }
    #endregion

    
    #region CreatureData

    [Serializable]
    public class CreatureData
    {
        public int DataId;
        public string DescriptionTextID;
        public string PrefabLabel;
        public float MaxHp;
        public float MaxHpBonus;
        public float Atk;
        public float AtkBonus;
        public float Def;
        public float MoveSpeed;
        public float TotalExp;
        public float HpRate;
        public float AtkRate;
        public float DefRate;
        public float MoveSpeedRate;
        public string IconLabel;
        public List<int> SkillTypeList;//InGameSkills를 제외한 추가스킬들
    }
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> creatures = new();
        
        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach (CreatureData creatureData in creatures)
            {
                dict.Add(creatureData.DataId, creatureData);
            }
            return dict;
        }
    }
    
    #endregion
    
    #region SkillData
    [Serializable]
    public class SkillData
    {
        public int DataId;
        public string Name;
        public string Description;
        public string PrefabLabel; //프리팹 경로
        public string IconLabel;//아이콘 경로
        public string SoundLabel;// 발동사운드 경로
        public string Category;//스킬 카테고리
        public float CoolTime; // 쿨타임
        public float DamageMultiplier; //스킬데미지 (곱하기)
        public float ProjectileSpacing;// 발사체 사이 간격
        public float Duration; //스킬 지속시간
        public float RecognitionRange;//인식범위
        public int NumProjectiles;// 회당 공격횟수
        public string CastingSound; // 시전사운드
        public float AngleBetweenProj;// 발사체 사이 각도
        public float AttackInterval; //공격간격
        public int NumBounce;//바운스 횟수
        public float BounceSpeed;// 바운스 속도
        public float BounceDist;//바운스 거리
        public int NumPenerations; //관통 횟수
        public int CastingEffect; // 스킬 발동시 효과
        public string HitSoundLabel; // 히트사운드
        public float ProbCastingEffect; // 스킬 발동 효과 확률
        public int HitEffect;// 적중시 이펙트
        public float ProbHitEffect; // 스킬 발동 효과 확률
        public float ProjRange; //투사체 사거리
        public float MinCoverage; //최소 효과 적용 범위
        public float MaxCoverage; // 최대 효과 적용 범위
        public float RoatateSpeed; // 회전 속도
        public float ProjSpeed; //발사체 속도
        public float ScaleMultiplier;
    }
    
    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
            {
                dict.Add(skill.DataId, skill);
            }
            return dict;
        }
    }
    #endregion
    
    #region StageData
    [Serializable]
    public class StageData
    {
        public int StageIndex = 1;
        public string StageName;
        public int StageLevel = 1;
        public string MapName;
        public int StageSkill;

        public int FirstWaveCountValue;
        public int FirstWaveClearRewardItemId;
        public int FirstWaveClearRewardItemValue;

        public int SecondWaveCountValue;
        public int SecondWaveClearRewardItemId;
        public int SecondWaveClearRewardItemValue;

        public int ThirdWaveCountValue;
        public int ThirdWaveClearRewardItemId;
        public int ThirdWaveClearRewardItemValue;

        public int ClearReward_Gold;
        public int ClearReward_Exp;
        public string StageImage;
        public List<int> AppearingMonsters;
        public List<WaveData> WaveArray;
    }
    
    public class StageDataLoader : ILoader<int, StageData>
    {
        public List<StageData> stages = new List<StageData>();

        public Dictionary<int, StageData> MakeDict()
        {
            Dictionary<int, StageData> dict = new Dictionary<int, StageData>();
            foreach (StageData stage in stages)
                dict.Add(stage.StageIndex, stage);
            return dict;
        }
    }
    #endregion
    
    #region WaveData
    [System.Serializable]
    public class WaveData
    {
        public int StageIndex = 1;
        public int WaveIndex = 1;
        public float SpawnInterval = 0.5f;
        public int OnceSpawnCount;
        public List<int> MonsterId;
        public List<int> EleteId;
        public List<int> BossId;
        public float RemainsTime;
        public float FirstMonsterSpawnRate;
        public float HpIncreaseRate; //일반 몬스터 웨이브에 맞춰서 hp 증가률
        public float nonDropRate;
        public float SmallGemDropRate;
        public float GreenGemDropRate;
        public float BlueGemDropRate;
        public float YellowGemDropRate;
        public List<int> EliteDropItemId;
        public WaveType WaveType;
    }

    public class WaveDataLoader : ILoader<int, WaveData>
    {
        public List<WaveData> waves = new List<WaveData>();

        public Dictionary<int, WaveData> MakeDict()
        {
            Dictionary<int, WaveData> dict = new Dictionary<int, WaveData>();
            foreach (WaveData wave in waves)
                dict.Add(wave.WaveIndex, wave);
            return dict;
        }
    }
    #endregion
    
    #region DropItemData
    public class DropItemData
    {
        public int DataId;
        public DropableItemType DropItemType;
        public string NameTextID;
        public string DescriptionTextID;
        public string SpriteName;
    }
    [Serializable]
    public class DropItemDataLoader : ILoader<int, DropItemData>
    {
        public List<DropItemData> DropItems = new List<DropItemData>();
        public Dictionary<int, DropItemData> MakeDict()
        {
            Dictionary<int, DropItemData> dict = new Dictionary<int, DropItemData>();
            foreach (DropItemData dtm in DropItems)
                dict.Add(dtm.DataId, dtm);
            return dict;
        }
    }

    #endregion

    #region ContinueData

    [Serializable]
    public class ContinueData
    {
        public int exp;
    }


    #endregion
    
    
    #region SupportSkilllData
    [Serializable]
    public class SupportSkillData
    {
        public int AcquiredLevel;
        public int DataId;
        public SupportSkillType SupportSkillType;
        public SupportSkillName SupportSkillName;
        public SupportSkillGrade SupportSkillGrade;
        public string Name;
        public string Description;
        public string IconLabel;
        public float HpRegen;
        public float HealRate; // 회복량 (최대HP%)
        public float HealBonusRate; // 회복량 증가
        public float MagneticRange; // 아이템 습득 범위
        public int SoulAmount; // 영혼획득
        public float HpRate;
        public float AtkRate;
        public float DefRate;
        public float MoveSpeedRate;
        public float CriRate;
        public float CriDmg;
        public float DamageReduction;
        public float ExpBonusRate;
        public float SoulBonusRate;
        public float ProjectileSpacing;// 발사체 사이 간격
        public float Duration; //스킬 지속시간
        public int NumProjectiles;// 회당 공격횟수
        public float AttackInterval; //공격간격
        public int NumBounce;//바운스 횟수
        public int NumPenerations; //관통 횟수
        public float ProjRange; //투사체 사거리
        public float RoatateSpeed; // 회전 속도
        public float ScaleMultiplier;
        public float Price;
        public bool IsLocked = false;
        public bool IsPurchased = false;
        
        // public bool CheckRecommendationCondition()
        // {
        //     if (IsLocked == true || Managers.Game.SoulShopList.Contains(this) == true)
        //     {
        //         return false;
        //     }
        //
        //     if (SupportSkillType == Define.SupportSkillType.Special)
        //     {
        //         //내가 가지고 있는 장비스킬이 아니면 false
        //         if (Managers.Game.EquippedEquipments.TryGetValue(Define.EquipmentType.Weapon, out Equipment myWeapon))
        //         {
        //             int skillId = myWeapon.EquipmentData.BasicSkill;
        //             SkillType type = Util.GetSkillTypeFromInt(skillId);
        //
        //             switch (SupportSkillName)
        //             {
        //                 case Define.SupportSkillName.ArrowShot:
        //                 case Define.SupportSkillName.SavageSmash:
        //                 case Define.SupportSkillName.PhotonStrike:
        //                 case Define.SupportSkillName.Shuriken:
        //                 case Define.SupportSkillName.EgoSword:
        //                     if (SupportSkillName.ToString() != type.ToString())
        //                         return false;
        //                     break;
        //             }
        //
        //         }
        //     }
        //     #region 서포트 스킬 중복 방지 모드 보류
        //     //if (Managers.Game.Player.Skills.SupportSkills.TryGetValue(SupportSkillName, out var existingSkill))
        //     //{
        //     //    if (existingSkill == null)
        //     //        return true;
        //
        //     //    if (DataId <= existingSkill.DataId)
        //     //    {
        //     //        return false;
        //     //    }
        //     //}
        //     #endregion
        //
        //     return true;
        // }
    }
    [Serializable]
    public class SupportSkillDataLoader : ILoader<int, SupportSkillData>
    {
        public List<SupportSkillData> supportSkills = new List<SupportSkillData>();

        public Dictionary<int, SupportSkillData> MakeDict()
        {
            Dictionary<int, SupportSkillData> dict = new Dictionary<int, SupportSkillData>();
            foreach (SupportSkillData skill in supportSkills)
                dict.Add(skill.DataId, skill);
            return dict;
        }
    }
    #endregion
    
    #region MaterialtData
    [Serializable]
    public class MaterialData
    {
        public int DataId;
        public MaterialType MaterialType;
        public MaterialGrade MaterialGrade;
        public string NameTextID;
        public string DescriptionTextID;
        public string SpriteName;

    }

    [Serializable]
    public class MaterialDataLoader : ILoader<int, MaterialData>
    {
        public List<MaterialData> Materials = new List<MaterialData>();
        public Dictionary<int, MaterialData> MakeDict()
        {
            Dictionary<int, MaterialData> dict = new Dictionary<int, MaterialData>();
            foreach (MaterialData mat in Materials)
                dict.Add(mat.DataId, mat);
            return dict;
        }
    }
    #endregion

    #region DefaultUserData

    [Serializable]
    public class DefaultUserData
    {
        public int itemId;
        public string itemName;
        public int itemValue;
    }

    [Serializable]
    public class DefaultUserDataLoader : ILoader<int, DefaultUserData>
    {
        public List<DefaultUserData> defaultUserDataList = new ();

        public Dictionary<int, DefaultUserData> MakeDict()
        {
            Dictionary<int, DefaultUserData> dict = new ();
            foreach (DefaultUserData item in defaultUserDataList)
                dict.Add(item.itemId, item);
            return dict;
        }
    }

    #endregion
 
    #region EquipmentData
    [Serializable]
    public class EquipmentData
    {
        public string DataId;
        public GachaRarity GachaRarity;
        public EquipmentType EquipmentType;
        public EquipmentGrade EquipmentGrade;
        public string NameTextID;
        public string DescriptionTextID;
        public string SpriteName;
        public string HpRegen;
        public int MaxHpBonus;
        public int MaxHpBonusPerUpgrade;
        public int AtkDmgBonus;
        public int AtkDmgBonusPerUpgrade;
        public int MaxLevel;
        public int UncommonGradeSkill;
        public int RareGradeSkill;
        public int EpicGradeSkill;
        public int LegendaryGradeSkill;
        public int BasicSkill;
        public MergeEquipmentType MergeEquipmentType1;
        public string MergeEquipment1;
        public MergeEquipmentType MergeEquipmentType2;
        public string MergeEquipment2;
        public string MergedItemCode;
        public int LevelupMaterialID;
        public string DowngradeEquipmentCode;
        public string DowngradeMaterialCode;
        public int DowngradeMaterialCount;
    }

    [Serializable]
    public class EquipmentDataLoader : ILoader<string, EquipmentData>
    {
        public List<EquipmentData> Equipments = new List<EquipmentData>();
        public Dictionary<string, EquipmentData> MakeDict()
        {
            Dictionary<string, EquipmentData> dict = new Dictionary<string, EquipmentData>();
            foreach (EquipmentData equip in Equipments)
                dict.Add(equip.DataId, equip);
            return dict;
        }
    }
    #endregion
    
    #region LevelData
    [Serializable]
    public class EquipmentLevelData
    {
        public int Level;
        public int UpgradeCost;
        public int UpgradeRequiredItems;
    }

    [Serializable]
    public class EquipmentLevelDataLoader : ILoader<int, EquipmentLevelData>
    {
        public List<EquipmentLevelData> levels = new List<EquipmentLevelData>();
        public Dictionary<int, EquipmentLevelData> MakeDict()
        {
            Dictionary<int, EquipmentLevelData> dict = new Dictionary<int, EquipmentLevelData>();

            foreach (EquipmentLevelData levelData in levels)
                dict.Add(levelData.Level, levelData);
            return dict;
        }
    }
    #endregion

    #region ShopData
    [Serializable]
    public class ShopData
    {
        public int ID;
        public string Title;
        public ShopType ShopType;
        public ShopItemType ShopItemType;
        public int CostItemType;
        public int CostValue;
        public int RewardItemType;
        public int RewardItemValue;
        public string RewardSpriteName;
        public string CostSpriteName;
        public GachaType GachaType;
    }

    [Serializable]
    public class ShopDataDataLoader : ILoader<int, ShopData>
    {
        public List<ShopData> shops = new List<ShopData>();
        public Dictionary<int, ShopData> MakeDict()
        {
            Dictionary<int, ShopData> dict = new Dictionary<int, ShopData>();

            foreach (ShopData shopData in shops)
                dict.Add(shopData.ID, shopData);
            return dict;
        }
    }
    
    #endregion
    
    #region GachaData
    public class GachaTableData
    {
        public GachaType Type;
        public List<GachaRateData> GachaRateTable = new List<GachaRateData>();
    }
    
    [Serializable]
    public class GachaDataLoader : ILoader<GachaType, GachaTableData>
    {
        public List<GachaTableData> GachaTable = new List<GachaTableData>();
        public Dictionary<GachaType, GachaTableData> MakeDict()
        {
            Dictionary<GachaType, GachaTableData> dict = new Dictionary<GachaType, GachaTableData>();
            foreach (GachaTableData gacha in GachaTable)
                dict.Add(gacha.Type, gacha);
            return dict;
        }
    }
    #endregion

    #region GachaRateData
    public class GachaRateData
    {
        public string EquipmentID;
        public float GachaRate;
        public EquipmentGrade EquipGrade;

    }

    #endregion
    
    #region CheckOutData
    public class CheckOutData
    {
        public int Day;
        public int RewardItemId;
        public int MissionTarRewardItemValuegetValue;
    }

    [Serializable]
    public class CheckOutDataLoader : ILoader<int, CheckOutData>
    {
        public List<CheckOutData> checkouts = new List<CheckOutData>();
        public Dictionary<int, CheckOutData> MakeDict()
        {
            Dictionary<int, CheckOutData> dict = new Dictionary<int, CheckOutData>();
            foreach (CheckOutData chk in checkouts)
                dict.Add(chk.Day, chk);
            return dict;
        }
    }
    #endregion
    
    
    #region MissionData
    public class MissionData
    {
        public int MissionId;
        public MissionType MissionType;
        public string DescriptionTextID;
        public MissionTarget MissionTarget;
        public int MissionTargetValue;
        public int ClearRewardItmeId;
        public int RewardValue;
    }

    [Serializable]
    public class MissionDataLoader : ILoader<int, MissionData>
    {
        public List<MissionData> missions = new List<MissionData>();
        public Dictionary<int, MissionData> MakeDict()
        {
            Dictionary<int, MissionData> dict = new Dictionary<int, MissionData>();
            foreach (MissionData mis in missions)
                dict.Add(mis.MissionId, mis);
            return dict;
        }
    }
    #endregion
    
    #region AchievementData
    [Serializable]
    public class AchievementData
    {
        public int AchievementID;
        public string DescriptionTextID;
        public MissionTarget MissionTarget;
        public int MissionTargetValue;
        public int ClearRewardItmeId;
        public int RewardValue;
        public bool IsCompleted;
        public bool IsRewarded;
        public int ProgressValue;
    }

    [Serializable]
    public class AchievementDataLoader : ILoader<int, AchievementData>
    {
        public List<AchievementData> Achievements = new List<AchievementData>();
        public Dictionary<int, AchievementData> MakeDict()
        {
            Dictionary<int, AchievementData> dict = new Dictionary<int, AchievementData>();
            foreach (AchievementData ach in Achievements)
                dict.Add(ach.AchievementID, ach);
            return dict;
        }
    }
    #endregion
    
    #region OfflineRewardData
    public class OfflineRewardData
    {
        public int StageIndex;
        public int Reward_Gold;
        public int Reward_Exp;
        public int FastReward_Scroll;
        public int FastReward_Box;
    }

    [Serializable]
    public class OfflineRewardDataLoader : ILoader<int, OfflineRewardData>
    {
        public List<OfflineRewardData> offlines = new List<OfflineRewardData>();
        public Dictionary<int, OfflineRewardData> MakeDict()
        {
            Dictionary<int, OfflineRewardData> dict = new Dictionary<int, OfflineRewardData>();
            foreach (OfflineRewardData ofr in offlines)
                dict.Add(ofr.StageIndex, ofr);
            return dict;
        }
    }
    #endregion

}