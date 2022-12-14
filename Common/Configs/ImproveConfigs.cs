using ImproveGame.Common.Systems;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs
{
    [Label($"${path}.ImproveConfigs.Label")]
    public class ImproveConfigs : ModConfig
    {
        const string path = "Mods.ImproveGame.Config";
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public override void OnLoaded() => Config = this;

        #region 物品设置

        [Header($"${path}.ItemConfigs.Header")]

        [Label($"${path}.SuperVoidVault.Label")]
        [Tooltip($"${path}.SuperVoidVault.Tooltip")]
        [DefaultValue(false)]
        public bool SuperVoidVault;

        [Label($"${path}.SmartVoidVault.Label")]
        [Tooltip($"${path}.SmartVoidVault.Tooltip")]
        [DefaultValue(false)]
        public bool SmartVoidVault;

        [Label($"${path}.SuperVault.Label")]
        [Tooltip($"${path}.SuperVault.Tooltip")]
        [DefaultValue(false)]
        public bool SuperVault;

        [Label($"${path}.GrabDistance.Label")]
        [Tooltip($"${path}.GrabDistance.Tooltip")]
        [DefaultValue(0)]
        [Slider]
        [Range(0, 50)]
        public int GrabDistance;

        [Label($"${path}.ItemMaxStack.Label")]
        [DefaultValue(9999)]
        [Range(1, int.MaxValue)]
        public int ItemMaxStack;

        [Label($"${path}.AutoReuseWeapon.Label")]
        [DefaultValue(true)]
        public bool AutoReuseWeapon;

        [Label($"${path}.AutoReuseWeapon_ExclusionList.Label")]
        [Tooltip($"${path}.AutoReuseWeapon_ExclusionList.Tooltip")]
        public List<ItemDefinition> AutoReuseWeaponExclusionList = new() { new(218), new(113), new(495) };

        [Label($"${path}.ImproveToolSpeed.Label")]
        [Tooltip($"${path}.ImproveToolSpeed.Tooltip")]
        [DefaultValue(0d)]
        [Range(0, 1f)]
        [Slider]
        [Increment(0.125f)]
        public float ExtraToolSpeed;

        [Label($"${path}.TileSpeedAndTileRange.Label")]
        [DefaultValue(false)]
        public bool ImproveTileSpeedAndTileRange;

        [Label($"${path}.TileSpeed_Blacklist.Label")]
        [Tooltip($"${path}.TileSpeed_Blacklist.Tooltip")]
        public List<string> TileSpeed_Blacklist = new() { new("torch") };

        [Label($"${path}.PortableCraftingStation.Label")]
        [Tooltip($"${path}.PortableCraftingStation.Tooltip")]
        [DefaultValue(true)]
        public bool PortableCraftingStation;

        [Label($"${path}.NoPlace_BUFFTile.Label")]
        [Tooltip($"${path}.NoPlace_BUFFTile.Tooltip")]
        [DefaultValue(true)]
        public bool NoPlace_BUFFTile;

        [Label($"${path}.NoPlace_BUFFTile_Banner.Label")]
        [Tooltip($"${path}.NoPlace_BUFFTile_Banner.Tooltip")]
        [DefaultValue(true)]
        public bool NoPlace_BUFFTile_Banner;

        [Label($"${path}.NoConsume_SummonItem.Label")]
        [Tooltip($"${path}.NoConsume_SummonItem.Tooltip")]
        [DefaultValue(false)]
        public bool NoConsume_SummonItem;

        [Label($"${path}.NoConsume_Potion.Label")]
        [Tooltip($"${path}.NoConsume_Potion.Tooltip")]
        [DefaultValue(true)]
        public bool NoConsume_Potion;

        [Label($"${path}.NoConsume_PotionRequirement.Label")]
        [Tooltip($"${path}.NoConsume_PotionRequirement.Tooltip")]
        [DefaultValue(30)]
        [Range(10, 999)]
        public int NoConsume_PotionRequirement;

        [Label($"${path}.NoConsume_Ammo.Label")]
        [Tooltip($"${path}.NoConsume_Ammo.Tooltip")]
        [DefaultValue(true)]
        public bool NoConsume_Ammo;

        [Label($"${path}.NoConsume_Projectile.Label")]
        [Tooltip($"${path}.NoConsume_Projectile.Tooltip")]
        [DefaultValue(true)]
        public bool NoConsume_Projectile;

        [Label($"${path}.HideNoConsumeBuffs.Label")]
        [Tooltip($"${path}.HideNoConsumeBuffs.Tooltip")]
        [DefaultValue(false)]
        public bool HideNoConsumeBuffs;

        [Label($"${path}.ImprovePrefix.Label")]
        [Tooltip($"${path}.ImprovePrefix.Tooltip")]
        [DefaultValue(false)]
        public bool ImprovePrefix;

        [Label($"${path}.ShowItemMoreData.Label")]
        [Tooltip($"${path}.ShowItemMoreData.Tooltip")]
        [DefaultValue(false)]
        public bool ShowItemMoreData;

        [Label($"${path}.ResurrectionTimeShortened.Label")]
        [Tooltip($"${path}.ResurrectionTimeShortened.Tooltip")]
        [DefaultValue(0)]
        [Range(0, 100)]
        [Increment(5)]
        [Slider]
        public int ResurrectionTimeShortened;

        [Label($"${path}.BanTombstone.Label")]
        [Tooltip($"${path}.NoNeedReload")]
        [DefaultValue(false)]
        public bool BanTombstone;

        [Label($"${path}.MiddleEnableBank.Label")]
        [Tooltip($"${path}.MiddleEnableBank.Tooltip")]
        [DefaultValue(true)]
        public bool MiddleEnableBank;

        [Label($"${path}.AutoSaveMoney.Label")]
        [Tooltip($"${path}.AutoSaveMoney.Tooltip")]
        [DefaultValue(true)]
        public bool AutoSaveMoney;

        [Label($"${path}.FasterExtractinator.Label")]
        [Tooltip($"${path}.FasterExtractinator.Tooltip")]
        [DefaultValue(true)]
        public bool FasterExtractinator;

        #endregion

        #region NPC设置

        [Header($"${path}.NPCConfigs.Header")]

        [Label($"${path}.TownNPCGetTFIntoHouse.Label")]
        [Tooltip($"${path}.TownNPCGetTFIntoHouse.Tooltip")]
        [DefaultValue(false)]
        public bool TownNPCGetTFIntoHouse;

        [Label($"${path}.NPCLiveInEvil.Label")]
        [Tooltip($"${path}.NPCLiveInEvil.Tooltip")]
        [DefaultValue(true)]
        public bool NPCLiveInEvil;

        [Label($"${path}.TownNPCSpawnSpeed.Label")]
        [Tooltip($"${path}.TownNPCSpawnSpeed.Tooltip")]
        [Slider]
        [Range(-1, 12)]
        [DefaultValue(-1)]
        public int TownNPCSpawnSpeed;

        [Label($"${path}.NoCD_FishermanQuest.Label")]
        [DefaultValue(false)]
        public bool NoCD_FishermanQuest;

        [Label($"${path}.NPCCoinDropRate.Label")]
        [Range(1, 25)]
        [DefaultValue(1)]
        [Slider]
        public int NPCCoinDropRate;

        [Label($"${path}.LavalessLavaSlime.Label")]
        [Tooltip($"${path}.LavalessLavaSlime.Tooltip")]
        [DefaultValue(false)]
        public bool LavalessLavaSlime;

        [Label($"${path}.TravellingMerchantStay.Label")]
        [Tooltip($"${path}.TravellingMerchantStay.Tooltip")]
        [DefaultValue(false)]
        public bool TravellingMerchantStay;

        [Label($"${path}.TravellingMerchantRefresh.Label")]
        [Tooltip($"${path}.TravellingMerchantRefresh.Tooltip")]
        [DefaultValue(true)]
        public bool TravellingMerchantRefresh;

        [Label($"${path}.QuickNurse.Label")]
        [Tooltip($"${path}.QuickNurse.Tooltip")]
        [DefaultValue(true)]
        public bool QuickNurse;

        [Label($"${path}.BestiaryQuickUnlock.Label")]
        [Tooltip($"${path}.BestiaryQuickUnlock.Tooltip")]
        [DefaultValue(true)]
        public bool BestiaryQuickUnlock;

        [Label($"${path}.BannerRequirement.Label")]
        [Tooltip($"${path}.BannerRequirement.Tooltip")]
        [Slider]
        [Range(0.1f, 10f)]
        [Increment(0.05f)]
        [DefaultValue(1f)]
        public float BannerRequirement;

        #endregion

        #region 游戏机制

        [Header($"${path}.GameMechanics.Header")]

        [Label($"${path}.AlchemyGrassGrowsFaster.Label")]
        [Tooltip($"${path}.AlchemyGrassGrowsFaster.Tooltip")]
        [DefaultValue(false)]
        public bool AlchemyGrassGrowsFaster;

        [Label($"${path}.AlchemyGrassAlwaysBlooms.Label")]
        [Tooltip($"${path}.AlchemyGrassAlwaysBlooms.Tooltip")]
        [DefaultValue(false)]
        public bool AlchemyGrassAlwaysBlooms;

        [Label($"${path}.StaffOfRegenerationAutomaticPlanting.Label")]
        [Tooltip($"${path}.StaffOfRegenerationAutomaticPlanting.Tooltip")]
        [DefaultValue(false)]
        public bool StaffOfRegenerationAutomaticPlanting;

        [Label($"${path}.NoBiomeSpread.Label")]
        [DefaultValue(true)]
        public bool NoBiomeSpread;

        [Label($"${path}.RespawnWithFullHP.Label")]
        [DefaultValue(true)]
        public bool RespawnWithFullHP;

        [Label($"${path}.DontDeleteBuff.Label")]
        [Tooltip($"${path}.DontDeleteBuff.Tooltip")]
        [DefaultValue(true)]
        public bool DontDeleteBuff;

        [Label($"${path}.JourneyResearch.Label")]
        [Tooltip($"${path}.JourneyResearch.Tooltip")]
        [DefaultValue(false)]
        public bool JourneyResearch;

        [Label($"${path}.BanDamageVar.Label")]
        [DefaultValue(false)]
        public bool BanDamageVar;

        [Label($"${path}.ExtraPlayerBuffSlots.Label")]
        [DefaultValue(99)]
        [Range(0, 99)]
        [Slider]
        [Increment(11)]
        [ReloadRequired]
        public int ExtraPlayerBuffSlots;

        #endregion

        #region 树木设置

        [Header($"${path}.TreeConfigs.Header")]

        [Label($"${path}.TreeGrowFaster.Label")]
        [Tooltip($"${path}.TreeGrowFaster.Tooltip")]
        [DefaultValue(true)]
        public bool TreeGrowFaster;

        [Label($"${path}.ShakeTreeFruit.Label")]
        [Tooltip($"${path}.ShakeTreeFruit.Tooltip")]
        [DefaultValue(false)]
        public bool ShakeTreeFruit;

        [Label($"${path}.GemTreeAlwaysDropGem.Label")]
        [Tooltip($"${path}.GemTreeAlwaysDropGem.Tooltip")]
        [DefaultValue(false)]
        public bool GemTreeAlwaysDropGem;

        [Range(1, 100)]
        [DefaultValue(5)]
        [Label($"${path}.MostTree.LabelMin")]
        public int MostTreeMin;

        [Range(1, 100)]
        [DefaultValue(16)]
        [Label($"${path}.MostTree.LabelMax")]
        public int MostTreeMax;

        [Range(1, 100)]
        [DefaultValue(10)]
        [Label($"${path}.PalmTree.LabelMin")]
        public int PalmTreeMin;

        [Range(1, 100)]
        [DefaultValue(20)]
        [Label($"${path}.PalmTree.LabelMax")]
        public int PalmTreeMax;

        [Range(1, 100)]
        [DefaultValue(7)]
        [Label($"${path}.GemTree.LabelMin")]
        public int GemTreeMin;

        [Range(1, 100)]
        [DefaultValue(12)]
        [Label($"${path}.GemTree.LabelMax")]
        public int GemTreeMax;

        #endregion

        #region 多人设置

        [Header($"${path}.TogetherConfigs.Header")]

        [Label($"${path}.ShareCraftingStation.Label")]
        [Tooltip($"${path}.ShareCraftingStation.Tooltip")]
        [DefaultValue(true)]
        public bool ShareCraftingStation;

        [Label($"${path}.ShareInfBuffs.Label")]
        [Tooltip($"${path}.ShareInfBuffs.Tooltip")]
        [DefaultValue(false)]
        public bool ShareInfBuffs;

        [Label($"${path}.ShareRange.Label")]
        [Tooltip($"${path}.ShareRange.Tooltip")]
        [DefaultValue(-1)]
        [Range(-1, 2000)]
        public int ShareRange;

        [Label($"${path}.TeamAutoJoin.Label")]
        [Tooltip($"${path}.TeamAutoJoin.Tooltip")]
        [DefaultValue(false)]
        public bool TeamAutoJoin;

        #endregion

        #region 模组设置

        [Header($"${path}.ServerSettings.Header")]

        [Label($"${path}.OnlyHost.Label")]
        [Tooltip($"${path}.OnlyHost.Tooltip")]
        [DefaultValue(false)]
        public bool OnlyHost;

        [Label($"${path}.OnlyHostByPassword.Label")]
        [Tooltip($"${path}.OnlyHostByPassword.Tooltip")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool OnlyHostByPassword;

        [Label($"${path}.SpawnRateMaxValue.Label")]
        [Tooltip($"${path}.SpawnRateMaxValue.Tooltip")]
        [DefaultValue(50)]
        [Range(1, 100)]
        public int SpawnRateMaxValue;

        [Label($"${path}.ShowModName.Label")]
        [DefaultValue(true)]
        public bool ShowModName;

        [Label($"${path}.EmptyAutofisher.Label")]
        [Tooltip($"${path}.EmptyAutofisher.Tooltip")]
        [DefaultValue(true)]
        public bool EmptyAutofisher;

        [Label($"${path}.LoadModItems.Label")]
        [ReloadRequired]
        public ModItemLoadPage LoadModItems = new();

        [SeparatePage]
        public class ModItemLoadPage
        {
            [Label("$Mods.ImproveGame.ItemName.MagickWand")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool MagickWand = true;

            [Label("$Mods.ImproveGame.ItemName.SpaceWand")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool SpaceWand = true;

            [Label("$Mods.ImproveGame.ItemName.StarburstWand")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool StarburstWand = true;

            [Label("$Mods.ImproveGame.ItemName.WallPlace")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool WallPlace = true;

            [Label("$Mods.ImproveGame.ItemName.CreateWand")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool CreateWand = true;

            [Label("$Mods.ImproveGame.ItemName.LiquidWand")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool LiquidWand = true;

            [Label("$Mods.ImproveGame.ItemName.PotionBag")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool PotionBag = true;

            [Label("$Mods.ImproveGame.ItemName.BannerChest")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool BannerChest = true;

            [Label("$Mods.ImproveGame.ItemName.Autofisher")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool Autofisher = true;

            [Label("$Mods.ImproveGame.ItemName.PaintWand")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool PaintWand = true;

            [Label("$Mods.ImproveGame.ItemName.ConstructWand")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool ConstructWand = true;

            [Label("$Mods.ImproveGame.ItemName.MoveChest")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool MoveChest = true;

            [Label("$Mods.ImproveGame.ItemName.CoinOne")]
            [Tooltip($"${path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool CoinOne = true;

            public override bool Equals(object obj)
            {
                if (obj is ModItemLoadPage other)
                    return MagickWand == other.MagickWand && SpaceWand == other.SpaceWand && StarburstWand == other.StarburstWand &&
                           WallPlace == other.WallPlace && CreateWand == other.CreateWand && LiquidWand == other.LiquidWand &&
                           PotionBag == other.PotionBag && BannerChest == other.BannerChest && Autofisher == other.Autofisher &&
                           PaintWand == other.PaintWand && ConstructWand == other.ConstructWand && MoveChest == other.MoveChest &&
                           CoinOne == other.CoinOne;
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return new
                {
                    MagickWand, SpaceWand, StarburstWand, WallPlace, CreateWand, LiquidWand, PotionBag,
                    BannerChest, Autofisher, PaintWand, ConstructWand, MoveChest, CoinOne
                }.GetHashCode();
            }
        }

        #endregion

        #region 预设

        [Header($"${path}.Presets.Header")]

        [Label($"${path}.ILoveBalance.Label")]
        [Tooltip($"${path}.ILoveBalance.Tooltip")]
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
                NPCLiveInEvil is true &&
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
                NoBiomeSpread is false &&
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
                    NPCLiveInEvil = true;
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
                    NoBiomeSpread = false;
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

        [Label($"${path}.FukMeCalamity.Label")]
        [Tooltip($"${path}.FukMeCalamity.Tooltip")]
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
                NPCLiveInEvil is true &&
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
                NoBiomeSpread is true &&
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
                    NPCLiveInEvil = true;
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
                    NoBiomeSpread = true;
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

        #endregion

        [Header($"${path}.OtherFunctions.Header")]
        [Label($"${path}.OtherFunctions.Label")]
        public bool OtherFunctions => true;

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

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            if (OnlyHostByPassword)
            {
                if (!NetPasswordSystem.Registered[whoAmI])
                {
                    message = GetText("Config.OnlyHostByPassword.Unaccepted");
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

        public static bool TryAcceptChanges(int whoAmI, ref string message)
        {
            // DoesPlayerSlotCountAsAHost是preview的，stable还没有，又被坑了
            // if (MessageBuffer.DoesPlayerSlotCountAsAHost(whoAmI)) {
            if (Netplay.Clients[whoAmI].Socket.GetRemoteAddress().IsLocalHost())
            {
                return true;
            }
            else
            {
                message = GetText("Config.OnlyHost.Unaccepted");
                return false;
            }
        }
    }
}
