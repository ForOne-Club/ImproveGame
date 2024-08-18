﻿using ImproveGame.Common.Configs.Elements;
using ImproveGame.Common.GlobalNPCs;
using ImproveGame.Common.GlobalProjectiles;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Functions;
using ImproveGame.UI.ModernConfig;
using Newtonsoft.Json;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs;

public class ImproveConfigs : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;
    public override void OnLoaded() => Config = this;

    [JsonIgnore]
    // 用于Config选项收藏功能，判别这个Config实例是不是真正的ImproveConfigs
    // 以决定要不要显示“已收藏”文本
    public bool IsRealImproveConfigs = true;

    #region 其他

    [Header("Others")]

    [CustomModConfigItem(typeof(OtherFunctionsElement))]
    public object OtherFunctions;

    [CustomModConfigItem(typeof(OpenConfigElement))]
    public object OpenConfig;

    [CustomModConfigItem(typeof(FaqElement))]
    public object OpenFaq;

    #endregion

    #region 预设

    [Header("Presets")]
    [CustomModConfigItem(typeof(ILoveBalanceElement))]
    public object ILoveBalance;

    [CustomModConfigItem(typeof(FukMeCalamityElement))]
    public object FukMeCalamity;

    [CustomModConfigItem(typeof(AllOffElement))]
    public object AllOff;

    #endregion

    #region 物品设置

    [Header("Item")]
    [DefaultValue(9999)]
    [Range(1, int.MaxValue)]
    [ReloadRequired]
    public int ItemMaxStack;

    [DefaultValue(true)]
    [ReloadRequired]
    public bool QuestFishStack;

    [DefaultValue(false)]
    public bool SuperVault;

    /// <summary>
    /// 超级虚空保险库
    /// </summary>
    [DefaultValue(false)]
    public bool SuperVoidVault;

    /// <summary>
    /// 智能虚空保险库
    /// </summary>
    [DefaultValue(false)]
    public bool SmartVoidVault;

    [DefaultValue(0)]
    [Slider]
    [Range(0, 75)]
    public int GrabDistance;

    [DefaultValue(0f)]
    [Range(0, 1f)]
    [Slider]
    [Increment(0.125f)]
    public float ExtraToolSpeed;

    [DefaultValue(false)]
    public bool ModifyPlayerPlaceSpeed;

    [DefaultValue(0)]
    [Slider]
    [Range(0, 20)]
    [Increment(2)]
    public int ModifyPlayerTileRange;

    public List<string> TileSpeed_Blacklist = new() { new("torch") };

    [DefaultValue(false)]
    public bool NoConsume_SummonItem;

    [DefaultValue(true)]
    public bool NoConsume_Ammo;

    [DefaultValue(true)]
    public bool NoConsume_Projectile;

    [DefaultValue(true)]
    public bool ImprovePrefix;

    [DefaultValue(true)]
    public bool MiddleEnableBank;

    [DefaultValue(true)]
    public bool FasterExtractinator;

    [DefaultValue(true)]
    public bool BedEverywhere;

    [DefaultValue(true)]
    public bool NoSleepRestrictions;

    [DefaultValue(false)]
    public bool NoPylonRestrictions;

    [DefaultValue(5)]
    [Increment(5)]
    [Range(5, 100)]
    [Slider]
    public int BedTimeRate;

    #endregion

    #region 物品效果设置

    [Header("ItemEffect")]
    [DefaultValue(true)]
    public bool PortableCraftingStation;

    [DefaultValue(true)]
    public bool NoPlace_BUFFTile;

    [DefaultValue(true)]
    public bool NoPlace_BUFFTile_Banner;

    [DefaultValue(true)]
    public bool NoConsume_Potion;

    [DisplayCondition(nameof(ImproveConfigs), nameof(NoConsume_Potion))]
    [DefaultValue(30)]
    [Range(10, 300)]
    public int NoConsume_PotionRequirement;

    [DefaultValue(false)]
    public bool InfiniteRedPotion;

    [DefaultValue(false)]
    [DisplayCondition(nameof(ImproveConfigs), nameof(InfiniteRedPotion))]
    public bool RedPotionEverywhere;

    [DefaultValue(30)]
    [Range(10, 300)]
    [DisplayCondition(nameof(ImproveConfigs), nameof(InfiniteRedPotion))]
    public int RedPotionRequirement;

    #endregion

    #region NPC设置

    [Header("NPC")]
    [DefaultValue(true)]
    public bool TownNPCHome;

    [DefaultValue(false)]
    public bool TownNPCGetTFIntoHouse;

    [DefaultValue(true)]
    public bool NPCLiveInEvil;

    [Slider]
    [Range(1, 10)]
    [DefaultValue(1)]
    public int TownNPCSpawnSpeed;

    [DefaultValue(FishQuestResetType.NotResetFish)]
    [DrawTicks]
    public FishQuestResetType NoCD_FishermanQuest;

    [Range(1, 25)]
    [DefaultValue(1)]
    [Slider]
    public int NPCCoinDropRate;

    [DefaultValue(false)]
    public bool ModifyNPCHappiness;

    [DisplayCondition(nameof(ImproveConfigs), nameof(ModifyNPCHappiness))]
    [DefaultValue(75)]
    [Range(75, 150)]
    [Slider]
    public int NPCHappiness;

    [DefaultValue(false)]
    public bool SlimeExDrop;

    [DefaultValue(false)]
    public bool LavalessLavaSlime;

    [DefaultValue(false)]
    public bool TravellingMerchantStay;

    [DefaultValue(true)]
    public bool TravellingMerchantRefresh;

    [DefaultValue(true)]
    public bool QuickNurse;

    [DefaultValue(true)]
    public bool BestiaryQuickUnlock;

    [Slider]
    [Range(0.1f, 10f)]
    [Increment(0.05f)]
    [DefaultValue(1f)]
    public float BannerRequirement;

    #endregion

    #region 游戏机制

    [Header("GameMechanics")]
    [DefaultValue(99)]
    [Range(0, 99)]
    [Slider]
    [Increment(11)]
    [ReloadRequired]
    public int ExtraPlayerBuffSlots;

    [DefaultValue(false)]
    public bool AlchemyGrassGrowsFaster;

    [DefaultValue(false)]
    public bool AlchemyGrassAlwaysBlooms;

    [DefaultValue(false)]
    public bool StaffOfRegenerationAutomaticPlanting;

    [DefaultValue(true)]
    public bool NoBiomeSpread;

    [DefaultValue(true)]
    public bool RespawnWithFullHP;

    [DefaultValue(true)]
    public bool DontDeleteBuff;

    [DefaultValue(false)]
    public bool JourneyResearch;

    [DefaultValue(false)]
    public bool BanDamageVar;

    [DefaultValue(true)]
    public bool NoLakeSizePenalty;

    [DefaultValue(0)]
    [Range(0, 100)]
    [Increment(5)]
    [Slider]
    public int ResurrectionTimeShortened;

    [DefaultValue(0)]
    [Range(0, 100)]
    [Increment(5)]
    [Slider]
    public int BOSSBattleResurrectionTimeShortened;

    [DefaultValue(false)]
    public bool BanTombstone;

    [DefaultValue(true)]
    public bool LongerExpertDebuff;

    [DefaultValue(false)]
    public bool LightNotBlocked;

    [DefaultValue(BombsNotDamageType.Disabled)]
    [DrawTicks]
    public BombsNotDamageType BombsNotDamage;

    [DefaultValue(true)]
    public bool AmmoChain;

    [DefaultValue(true)]
    public bool SimpleVeinMining;

    [DefaultValue(false)]
    public bool DisableVeinMiningPopup;

    #endregion

    #region 树木设置

    [Header("Tree")]
    [DefaultValue(true)]
    public bool TreeGrowFaster;

    [DefaultValue(false)]
    public bool ShakeTreeFruit;

    [DefaultValue(false)]
    public bool GemTreeAlwaysDropGem;

    [Range(1, 80)]
    [DefaultValue(5)]
    public int MostTreeMin;

    [Range(1, 80)]
    [DefaultValue(16)]
    public int MostTreeMax;

    [Range(1, 80)]
    [DefaultValue(10)]
    public int PalmTreeMin;

    [Range(1, 80)]
    [DefaultValue(20)]
    public int PalmTreeMax;

    [Range(1, 80)]
    [DefaultValue(7)]
    public int GemTreeMin;

    [Range(1, 80)]
    [DefaultValue(12)]
    public int GemTreeMax;

    #endregion

    #region 多人设置

    [Header("Together")]
    [DefaultValue(true)]
    public bool ShareCraftingStation;

    [DefaultValue(false)]
    public bool ShareInfBuffs;

    [DefaultValue(-1)]
    [Range(-1, 2000)]
    public int ShareRange;

    [DefaultValue(false)]
    public bool TeamAutoJoin;

    [DefaultValue(false)]
    public bool BedOnlyOne;

    [DefaultValue(true)]
    public bool NoConditionTP;

    #endregion

    #region 模组设置

    [Header("Server")]
    [DefaultValue(false)]
    public bool OnlyHost;

    [DefaultValue(false)]
    [ReloadRequired]
    public bool OnlyHostByPassword;

    [DefaultValue(50)]
    [Range(1, 100)]
    public int SpawnRateMaxValue;

    [DefaultValue(true)]
    public bool EmptyAutofisher;

    [DefaultValue(11)]
    [Range(5, 60)]
    [Slider]
    public int ExStorageSearchDistance;

    [DefaultValue(true)]
    public bool WorldFeaturePanel;

    [DefaultValue(true)]
    public bool WeatherControl;

    [DefaultValue(true)]
    public bool MinimapMark;

    [DefaultValue(true)]
    public bool WandMaterialNoConsume;

    // [DefaultValue(true)]
    // public bool WandManaConsumption;

    [ReloadRequired]
    public ModItemLoadPage LoadModItems = new();

    [SeparatePage]
    public class ModItemLoadPage
    {
        [DefaultValue(true)]
        public bool MagickWand = true;

        [DefaultValue(true)]
        public bool SpaceWand = true;

        [DefaultValue(true)]
        public bool StarburstWand = true;

        [DefaultValue(true)]
        public bool WallPlace = true;

        [DefaultValue(true)]
        public bool CreateWand = true;

        [DefaultValue(true)]
        public bool LiquidWand = true;

        [DefaultValue(true)]
        public bool PotionBag = true;

        [DefaultValue(true)]
        public bool BannerChest = true;

        [DefaultValue(true)]
        public bool Autofisher = true;

        [DefaultValue(true)]
        public bool PaintWand = true;

        [DefaultValue(true)]
        public bool ConstructWand = true;

        [DefaultValue(true)]
        public bool MoveChest = true;

        [DefaultValue(true)]
        public bool CoinOne = true;

        [DefaultValue(true)]
        public bool ExtremeStorage = true;

        [DefaultValue(true)]
        public bool DetectorDrone = true;

        [DefaultValue(true)]
        public bool BaitSupplier = true;

        public override bool Equals(object obj)
        {
            if (obj is ModItemLoadPage other)
                return MagickWand == other.MagickWand && SpaceWand == other.SpaceWand && StarburstWand == other.StarburstWand &&
                       WallPlace == other.WallPlace && CreateWand == other.CreateWand && LiquidWand == other.LiquidWand &&
                       PotionBag == other.PotionBag && BannerChest == other.BannerChest && Autofisher == other.Autofisher &&
                       PaintWand == other.PaintWand && ConstructWand == other.ConstructWand && MoveChest == other.MoveChest &&
                       CoinOne == other.CoinOne && ExtremeStorage == other.ExtremeStorage && DetectorDrone == other.DetectorDrone &&
                       BaitSupplier == other.BaitSupplier;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new
            {
                MagickWand, SpaceWand, StarburstWand, WallPlace, CreateWand, LiquidWand, PotionBag,
                BannerChest, Autofisher, PaintWand, ConstructWand, MoveChest, CoinOne, ExtremeStorage,
                DetectorDrone, BaitSupplier
            }.GetHashCode();
        }
    }

    #endregion

    public override void OnChanged()
    {
        if (MostTreeMin > MostTreeMax)
        {
            MostTreeMin = MostTreeMax;
        }
        if (GemTreeMin > GemTreeMax)
        {
            GemTreeMin = GemTreeMax;
        }
        if (PalmTreeMin > PalmTreeMax)
        {
            PalmTreeMin = PalmTreeMax;
        }
        HigherTreeSystem.SetTreeHeights(GemTreeMin, GemTreeMax, MostTreeMin, MostTreeMax);
    }

    public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
    {
        if (OnlyHostByPassword)
        {
            if (!NetPasswordSystem.Registered[whoAmI])
            {
                message = new NetworkText(
                    GetText("Configs.ImproveConfigs.OnlyHostByPassword.Unaccepted"), NetworkText.Mode.Literal);
            }
            return NetPasswordSystem.Registered[whoAmI];
        }

        if (((ImproveConfigs)pendingConfig).OnlyHost != OnlyHost)
        {
            return TryAcceptChanges(whoAmI, ref message);
        }
        else if (OnlyHost)
        {
            return TryAcceptChanges(whoAmI, ref message);
        }
        return base.AcceptClientChanges(pendingConfig, whoAmI, ref message);
    }

    private static bool TryAcceptChanges(int whoAmI, ref NetworkText message)
    {
        if (NetMessage.DoesPlayerSlotCountAsAHost(whoAmI))
            return true;

        message = new NetworkText(
            GetText("Configs.ImproveConfigs.OnlyHost.Unaccepted"), NetworkText.Mode.Literal);
        return false;
    }
}
