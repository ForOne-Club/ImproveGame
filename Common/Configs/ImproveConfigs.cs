using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs
{
    [Label("$Mods.ImproveGame.Config.ImproveConfigs.Label")]
    public class ImproveConfigs : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

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
        [ReloadRequired]
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
        [DefaultValue(false)]
        public bool ImproveToolSpeed;

        [Label("$Mods.ImproveGame.Config.TileSpeedAndTileRange.Label")]
        [DefaultValue(false)]
        public bool ImproveTileSpeedAndTileRange;

        [Label("$Mods.ImproveGame.Config.NoPlace_BUFFTile.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoPlace_BUFFTile.Tooltip")]
        [DefaultValue(false)]
        public bool NoPlace_BUFFTile;

        [Label("$Mods.ImproveGame.Config.NoPlace_BUFFTile_Banner.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoPlace_BUFFTile_Banner.Tooltip")]
        [DefaultValue(false)]
        public bool NoPlace_BUFFTile_Banner;

        [Label("$Mods.ImproveGame.Config.NoConsume_SummonItem.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoConsume_SummonItem.Tooltip")]
        [DefaultValue(false)]
        public bool NoConsume_SummonItem;

        [Label("$Mods.ImproveGame.Config.NoConsume_Potion.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoConsume_Potion.Tooltip")]
        [DefaultValue(false)]
        public bool NoConsume_Potion;

        [Label("$Mods.ImproveGame.Config.NoConsume_Ammo.Label")]
        [Tooltip("$Mods.ImproveGame.Config.NoConsume_Ammo.Tooltip")]
        [DefaultValue(false)]
        public bool NoConsume_Ammo;

        [Label("$Mods.ImproveGame.Config.HideNoConsumeBuffs.Label")]
        [Tooltip("$Mods.ImproveGame.Config.HideNoConsumeBuffs.Tooltip")]
        [DefaultValue(true)]
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

        [Header("$Mods.ImproveGame.Config.NPCConfigs.Header")]

        [Label("$Mods.ImproveGame.Config.TownNPCSpawnInNight.Label")]
        [DefaultValue(false)]
        public bool TownNPCSpawnInNight;

        [Label("$Mods.ImproveGame.Config.TownNPCSpawnSpeed.Label")]
        [Slider]
        [Range(0, 12)]
        [DefaultValue(0)]
        public int TownNPCSpawnSpeed;

        [Label("$Mods.ImproveGame.Config.NoCD_FishermanQuest.Label")]
        [DefaultValue(false)]
        public bool NoCD_FishermanQuest;

        [Label("$Mods.ImproveGame.Config.NPCCoinDropRate.Label")]
        [Range(1, 25)]
        [DefaultValue(1)]
        [Slider]
        [ReloadRequired]
        public int NPCCoinDropRate;

        [Header("$Mods.ImproveGame.Config.GameMechanics.Header")]

        [Label("$Mods.ImproveGame.Config.TreeGrowFaster.Label")]
        [DefaultValue(true)]
        public bool TreeGrowFaster;

        [Label("$Mods.ImproveGame.Config.ShakeTreeFruit.Label")]
        [DefaultValue(false)]
        public bool ShakeTreeFruit;

        [Label("$Mods.ImproveGame.Config.BanDamageVar.Label")]
        [DefaultValue(false)]
        public bool BanDamageVar;

        [Label("$Mods.ImproveGame.Config.ExtraPlayerBuffSlots.Label")]
        [DefaultValue(0)]
        [Range(0, 99)]
        [Slider]
        [Increment(11)]
        public int ExtraPlayerBuffSlots;
    }
}
