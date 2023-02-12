using ImproveGame.Common.Systems;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs
{
    [Label($"${_path}.ImproveConfigs.Label")]
    public class ImproveConfigs : ModConfig
    {
        const string _path = "Mods.ImproveGame.Config";
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public override void OnLoaded() => Config = this;

        #region 物品设置

        [Header($"${_path}.ItemConfigs.Header")]

        [Label($"${_path}.SuperVoidVault.Label")]
        [Tooltip($"${_path}.SuperVoidVault.Tooltip")]
        [DefaultValue(false)]
        public bool SuperVoidVault;

        [Label($"${_path}.SmartVoidVault.Label")]
        [Tooltip($"${_path}.SmartVoidVault.Tooltip")]
        [DefaultValue(false)]
        public bool SmartVoidVault;

        [Label($"${_path}.SuperVault.Label")]
        [Tooltip($"${_path}.SuperVault.Tooltip")]
        [DefaultValue(false)]
        public bool SuperVault;

        [Label($"${_path}.GrabDistance.Label")]
        [Tooltip($"${_path}.GrabDistance.Tooltip")]
        [DefaultValue(0)]
        [Slider]
        [Range(0, 75)]
        public int GrabDistance;

        [Label($"${_path}.ItemMaxStack.Label")]
        [DefaultValue(9999)]
        [Range(1, int.MaxValue)]
        public int ItemMaxStack;

        [Label($"${_path}.AutoReuseWeapon.Label")]
        [DefaultValue(true)]
        public bool AutoReuseWeapon;

        [Label($"${_path}.AutoReuseWeapon_ExclusionList.Label")]
        [Tooltip($"${_path}.AutoReuseWeapon_ExclusionList.Tooltip")]
        public List<ItemDefinition> AutoReuseWeaponExclusionList = new() { new(218), new(113), new(495) };

        [Label($"${_path}.ImproveToolSpeed.Label")]
        [Tooltip($"${_path}.ImproveToolSpeed.Tooltip")]
        [DefaultValue(0d)]
        [Range(0, 1f)]
        [Slider]
        [Increment(0.125f)]
        public float ExtraToolSpeed;

        [Label($"${_path}.ModifyPlayerPlaceSpeed.Label")]
        [DefaultValue(false)]
        public bool ModifyPlayerPlaceSpeed;

        [Label($"${_path}.ModifyPlayerTileRangle.Label")]
        [Slider]
        [Range(0, 20)]
        [Increment(2)]
        public int ModifyPlayerTileRangle;

        [Label($"${_path}.TileSpeed_Blacklist.Label")]
        [Tooltip($"${_path}.TileSpeed_Blacklist.Tooltip")]
        public List<string> TileSpeed_Blacklist = new() { new("torch") };

        [Label($"${_path}.PortableCraftingStation.Label")]
        [Tooltip($"${_path}.PortableCraftingStation.Tooltip")]
        [DefaultValue(true)]
        public bool PortableCraftingStation;

        [Label($"${_path}.NoPlace_BUFFTile.Label")]
        [Tooltip($"${_path}.NoPlace_BUFFTile.Tooltip")]
        [DefaultValue(true)]
        public bool NoPlace_BUFFTile;

        [Label($"${_path}.NoPlace_BUFFTile_Banner.Label")]
        [Tooltip($"${_path}.NoPlace_BUFFTile_Banner.Tooltip")]
        [DefaultValue(true)]
        public bool NoPlace_BUFFTile_Banner;

        [Label($"${_path}.NoConsume_SummonItem.Label")]
        [Tooltip($"${_path}.NoConsume_SummonItem.Tooltip")]
        [DefaultValue(false)]
        public bool NoConsume_SummonItem;

        [Label($"${_path}.NoConsume_Potion.Label")]
        [Tooltip($"${_path}.NoConsume_Potion.Tooltip")]
        [DefaultValue(true)]
        public bool NoConsume_Potion;

        [Label($"${_path}.NoConsume_PotionRequirement.Label")]
        [Tooltip($"${_path}.NoConsume_PotionRequirement.Tooltip")]
        [DefaultValue(30)]
        [Range(10, 999)]
        public int NoConsume_PotionRequirement;

        [Label($"${_path}.NoConsume_Ammo.Label")]
        [Tooltip($"${_path}.NoConsume_Ammo.Tooltip")]
        [DefaultValue(true)]
        public bool NoConsume_Ammo;

        [Label($"${_path}.NoConsume_Projectile.Label")]
        [Tooltip($"${_path}.NoConsume_Projectile.Tooltip")]
        [DefaultValue(true)]
        public bool NoConsume_Projectile;

        [Label($"${_path}.HideNoConsumeBuffs.Label")]
        [Tooltip($"${_path}.HideNoConsumeBuffs.Tooltip")]
        [DefaultValue(false)]
        public bool HideNoConsumeBuffs;

        [Label($"${_path}.ImprovePrefix.Label")]
        [Tooltip($"${_path}.ImprovePrefix.Tooltip")]
        [DefaultValue(false)]
        public bool ImprovePrefix;

        [Label($"${_path}.ResurrectionTimeShortened.Label")]
        [Tooltip($"${_path}.ResurrectionTimeShortened.Tooltip")]
        [DefaultValue(0)]
        [Range(0, 100)]
        [Increment(5)]
        [Slider]
        public int ResurrectionTimeShortened;

        [Label($"${_path}.BOSSBattleResurrectionTimeShortened.Label")]
        [DefaultValue(0)]
        [Range(0, 100)]
        [Increment(5)]
        [Slider]
        public int BOSSBattleResurrectionTimeShortened;

        [Label($"${_path}.BanTombstone.Label")]
        [Tooltip($"${_path}.NoNeedReload")]
        [DefaultValue(false)]
        public bool BanTombstone;

        [Label($"${_path}.MiddleEnableBank.Label")]
        [Tooltip($"${_path}.MiddleEnableBank.Tooltip")]
        [DefaultValue(true)]
        public bool MiddleEnableBank;

        [Label($"${_path}.AutoSaveMoney.Label")]
        [Tooltip($"${_path}.AutoSaveMoney.Tooltip")]
        [DefaultValue(true)]
        public bool AutoSaveMoney;

        [Label($"${_path}.FasterExtractinator.Label")]
        [Tooltip($"${_path}.FasterExtractinator.Tooltip")]
        [DefaultValue(true)]
        public bool FasterExtractinator;

        #endregion

        #region NPC设置

        [Header($"${_path}.NPCConfigs.Header")]

        [Label($"${_path}.TownNPCGetTFIntoHouse.Label")]
        [Tooltip($"${_path}.TownNPCGetTFIntoHouse.Tooltip")]
        [DefaultValue(false)]
        public bool TownNPCGetTFIntoHouse;

        [Label($"${_path}.NPCLiveInEvil.Label")]
        [Tooltip($"${_path}.NPCLiveInEvil.Tooltip")]
        [DefaultValue(true)]
        public bool NPCLiveInEvil;

        [Label($"${_path}.TownNPCSpawnSpeed.Label")]
        [Tooltip($"${_path}.TownNPCSpawnSpeed.Tooltip")]
        [Slider]
        [Range(0, 12)]
        [DefaultValue(0)]
        public int TownNPCSpawnSpeed;

        [Label($"${_path}.NoCD_FishermanQuest.Label")]
        [DefaultValue(false)]
        public bool NoCD_FishermanQuest;

        [Label($"${_path}.NPCCoinDropRate.Label")]
        [Range(1, 25)]
        [DefaultValue(1)]
        [Slider]
        public int NPCCoinDropRate;

        [Label($"${_path}.LavalessLavaSlime.Label")]
        [Tooltip($"${_path}.LavalessLavaSlime.Tooltip")]
        [DefaultValue(false)]
        public bool LavalessLavaSlime;

        [Label($"${_path}.TravellingMerchantStay.Label")]
        [Tooltip($"${_path}.TravellingMerchantStay.Tooltip")]
        [DefaultValue(false)]
        public bool TravellingMerchantStay;

        [Label($"${_path}.TravellingMerchantRefresh.Label")]
        [Tooltip($"${_path}.TravellingMerchantRefresh.Tooltip")]
        [DefaultValue(true)]
        public bool TravellingMerchantRefresh;

        [Label($"${_path}.QuickNurse.Label")]
        [Tooltip($"${_path}.QuickNurse.Tooltip")]
        [DefaultValue(true)]
        public bool QuickNurse;

        [Label($"${_path}.BestiaryQuickUnlock.Label")]
        [Tooltip($"${_path}.BestiaryQuickUnlock.Tooltip")]
        [DefaultValue(true)]
        public bool BestiaryQuickUnlock;

        [Label($"${_path}.BannerRequirement.Label")]
        [Tooltip($"${_path}.BannerRequirement.Tooltip")]
        [Slider]
        [Range(0.1f, 10f)]
        [Increment(0.05f)]
        [DefaultValue(1f)]
        public float BannerRequirement;

        #endregion

        #region 游戏机制

        [Header($"${_path}.GameMechanics.Header")]

        [Label($"${_path}.AlchemyGrassGrowsFaster.Label")]
        [Tooltip($"${_path}.AlchemyGrassGrowsFaster.Tooltip")]
        [DefaultValue(false)]
        public bool AlchemyGrassGrowsFaster;

        [Label($"${_path}.AlchemyGrassAlwaysBlooms.Label")]
        [Tooltip($"${_path}.AlchemyGrassAlwaysBlooms.Tooltip")]
        [DefaultValue(false)]
        public bool AlchemyGrassAlwaysBlooms;

        [Label($"${_path}.StaffOfRegenerationAutomaticPlanting.Label")]
        [Tooltip($"${_path}.StaffOfRegenerationAutomaticPlanting.Tooltip")]
        [DefaultValue(false)]
        public bool StaffOfRegenerationAutomaticPlanting;

        [Label($"${_path}.NoBiomeSpread.Label")]
        [DefaultValue(true)]
        public bool NoBiomeSpread;

        [Label($"${_path}.RespawnWithFullHP.Label")]
        [DefaultValue(true)]
        public bool RespawnWithFullHP;

        [Label($"${_path}.DontDeleteBuff.Label")]
        [Tooltip($"${_path}.DontDeleteBuff.Tooltip")]
        [DefaultValue(true)]
        public bool DontDeleteBuff;

        [Label($"${_path}.JourneyResearch.Label")]
        [Tooltip($"${_path}.JourneyResearch.Tooltip")]
        [DefaultValue(false)]
        public bool JourneyResearch;

        [Label($"${_path}.BanDamageVar.Label")]
        [DefaultValue(false)]
        public bool BanDamageVar;

        [Label($"${_path}.ExtraPlayerBuffSlots.Label")]
        [DefaultValue(99)]
        [Range(0, 99)]
        [Slider]
        [Increment(11)]
        [ReloadRequired]
        public int ExtraPlayerBuffSlots;

        #endregion

        #region 树木设置

        [Header($"${_path}.TreeConfigs.Header")]

        [Label($"${_path}.TreeGrowFaster.Label")]
        [Tooltip($"${_path}.TreeGrowFaster.Tooltip")]
        [DefaultValue(true)]
        public bool TreeGrowFaster;

        [Label($"${_path}.ShakeTreeFruit.Label")]
        [Tooltip($"${_path}.ShakeTreeFruit.Tooltip")]
        [DefaultValue(false)]
        public bool ShakeTreeFruit;

        [Label($"${_path}.GemTreeAlwaysDropGem.Label")]
        [Tooltip($"${_path}.GemTreeAlwaysDropGem.Tooltip")]
        [DefaultValue(false)]
        public bool GemTreeAlwaysDropGem;

        [Range(1, 100)]
        [DefaultValue(5)]
        [Label($"${_path}.MostTree.LabelMin")]
        public int MostTreeMin;

        [Range(1, 100)]
        [DefaultValue(16)]
        [Label($"${_path}.MostTree.LabelMax")]
        public int MostTreeMax;

        [Range(1, 100)]
        [DefaultValue(10)]
        [Label($"${_path}.PalmTree.LabelMin")]
        public int PalmTreeMin;

        [Range(1, 100)]
        [DefaultValue(20)]
        [Label($"${_path}.PalmTree.LabelMax")]
        public int PalmTreeMax;

        [Range(1, 100)]
        [DefaultValue(7)]
        [Label($"${_path}.GemTree.LabelMin")]
        public int GemTreeMin;

        [Range(1, 100)]
        [DefaultValue(12)]
        [Label($"${_path}.GemTree.LabelMax")]
        public int GemTreeMax;

        #endregion

        #region 多人设置

        [Header($"${_path}.TogetherConfigs.Header")]

        [Label($"${_path}.ShareCraftingStation.Label")]
        [Tooltip($"${_path}.ShareCraftingStation.Tooltip")]
        [DefaultValue(true)]
        public bool ShareCraftingStation;

        [Label($"${_path}.ShareInfBuffs.Label")]
        [Tooltip($"${_path}.ShareInfBuffs.Tooltip")]
        [DefaultValue(false)]
        public bool ShareInfBuffs;

        [Label($"${_path}.ShareRange.Label")]
        [Tooltip($"${_path}.ShareRange.Tooltip")]
        [DefaultValue(-1)]
        [Range(-1, 2000)]
        public int ShareRange;

        [Label($"${_path}.TeamAutoJoin.Label")]
        [Tooltip($"${_path}.TeamAutoJoin.Tooltip")]
        [DefaultValue(false)]
        public bool TeamAutoJoin;

        #endregion

        #region 模组设置

        [Header($"${_path}.ServerSettings.Header")]

        [Label($"${_path}.OnlyHost.Label")]
        [Tooltip($"${_path}.OnlyHost.Tooltip")]
        [DefaultValue(false)]
        public bool OnlyHost;

        [Label($"${_path}.OnlyHostByPassword.Label")]
        [Tooltip($"${_path}.OnlyHostByPassword.Tooltip")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool OnlyHostByPassword;

        [Label($"${_path}.SpawnRateMaxValue.Label")]
        [Tooltip($"${_path}.SpawnRateMaxValue.Tooltip")]
        [DefaultValue(50)]
        [Range(1, 100)]
        public int SpawnRateMaxValue;

        [Label($"${_path}.ShowModName.Label")]
        [DefaultValue(true)]
        public bool ShowModName;

        [Label($"${_path}.EmptyAutofisher.Label")]
        [Tooltip($"${_path}.EmptyAutofisher.Tooltip")]
        [DefaultValue(true)]
        public bool EmptyAutofisher;

        [Label($"${_path}.LoadModItems.Label")]
        [ReloadRequired]
        public ModItemLoadPage LoadModItems = new();

        [SeparatePage]
        public class ModItemLoadPage
        {
            [Label("$Mods.ImproveGame.ItemName.MagickWand")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool MagickWand = true;

            [Label("$Mods.ImproveGame.ItemName.SpaceWand")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool SpaceWand = true;

            [Label("$Mods.ImproveGame.ItemName.StarburstWand")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool StarburstWand = true;

            [Label("$Mods.ImproveGame.ItemName.WallPlace")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool WallPlace = true;

            [Label("$Mods.ImproveGame.ItemName.CreateWand")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool CreateWand = true;

            [Label("$Mods.ImproveGame.ItemName.LiquidWand")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool LiquidWand = true;

            [Label("$Mods.ImproveGame.ItemName.PotionBag")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool PotionBag = true;

            [Label("$Mods.ImproveGame.ItemName.BannerChest")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool BannerChest = true;

            [Label("$Mods.ImproveGame.ItemName.Autofisher")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool Autofisher = true;

            [Label("$Mods.ImproveGame.ItemName.PaintWand")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool PaintWand = true;

            [Label("$Mods.ImproveGame.ItemName.ConstructWand")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool ConstructWand = true;

            [Label("$Mods.ImproveGame.ItemName.MoveChest")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
            [DefaultValue(true)]
            public bool MoveChest = true;

            [Label("$Mods.ImproveGame.ItemName.CoinOne")]
            [Tooltip($"${_path}.LoadModItems.Tooltip")]
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

        [Header($"${_path}.Presets.Header")]

        [Label($"${_path}.ILoveBalance.Label")]
        [Tooltip($"${_path}.ILoveBalance.Tooltip")]
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
                ModifyPlayerPlaceSpeed is true &&
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
                    ModifyPlayerPlaceSpeed = true;
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

        [Label($"${_path}.FukMeCalamity.Label")]
        [Tooltip($"${_path}.FukMeCalamity.Tooltip")]
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
                ModifyPlayerPlaceSpeed is true &&
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
                    ModifyPlayerPlaceSpeed = true;
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

        [Header($"${_path}.OtherFunctions.Header")]
        [Label($"${_path}.OtherFunctions.Label")]
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
