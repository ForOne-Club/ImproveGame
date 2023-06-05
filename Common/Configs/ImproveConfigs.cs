using ImproveGame.Common.ModSystems;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.Common.Configs
{
    public class ImproveConfigs : ModConfig
    {
        public const string QolConfigsPath = "Mods.ImproveGame.Configs.ImproveConfigs";
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public override void OnLoaded() => Config = this;

        #region 物品设置

        [Header($"${QolConfigsPath}.ItemConfigs.Header")]
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

        [DefaultValue(9999)]
        [Range(1, int.MaxValue)]
        public int ItemMaxStack;

        [DefaultValue(0d)]
        [Range(0, 1f)]
        [Slider]
        [Increment(0.125f)]
        public float ExtraToolSpeed;

        [DefaultValue(false)]
        public bool ModifyPlayerPlaceSpeed;

        [Slider]
        [Range(0, 20)]
        [Increment(2)]
        public int ModifyPlayerTileRange;

        public List<string> TileSpeed_Blacklist = new() { new("torch") };

        [DefaultValue(true)]
        public bool PortableCraftingStation;

        [DefaultValue(true)]
        public bool NoPlace_BUFFTile;

        [DefaultValue(true)]
        public bool NoPlace_BUFFTile_Banner;

        [DefaultValue(false)]
        public bool NoConsume_SummonItem;

        [DefaultValue(true)]
        public bool NoConsume_Potion;

        [DefaultValue(30)]
        [Range(10, 999)]
        public int NoConsume_PotionRequirement;

        [DefaultValue(true)]
        public bool NoConsume_Ammo;

        [DefaultValue(true)]
        public bool NoConsume_Projectile;

        [DefaultValue(false)]
        public bool HideNoConsumeBuffs;

        [DefaultValue(false)]
        public bool ImprovePrefix;

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
        public bool MiddleEnableBank;

        [DefaultValue(true)]
        public bool AutoSaveMoney;

        [DefaultValue(true)]
        public bool FasterExtractinator;

        #endregion

        #region NPC设置

        [Header($"${QolConfigsPath}.NPCConfigs.Header")]
        [DefaultValue(false)]
        public bool TownNPCGetTFIntoHouse;

        [DefaultValue(true)]
        public bool NPCLiveInEvil;

        [Slider]
        [Range(1, 10)]
        [DefaultValue(1)]
        public int TownNPCSpawnSpeed;

        [DefaultValue(false)]
        public bool NoCD_FishermanQuest;

        [Range(1, 25)]
        [DefaultValue(1)]
        [Slider]
        public int NPCCoinDropRate;

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

        [Header($"${QolConfigsPath}.GameMechanics.Header")]
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

        [DefaultValue(99)]
        [Range(0, 99)]
        [Slider]
        [Increment(11)]
        [ReloadRequired]
        public int ExtraPlayerBuffSlots;

        #endregion

        #region 树木设置

        [Header($"${QolConfigsPath}.TreeConfigs.Header")]
        [DefaultValue(true)]
        public bool TreeGrowFaster;

        [DefaultValue(false)]
        public bool ShakeTreeFruit;

        [DefaultValue(false)]
        public bool GemTreeAlwaysDropGem;

        [Range(1, 100)]
        [DefaultValue(5)]
        public int MostTreeMin;

        [Range(1, 100)]
        [DefaultValue(16)]
        public int MostTreeMax;

        [Range(1, 100)]
        [DefaultValue(10)]
        public int PalmTreeMin;

        [Range(1, 100)]
        [DefaultValue(20)]
        public int PalmTreeMax;

        [Range(1, 100)]
        [DefaultValue(7)]
        public int GemTreeMin;

        [Range(1, 100)]
        [DefaultValue(12)]
        public int GemTreeMax;

        #endregion

        #region 多人设置

        [Header($"${QolConfigsPath}.TogetherConfigs.Header")]
        [DefaultValue(true)]
        public bool ShareCraftingStation;

        [DefaultValue(false)]
        public bool ShareInfBuffs;

        [DefaultValue(-1)]
        [Range(-1, 2000)]
        public int ShareRange;

        [DefaultValue(false)]
        public bool TeamAutoJoin;

        #endregion

        #region 模组设置

        [Header($"${QolConfigsPath}.ServerSettings.Header")]
        [DefaultValue(false)]
        public bool OnlyHost;

        [DefaultValue(false)]
        [ReloadRequired]
        public bool OnlyHostByPassword;

        [DefaultValue(50)]
        [Range(1, 100)]
        public int SpawnRateMaxValue;

        [DefaultValue(true)]
        public bool ShowModName;

        [DefaultValue(true)]
        public bool EmptyAutofisher;

        // 
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

            public override bool Equals(object obj)
            {
                if (obj is ModItemLoadPage other)
                    return MagickWand == other.MagickWand && SpaceWand == other.SpaceWand && StarburstWand == other.StarburstWand &&
                           WallPlace == other.WallPlace && CreateWand == other.CreateWand && LiquidWand == other.LiquidWand &&
                           PotionBag == other.PotionBag && BannerChest == other.BannerChest && Autofisher == other.Autofisher &&
                           PaintWand == other.PaintWand && ConstructWand == other.ConstructWand && MoveChest == other.MoveChest &&
                           CoinOne == other.CoinOne && ExtremeStorage == other.ExtremeStorage;
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return new
                {
                    MagickWand, SpaceWand, StarburstWand, WallPlace, CreateWand, LiquidWand, PotionBag,
                    BannerChest, Autofisher, PaintWand, ConstructWand, MoveChest, CoinOne, ExtremeStorage
                }.GetHashCode();
            }
        }

        #endregion

        #region 预设

        [Header($"${QolConfigsPath}.Presets.Header")]
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

        [CustomModConfigItem(typeof(NonConfigurableFunctionsElement))]
        public bool OtherFunctions => true;

        #endregion

        class NonConfigurableFunctionsElement : ConfigElement
        {
            public override void OnBind()
            {
                base.OnBind();

                UIText uiText = new(Language.GetText($"{QolConfigsPath}.OtherFunctions.Header"), 0.8f)
                {
                    HAlign = 0f,
                    VAlign = 0f,
                    TextOriginX = 0f,
                    IsWrapped = true
                };

                uiText.SetSize(0f, 0f, 1f, 1f);
                uiText.SetPos(6f, 32f);

                uiText.OnInternalTextChange += () =>
                {
                    Height = new(uiText.MinHeight.Pixels + 14, 0f);
                };

                Append(uiText);
            }
        }

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
