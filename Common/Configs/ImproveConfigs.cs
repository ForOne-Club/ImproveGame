using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs
{
    [Label("$Mods.ImproveGame.ImproveConfigs.ImproveConfigs_Label")]
    public class ImproveConfigs : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("$Mods.ImproveGame.ImproveConfigs.LoadModItems_Header")]
        [Label("$Mods.ImproveGame.ImproveConfigs.LoadModItems_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.LoadModItems_Tooltip")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool LoadModItems;

        [Label("$Mods.ImproveGame.ImproveConfigs.SuperVoidVault_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.SuperVoidVault_Tooltip")]
        [DefaultValue(false)]
        public bool SuperVoidVault;

        [Label("$Mods.ImproveGame.ImproveConfigs.SmartVoidVault_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.SmartVoidVault_Tooltip")]
        [DefaultValue(false)]
        public bool SmartVoidVault;

        [Label("$Mods.ImproveGame.ImproveConfigs.SuperVault_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.SuperVault_Tooltip")]
        [DefaultValue(false)]
        public bool SuperVault;

        [Label("$Mods.ImproveGame.ImproveConfigs.NoPlace_BUFFTile_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.NoPlace_BUFFTile_Tooltip")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool NoPlace_BUFFTile;

        [Label("$Mods.ImproveGame.ImproveConfigs.NoPlace_BUFFTile_Banner_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.NoPlace_BUFFTile_Banner_Tooltip")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool NoPlace_BUFFTile_Banner;

        [Label("$Mods.ImproveGame.ImproveConfigs.ItemMaxStack_Label")]
        [DefaultValue(9999)]
        [Range(1, int.MaxValue)]
        [ReloadRequired]
        public int ItemMaxStack;

        [Label("$Mods.ImproveGame.ImproveConfigs.GrabDistance_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.GrabDistance_Tooltip")]
        [DefaultValue(0)]
        [Slider]
        [Range(0, 25)]
        [ReloadRequired]
        public int GrabDistance;

        [Label("$Mods.ImproveGame.ImproveConfigs.NoConsume_SummonItem_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.NoConsume_SummonItem_Tooltip")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool NoConsume_SummonItem;

        [Label("$Mods.ImproveGame.ImproveConfigs.AutoReuseWeapon_Label")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool AutoReuseWeapon;

        [Label("$Mods.ImproveGame.ImproveConfigs.AutoReuseWeapon_ExclusionList_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.AutoReuseWeapon_ExclusionList_Tooltip")]
        [ReloadRequired]
        public List<ItemDefinition> AutoReuseWeapon_ExclusionList =
            new List<ItemDefinition> { new(218), new(113), new(495) };

        [Label("$Mods.ImproveGame.ImproveConfigs.ImproveToolSpeed_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.ImproveToolSpeed_Tooltip")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool ImproveToolSpeed;

        [Label("$Mods.ImproveGame.ImproveConfigs.TileSpeedAndTileRange_Label")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool ImproveTileSpeedAndTileRange;

        [Label("$Mods.ImproveGame.ImproveConfigs.NoConsume_Potion_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.NoConsume_Potion_Tooltip")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool NoConsume_Potion;

        [Label("$Mods.ImproveGame.ImproveConfigs.NoConsume_Ammo_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.NoConsume_Ammo_Tooltip")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool NoConsume_Ammo;

        [Label("$Mods.ImproveGame.ImproveConfigs.ImprovePrefix_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.ImprovePrefix_Tooltip")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool ImprovePrefix;

        [Label("$Mods.ImproveGame.ImproveConfigs.ShowPrefixCount_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.NoNeedReload")]
        [DefaultValue(true)]
        public bool ShowPrefixCount;

        [Label("$Mods.ImproveGame.ImproveConfigs.ShowItemMoreData_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.NoNeedReload")]
        [DefaultValue(false)]
        public bool ShowItemMoreData;

        [Label("$Mods.ImproveGame.ImproveConfigs.BanTombstone_Label")]
        [Tooltip("$Mods.ImproveGame.ImproveConfigs.NoNeedReload")]
        [DefaultValue(false)]
        public bool BanTombstone;

        [Header("$Mods.ImproveGame.ImproveConfigs.TownNPCSpawnInNight_Header")]
        [Label("$Mods.ImproveGame.ImproveConfigs.TownNPCSpawnInNight_Label")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool TownNPCSpawnInNight;

        [Label("$Mods.ImproveGame.ImproveConfigs.TownNPCSpawnSpeed_Label")]
        [Slider]
        [Range(0, 12)]
        [DefaultValue(0)]
        [ReloadRequired]
        public int TownNPCSpawnSpeed;

        [Label("$Mods.ImproveGame.ImproveConfigs.NoCD_FishermanQuest_Label")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool NoCD_FishermanQuest;

        [Label("$Mods.ImproveGame.ImproveConfigs.NPCCoinDropRate_Label")]
        [Range(1, 25)]
        [DefaultValue(1)]
        [Slider]
        [ReloadRequired]
        public int NPCCoinDropRate;

        [Header("$Mods.ImproveGame.ImproveConfigs.BanDamageVar_Header")]
        [Label("$Mods.ImproveGame.ImproveConfigs.BanDamageVar_Label")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool BanDamageVar;
    }
}
