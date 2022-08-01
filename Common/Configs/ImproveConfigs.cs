using ImproveGame.Common.Systems;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs
{
    [Label("$Mods.ImproveGame.Config.ImproveConfigs.Label")]
    public class ImproveConfigs : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public override void OnLoaded() => MyUtils.Config = this;

        // 物品设置
        [Header("$Mods.ImproveGame.Config.ItemConfigs.Header")]

        [Label("$Mods.ImproveGame.Config.LoadModItems.Label")]
        [Tooltip("$Mods.ImproveGame.Config.LoadModItems.Tooltip")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool LoadModItems;

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

        [Label("$Mods.ImproveGame.Config.TownNPCSpawnInNight.Label")]
        [DefaultValue(false)]
        public bool TownNPCSpawnInNight;

        [Label("$Mods.ImproveGame.Config.TownNPCSpawnSpeed.Label")]
        [Tooltip("$Mods.ImproveGame.Config.TownNPCSpawnSpeed.Tooltip")]
        [Slider]
        [Range(-1, 12)]
        [DefaultValue(-1)]
        public int TownNPCSpawnSpeed;

        [Label("$Mods.ImproveGame.Config.NoCD_FishermanQuest.Label")]
        [DefaultValue(false)]
        public bool NoCD_FishermanQuest;

        [Label("$Mods.ImproveGame.Config.NPCCoinDropRate.Label")]
        [Range(1, 25)]
        [DefaultValue(1)]
        [Slider]
        public int NPCCoinDropRate;

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

        [Label("$Mods.ImproveGame.Config.BestiaryQuickUnlock.Label")]
        [Tooltip("$Mods.ImproveGame.Config.BestiaryQuickUnlock.Tooltip")]
        [DefaultValue(true)]
        public bool BestiaryQuickUnlock;

        [Label("$Mods.ImproveGame.Config.RespawnWithFullHP.Label")]
        [DefaultValue(true)]
        public bool RespawnWithFullHP;

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

        [Header("$Mods.ImproveGame.Config.ModSettings.Header")]

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
                    message = MyUtils.GetText("Config.OnlyHostByPassword.Unaccepted");
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
                message = MyUtils.GetText("Config.OnlyHost.Unaccepted");
                return false;
            }
        }
    }
}
