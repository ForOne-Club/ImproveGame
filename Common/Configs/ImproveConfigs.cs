using ImproveGame.Common.Systems;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs
{
    [Label("$Mods.ImproveGame.Config.ImproveConfigs.Label")]
    public class ImproveConfigs : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public override void OnLoaded() => Config = this;

        // 物品设置
        [Header("$Mods.ImproveGame.Config.ItemConfigs.Header")]

        [Label("$Mods.ImproveGame.Config.SuperVoidVault.Label")]
        [Tooltip("$Mods.ImproveGame.Config.SuperVoidVault.Tooltip")]
        [DefaultValue(false)]
        public bool SuperVoidVault;

        [Label("$Mods.ImproveGame.Config.SmartVoidVault.Label")]
        [Tooltip("$Mods.ImproveGame.Config.SmartVoidVault.Tooltip")]
        [DefaultValue(false)]
        public bool SmartVoidVault;

        [Label("$Mods.ImproveGame.Config.SuperVault.Label")]
        [Tooltip("$Mods.ImproveGame.Config.SuperVault.Tooltip")]
        [DefaultValue(false)]
        public bool SuperVault;

        [Label("$Mods.ImproveGame.Config.GrabDistance.Label")]
        [Tooltip("$Mods.ImproveGame.Config.GrabDistance.Tooltip")]
        [DefaultValue(0)]
        [Slider]
        [Range(0, 25)]
        public int GrabDistance;

        [Label("$Mods.ImproveGame.Config.ItemMaxStack.Label")]
        [DefaultValue(9999)]
        [Range(1, int.MaxValue)]
        public int ItemMaxStack;

        [Label("$Mods.ImproveGame.Config.AutoReuseWeapon.Label")]
        [DefaultValue(true)]
        public bool AutoReuseWeapon;

        [Label("$Mods.ImproveGame.Config.AutoReuseWeapon_ExclusionList.Label")]
        [Tooltip("$Mods.ImproveGame.Config.AutoReuseWeapon_ExclusionList.Tooltip")]
        public List<ItemDefinition> AutoReuseWeapon_ExclusionList =
            new List<ItemDefinition> { new(218), new(113), new(495) };

        [Label("$Mods.ImproveGame.Config.ImproveToolSpeed.Label")]
        [Tooltip("$Mods.ImproveGame.Config.ImproveToolSpeed.Tooltip")]
        [DefaultValue(0)]
        [Range(0, 1f)]
        [Slider]
        [Increment(0.125f)]
        public float ExtraToolSpeed;

        [Label("$Mods.ImproveGame.Config.TileSpeedAndTileRange.Label")]
        [DefaultValue(false)]
        public bool ImproveTileSpeedAndTileRange;

        [Label("$Mods.ImproveGame.Config.TileSpeed_Blacklist.Label")]
        [Tooltip("$Mods.ImproveGame.Config.TileSpeed_Blacklist.Tooltip")]
        public List<string> TileSpeed_Blacklist =
            new() { new("torch") };

        [Label("$Mods.ImproveGame.Config.PortableCraftingStation.Label")]
        [Tooltip("$Mods.ImproveGame.Config.PortableCraftingStation.Tooltip")]
        [DefaultValue(true)]
        public bool PortableCraftingStation;

        [Label("$Mods.ImproveGame.Config.NoPlace_BUFFTile.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoPlace_BUFFTile.Tooltip")]
        [DefaultValue(true)]
        public bool NoPlace_BUFFTile;

        [Label("$Mods.ImproveGame.Config.NoPlace_BUFFTile_Banner.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoPlace_BUFFTile_Banner.Tooltip")]
        [DefaultValue(true)]
        public bool NoPlace_BUFFTile_Banner;

        [Label("$Mods.ImproveGame.Config.NoConsume_SummonItem.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoConsume_SummonItem.Tooltip")]
        [DefaultValue(false)]
        public bool NoConsume_SummonItem;

        [Label("$Mods.ImproveGame.Config.NoConsume_Potion.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoConsume_Potion.Tooltip")]
        [DefaultValue(true)]
        public bool NoConsume_Potion;

        [Label("$Mods.ImproveGame.Config.NoConsume_PotionRequirement.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoConsume_PotionRequirement.Tooltip")]
        [DefaultValue(30)]
        [Range(30, 999)]
        public int NoConsume_PotionRequirement;

        [Label("$Mods.ImproveGame.Config.NoConsume_Ammo.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoConsume_Ammo.Tooltip")]
        [DefaultValue(true)]
        public bool NoConsume_Ammo;

        [Label("$Mods.ImproveGame.Config.HideNoConsumeBuffs.Label")]
        [Tooltip("$Mods.ImproveGame.Config.HideNoConsumeBuffs.Tooltip")]
        [DefaultValue(false)]
        public bool HideNoConsumeBuffs;

        [Label("$Mods.ImproveGame.Config.ImprovePrefix.Label")]
        [Tooltip("$Mods.ImproveGame.Config.ImprovePrefix.Tooltip")]
        [DefaultValue(false)]
        public bool ImprovePrefix;

        [Label("$Mods.ImproveGame.Config.ShowPrefixCount.Label")]
        [Tooltip("$Mods.ImproveGame.Config.ShowPrefixCount.Tooltip")]
        [DefaultValue(true)]
        public bool ShowPrefixCount;

        [Label("$Mods.ImproveGame.Config.ShowItemMoreData.Label")]
        [Tooltip("$Mods.ImproveGame.Config.ShowItemMoreData.Tooltip")]
        [DefaultValue(false)]
        public bool ShowItemMoreData;

        [Label("$Mods.ImproveGame.Config.BanTombstone.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoNeedReload")]
        [DefaultValue(false)]
        public bool BanTombstone;

        [Label("$Mods.ImproveGame.Config.MiddleEnableBank.Label")]
        [Tooltip("$Mods.ImproveGame.Config.MiddleEnableBank.Tooltip")]
        [DefaultValue(true)]
        public bool MiddleEnableBank;

        [Label("$Mods.ImproveGame.Config.AutoSaveMoney.Label")]
        [Tooltip("$Mods.ImproveGame.Config.AutoSaveMoney.Tooltip")]
        [DefaultValue(true)]
        public bool AutoSaveMoney;

        [Label("$Mods.ImproveGame.Config.FasterExtractinator.Label")]
        [Tooltip("$Mods.ImproveGame.Config.FasterExtractinator.Tooltip")]
        [DefaultValue(true)]
        public bool FasterExtractinator;

        [Header("$Mods.ImproveGame.Config.NPCConfigs.Header")]

        [Label("$Mods.ImproveGame.Config.TownNPCGetTFIntoHouse.Label")]
        [Tooltip("$Mods.ImproveGame.Config.TownNPCGetTFIntoHouse.Tooltip")]
        [DefaultValue(false)]
        public bool TownNPCGetTFIntoHouse;

        [Label("$Mods.ImproveGame.Config.TownNPCSpawnInNight.Label")]
        [DefaultValue(false)]
        public bool TownNPCSpawnInNight;

        [Label("$Mods.ImproveGame.Config.TownNPCSpawnSpeed.Label")]
        [Tooltip("$Mods.ImproveGame.Config.TownNPCSpawnSpeed.Tooltip")]
        [Slider]
        [Range(-1, 12)]
        [DefaultValue(-1)]
        public float TownNPCSpawnSpeed;

        [Label("$Mods.ImproveGame.Config.NoCD_FishermanQuest.Label")]
        [DefaultValue(false)]
        public bool NoCD_FishermanQuest;

        [Label("$Mods.ImproveGame.Config.NPCCoinDropRate.Label")]
        [Range(1, 25)]
        [DefaultValue(1)]
        [Slider]
        public int NPCCoinDropRate;

        [Label("$Mods.ImproveGame.Config.LavalessLavaSlime.Label")]
        [Tooltip("$Mods.ImproveGame.Config.LavalessLavaSlime.Tooltip")]
        [DefaultValue(false)]
        public bool LavalessLavaSlime;

        [Label("$Mods.ImproveGame.Config.TravellingMerchantStay.Label")]
        [Tooltip("$Mods.ImproveGame.Config.TravellingMerchantStay.Tooltip")]
        [DefaultValue(false)]
        public bool TravellingMerchantStay;

        [Label("$Mods.ImproveGame.Config.TravellingMerchantRefresh.Label")]
        [Tooltip("$Mods.ImproveGame.Config.TravellingMerchantRefresh.Tooltip")]
        [DefaultValue(true)]
        public bool TravellingMerchantRefresh;

        [Label("$Mods.ImproveGame.Config.QuickNurse.Label")]
        [Tooltip("$Mods.ImproveGame.Config.QuickNurse.Tooltip")]
        [DefaultValue(true)]
        public bool QuickNurse;

        [Label("$Mods.ImproveGame.Config.BestiaryQuickUnlock.Label")]
        [Tooltip("$Mods.ImproveGame.Config.BestiaryQuickUnlock.Tooltip")]
        [DefaultValue(true)]
        public bool BestiaryQuickUnlock;

        [Label("$Mods.ImproveGame.Config.BannerRequirement.Label")]
        [Tooltip("$Mods.ImproveGame.Config.BannerRequirement.Tooltip")]
        [Slider]
        [Range(0.1f, 10f)]
        [Increment(0.05f)]
        [DefaultValue(1f)]
        public float BannerRequirement;

        [Header("$Mods.ImproveGame.Config.GameMechanics.Header")]

        [Label("$Mods.ImproveGame.Config.AlchemyGrassGrowsFaster.Label")]
        [Tooltip("$Mods.ImproveGame.Config.AlchemyGrassGrowsFaster.Tooltip")]
        [DefaultValue(false)]
        public bool AlchemyGrassGrowsFaster;

        [Label("$Mods.ImproveGame.Config.AlchemyGrassAlwaysBlooms.Label")]
        [Tooltip("$Mods.ImproveGame.Config.AlchemyGrassAlwaysBlooms.Tooltip")]
        [DefaultValue(false)]
        public bool AlchemyGrassAlwaysBlooms;

        [Label("$Mods.ImproveGame.Config.StaffOfRegenerationAutomaticPlanting.Label")]
        [Tooltip("$Mods.ImproveGame.Config.StaffOfRegenerationAutomaticPlanting.Tooltip")]
        [DefaultValue(false)]
        public bool StaffOfRegenerationAutomaticPlanting;

        [Label("$Mods.ImproveGame.Config.RespawnWithFullHP.Label")]
        [DefaultValue(true)]
        public bool RespawnWithFullHP;

        [Label("$Mods.ImproveGame.Config.DontDeleteBuff.Label")]
        [Tooltip("$Mods.ImproveGame.Config.DontDeleteBuff.Tooltip")]
        [DefaultValue(true)]
        public bool DontDeleteBuff;

        [Label("$Mods.ImproveGame.Config.BanDamageVar.Label")]
        [DefaultValue(false)]
        public bool BanDamageVar;

        [Label("$Mods.ImproveGame.Config.ExtraPlayerBuffSlots.Label")]
        [DefaultValue(99)]
        [Range(0, 99)]
        [Slider]
        [Increment(11)]
        [ReloadRequired]
        public int ExtraPlayerBuffSlots;

        [Header("$Mods.ImproveGame.Config.TreeConfigs.Header")]

        [Label("$Mods.ImproveGame.Config.TreeGrowFaster.Label")]
        [Tooltip("$Mods.ImproveGame.Config.TreeGrowFaster.Tooltip")]
        [DefaultValue(true)]
        public bool TreeGrowFaster;

        [Label("$Mods.ImproveGame.Config.ShakeTreeFruit.Label")]
        [Tooltip("$Mods.ImproveGame.Config.ShakeTreeFruit.Tooltip")]
        [DefaultValue(false)]
        public bool ShakeTreeFruit;

        [Label("$Mods.ImproveGame.Config.GemTreeAlwaysDropGem.Label")]
        [Tooltip("$Mods.ImproveGame.Config.GemTreeAlwaysDropGem.Tooltip")]
        [DefaultValue(false)]
        public bool GemTreeAlwaysDropGem;

        [Range(1, 100)]
        [DefaultValue(5)]
        [Label("$Mods.ImproveGame.Config.MostTree.LabelMin")]
        public int MostTreeMin;

        [Range(1, 100)]
        [DefaultValue(16)]
        [Label("$Mods.ImproveGame.Config.MostTree.LabelMax")]
        public int MostTreeMax;

        [Range(1, 100)]
        [DefaultValue(10)]
        [Label("$Mods.ImproveGame.Config.PalmTree.LabelMin")]
        public int PalmTreeMin;

        [Range(1, 100)]
        [DefaultValue(20)]
        [Label("$Mods.ImproveGame.Config.PalmTree.LabelMax")]
        public int PalmTreeMax;

        [Range(1, 100)]
        [DefaultValue(7)]
        [Label("$Mods.ImproveGame.Config.GemTree.LabelMin")]
        public int GemTreeMin;

        [Range(1, 100)]
        [DefaultValue(12)]
        [Label("$Mods.ImproveGame.Config.GemTree.LabelMax")]
        public int GemTreeMax;

        [Header("$Mods.ImproveGame.Config.TogetherConfigs.Header")]

        [Label("$Mods.ImproveGame.Config.ShareCraftingStation.Label")]
        [Tooltip("$Mods.ImproveGame.Config.ShareCraftingStation.Tooltip")]
        [DefaultValue(true)]
        public bool ShareCraftingStation;

        [Label("$Mods.ImproveGame.Config.ShareInfBuffs.Label")]
        [Tooltip("$Mods.ImproveGame.Config.ShareInfBuffs.Tooltip")]
        [DefaultValue(false)]
        public bool ShareInfBuffs;

        [Label("$Mods.ImproveGame.Config.ShareRange.Label")]
        [Tooltip("$Mods.ImproveGame.Config.ShareRange.Tooltip")]
        [DefaultValue(-1)]
        [Range(-1, 2000)]
        public int ShareRange;

        [Header("$Mods.ImproveGame.Config.ServerSettings.Header")]

        [Label("$Mods.ImproveGame.Config.OnlyHost.Label")]
        [Tooltip("$Mods.ImproveGame.Config.OnlyHost.Tooltip")]
        [DefaultValue(false)]
        public bool OnlyHost;

        [Label("$Mods.ImproveGame.Config.OnlyHostByPassword.Label")]
        [Tooltip("$Mods.ImproveGame.Config.OnlyHostByPassword.Tooltip")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool OnlyHostByPassword;

        [Label("$Mods.ImproveGame.Config.SpawnRateMaxValue.Label")]
        [Tooltip("$Mods.ImproveGame.Config.SpawnRateMaxValue.Tooltip")]
        [DefaultValue(50)]
        [Range(1, 100)]
        public int SpawnRateMaxValue;

        [Label("$Mods.ImproveGame.Config.LoadModItems.Label")]
        [Tooltip("$Mods.ImproveGame.Config.LoadModItems.Tooltip")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool LoadModItems;

        // 预设
        [Header("$Mods.ImproveGame.Config.Presets.Header")]

        [Label("$Mods.ImproveGame.Config.ILoveBalance.Label")]
        [Tooltip("$Mods.ImproveGame.Config.ILoveBalance.Tooltip")]
        [DefaultValue(false)]
        public bool ILoveBalance
        {
            get =>
                SuperVoidVault is false &&
                SmartVoidVault is false &&
                SuperVault is false &&
                ItemMaxStack is 9999 &&
                GrabDistance is 5 &&
                NoConsume_SummonItem is false &&
                AutoReuseWeapon is false &&
                ExtraToolSpeed is 0.125f &&
                ImproveTileSpeedAndTileRange is true &&
                PortableCraftingStation is true &&
                NoPlace_BUFFTile_Banner is false &&
                NoConsume_Potion is false &&
                NoConsume_Ammo is true &&
                ImprovePrefix is false &&
                MiddleEnableBank is true &&
                AutoSaveMoney is true &&
                FasterExtractinator is true &&
                TownNPCGetTFIntoHouse is false &&
                TownNPCSpawnInNight is true &&
                TownNPCSpawnSpeed is -1 &&
                NoCD_FishermanQuest is true &&
                NPCCoinDropRate is 1 &&
                LavalessLavaSlime is true &&
                TravellingMerchantStay is false &&
                TravellingMerchantRefresh is true &&
                BestiaryQuickUnlock is false &&
                AlchemyGrassGrowsFaster is false &&
                AlchemyGrassAlwaysBlooms is false &&
                StaffOfRegenerationAutomaticPlanting is true &&
                RespawnWithFullHP is true &&
                DontDeleteBuff is true &&
                ExtraPlayerBuffSlots is 99 &&
                TreeGrowFaster is false &&
                ShakeTreeFruit is false &&
                GemTreeAlwaysDropGem is false &&
                MostTreeMin is 10 &&
                MostTreeMax is 20 &&
                PalmTreeMin is 10 &&
                PalmTreeMax is 20 &&
                GemTreeMin is 7 &&
                GemTreeMax is 12
            ;
            set
            {
                if (value)
                {
                    SuperVoidVault = false;
                    SmartVoidVault = false;
                    SuperVault = false;
                    ItemMaxStack = 9999;
                    GrabDistance = 5;
                    NoConsume_SummonItem = false;
                    AutoReuseWeapon = false;
                    ExtraToolSpeed = 0.125f;
                    ImproveTileSpeedAndTileRange = true;
                    PortableCraftingStation = true;
                    NoPlace_BUFFTile_Banner = false;
                    NoConsume_Potion = false;
                    NoConsume_Ammo = true;
                    ImprovePrefix = false;
                    MiddleEnableBank = true;
                    AutoSaveMoney = true;
                    FasterExtractinator = true;
                    TownNPCGetTFIntoHouse = false;
                    TownNPCSpawnInNight = true;
                    TownNPCSpawnSpeed = -1;
                    NoCD_FishermanQuest = true;
                    NPCCoinDropRate = 1;
                    LavalessLavaSlime = true;
                    TravellingMerchantStay = false;
                    TravellingMerchantRefresh = true;
                    BestiaryQuickUnlock = false;
                    AlchemyGrassGrowsFaster = false;
                    AlchemyGrassAlwaysBlooms = false;
                    StaffOfRegenerationAutomaticPlanting = true;
                    RespawnWithFullHP = true;
                    DontDeleteBuff = true;
                    ExtraPlayerBuffSlots = 99;
                    TreeGrowFaster = false;
                    ShakeTreeFruit = false;
                    GemTreeAlwaysDropGem = false;
                    MostTreeMin = 10;
                    MostTreeMax = 20;
                    PalmTreeMin = 10;
                    PalmTreeMax = 20;
                    GemTreeMin = 7;
                    GemTreeMax = 12;
                }
            }
        }

        [Label("$Mods.ImproveGame.Config.FukMeCalamity.Label")]
        [Tooltip("$Mods.ImproveGame.Config.FukMeCalamity.Tooltip")]
        [DefaultValue(false)]
        public bool FukMeCalamity
        {
            get =>
                SuperVoidVault is true &&
                SmartVoidVault is true &&
                SuperVault is true &&
                ItemMaxStack is 9999 &&
                GrabDistance is 5 &&
                NoConsume_SummonItem is true &&
                AutoReuseWeapon is true &&
                ExtraToolSpeed is 0.5f &&
                ImproveTileSpeedAndTileRange is true &&
                PortableCraftingStation is true &&
                NoPlace_BUFFTile_Banner is true &&
                NoConsume_Potion is true &&
                NoConsume_Ammo is true &&
                ImprovePrefix is true &&
                MiddleEnableBank is true &&
                AutoSaveMoney is true &&
                FasterExtractinator is true &&
                TownNPCGetTFIntoHouse is true &&
                TownNPCSpawnInNight is true &&
                TownNPCSpawnSpeed is 12 &&
                NoCD_FishermanQuest is true &&
                NPCCoinDropRate is 8 &&
                LavalessLavaSlime is true &&
                TravellingMerchantStay is true &&
                TravellingMerchantRefresh is true &&
                BestiaryQuickUnlock is true &&
                AlchemyGrassGrowsFaster is true &&
                AlchemyGrassAlwaysBlooms is true &&
                StaffOfRegenerationAutomaticPlanting is true &&
                RespawnWithFullHP is true &&
                DontDeleteBuff is true &&
                ExtraPlayerBuffSlots is 99 &&
                TreeGrowFaster is true &&
                ShakeTreeFruit is true &&
                GemTreeAlwaysDropGem is true &&
                MostTreeMin is 22 &&
                MostTreeMax is 36 &&
                PalmTreeMin is 22 &&
                PalmTreeMax is 36 &&
                GemTreeMin is 20 &&
                GemTreeMax is 30
            ;
            set
            {
                if (value)
                {
                    SuperVoidVault = true;
                    SmartVoidVault = true;
                    SuperVault = true;
                    ItemMaxStack = 9999;
                    GrabDistance = 5;
                    NoConsume_SummonItem = true;
                    AutoReuseWeapon = true;
                    ExtraToolSpeed = 0.5f;
                    ImproveTileSpeedAndTileRange = true;
                    PortableCraftingStation = true;
                    NoPlace_BUFFTile_Banner = true;
                    NoConsume_Potion = true;
                    NoConsume_Ammo = true;
                    ImprovePrefix = true;
                    MiddleEnableBank = true;
                    AutoSaveMoney = true;
                    FasterExtractinator = true;
                    TownNPCGetTFIntoHouse = true;
                    TownNPCSpawnInNight = true;
                    TownNPCSpawnSpeed = 12;
                    NoCD_FishermanQuest = true;
                    NPCCoinDropRate = 8;
                    LavalessLavaSlime = true;
                    TravellingMerchantStay = true;
                    TravellingMerchantRefresh = true;
                    BestiaryQuickUnlock = true;
                    AlchemyGrassGrowsFaster = true;
                    AlchemyGrassAlwaysBlooms = true;
                    StaffOfRegenerationAutomaticPlanting = true;
                    RespawnWithFullHP = true;
                    DontDeleteBuff = true;
                    ExtraPlayerBuffSlots = 99;
                    TreeGrowFaster = true;
                    ShakeTreeFruit = true;
                    GemTreeAlwaysDropGem = true;
                    MostTreeMin = 22;
                    MostTreeMax = 36;
                    PalmTreeMin = 22;
                    PalmTreeMax = 36;
                    GemTreeMin = 20;
                    GemTreeMax = 30;
                }
            }
        }

        [Header("$Mods.ImproveGame.Config.OtherFunctions.Header")]
        [Label("$Mods.ImproveGame.Config.OtherFunctions.Label")]
        public bool OtherFunctions => true;

        public override void OnChanged() {
            if (MostTreeMin > MostTreeMax) {
                MostTreeMin = MostTreeMax;
            }
            if (GemTreeMin > GemTreeMax) {
                GemTreeMin = GemTreeMax;
            }
            if (PalmTreeMin > PalmTreeMax) {
                PalmTreeMin = PalmTreeMax;
            }
            HigherTreeSystem.SetTreeHeights(GemTreeMin, GemTreeMax, MostTreeMin, MostTreeMax);
        }

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message) {
            if (OnlyHostByPassword) {
                if (!NetPasswordSystem.Registered[whoAmI]) {
                    message = GetText("Config.OnlyHostByPassword.Unaccepted");
                }
                return NetPasswordSystem.Registered[whoAmI];
            }

            if ((pendingConfig as ImproveConfigs).OnlyHost != OnlyHost) {
                return TryAcceptChanges(whoAmI, ref message);
            }
            else if (OnlyHost) {
                return TryAcceptChanges(whoAmI, ref message);
            }
            return base.AcceptClientChanges(pendingConfig, whoAmI, ref message);
        }

        public static bool TryAcceptChanges(int whoAmI, ref string message) {
            // DoesPlayerSlotCountAsAHost是preview的，stable还没有，又被坑了
            // if (MessageBuffer.DoesPlayerSlotCountAsAHost(whoAmI)) {
            if (Netplay.Clients[whoAmI].Socket.GetRemoteAddress().IsLocalHost()) {
                return true;
            }
            else {
                message = GetText("Config.OnlyHost.Unaccepted");
                return false;
            }
        }
    }
}
