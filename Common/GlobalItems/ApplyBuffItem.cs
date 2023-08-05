using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Patches;
using ImproveGame.Core;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using Microsoft.Xna.Framework.Input;
using System.Collections.ObjectModel;
using Terraria.GameInput;

namespace ImproveGame.Common.GlobalItems
{
    public class ApplyBuffItem : GlobalItem
    {
        public static void UpdateInventoryGlow(Item item) {
            bool globalItemNotNull = item.TryGetGlobalItem<GlobalItemData>(out var globalItem);
            if (globalItemNotNull)
                globalItem.InventoryGlow = false;

            int buffType = GetItemBuffType(item);
            if (buffType is not -1) {
                HideBuffSystem.BuffTypesShouldHide[buffType] = true;
                if (globalItemNotNull)
                    globalItem.InventoryGlow = true;
            }

            // 非增益药剂
            if (Config.NoConsume_Potion && item.stack >= Config.NoConsume_PotionRequirement && Lookups.SpecialPotions.Contains(item.type) && globalItemNotNull)
                globalItem.InventoryGlow = true;

            // 随身增益站：旗帜
            if (Config.NoPlace_BUFFTile_Banner && globalItemNotNull && globalItem.ShouldHaveInvGlowForBanner)
                globalItem.InventoryGlow = true;

            // 弹药
            if (Config.NoConsume_Ammo && item.stack >= 3996 && item.ammo > 0 && globalItemNotNull)
                globalItem.InventoryGlow = true;

            // 花园侏儒
            if (item.type == ItemID.GardenGnome && globalItemNotNull)
                globalItem.InventoryGlow = true;
        }

        public static int GetItemBuffType(Item item) {
            if (ModIntegrationsSystem.ModdedInfBuffsIgnore.Contains(item.type))
                return -1;
            if (Config.NoConsume_Potion) {
                // 普通药水
                if (item.stack >= Config.NoConsume_PotionRequirement && item.active) {
                    if (item.buffType > 0)
                        return item.buffType;
                    // 其他Mod的，自行添加了引用
                    if (ModIntegrationsSystem.ModdedPotionBuffs.ContainsKey(item.type))
                        return ModIntegrationsSystem.ModdedPotionBuffs[item.type];
                }
            }
            // 随身增益站：普通
            if (Config.NoPlace_BUFFTile) {
                IsBuffTileItem(item, out int buffType);
                if (buffType is not -1)
                    return buffType;

                if (item.type == ItemID.HoneyBucket) {
                    return BuffID.Honey;
                }
            }
            return -1;
        }

        public static bool IsBuffTileItem(Item item, out int buffType) {
            // 会给玩家buff的雕像
            for (int i = 0; i < Lookups.BuffTiles.Count; i++) {
                if (item.createTile == Lookups.BuffTiles[i].TileID && (item.placeStyle == Lookups.BuffTiles[i].Style || Lookups.BuffTiles[i].Style == -1)) {
                    buffType = Lookups.BuffTiles[i].BuffID;
                    return true;
                }
            }
            // 其他Mod的，自行添加了引用
            foreach (var moddedBuff in ModIntegrationsSystem.ModdedPlaceableItemBuffs) {
                if (item.type == moddedBuff.Key) {
                    buffType = moddedBuff.Value;
                    return true;
                }
            }
            buffType = -1;
            return false;
        }

        // 物品消耗
        public override bool ConsumeItem(Item item, Player player) {
            if (item.ModItem?.Mod.Name is "Everglow") return base.ConsumeItem(item, player);
            
            if (Config.NoConsume_Potion && item.stack >= Config.NoConsume_PotionRequirement && (item.buffType > 0 || Lookups.SpecialPotions.Contains(item.type))) {
                return false;
            }
            return base.ConsumeItem(item, player);
        }

        private static bool _oldMiddlePressed;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            if (!item.TryGetGlobalItem<GlobalItemData>(out var global) || !global.InventoryGlow)
                return;

            if (IsBuffTileItem(item, out _) || item.type == ItemID.HoneyBucket || item.type == ItemID.GardenGnome ||
                (item.stack >= Config.NoConsume_PotionRequirement && item.buffType > 0 && item.active)) {
                int buffType = GetItemBuffType(item);

                if (buffType is -1 && item.type != ItemID.GardenGnome) return;

                MouseState mouseState = Mouse.GetState();
                if (_oldMiddlePressed)
                {
                    _oldMiddlePressed = mouseState.MiddleButton == ButtonState.Pressed;
                }

                if (mouseState.MiddleButton == ButtonState.Pressed && !_oldMiddlePressed)
                {
                    _oldMiddlePressed = true;
                    if (BuffTrackerGUI.Visible)
                        UISystem.Instance.BuffTrackerGUI.Close();
                    else
                        UISystem.Instance.BuffTrackerGUI.Open();
                }

                TagItem.ModifyBuffTooltips(Mod, item.type, buffType, tooltips);
            }
        }

        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y) {
            if (!item.TryGetGlobalItem<GlobalItemData>(out var global) || !global.InventoryGlow)
                return base.PreDrawTooltip(item, lines, ref x, ref y);

            if (item.type == ItemID.GardenGnome && ItemSlot.ShiftInUse)
            {
                TagItem.DrawTagTooltips(lines, TagItem.GenerateDetailedTags(Mod, lines), x, y);
                return base.PreDrawTooltip(item, lines, ref x, ref y);
            }

            if (IsBuffTileItem(item, out _) || item.type == ItemID.HoneyBucket ||
                (item.stack >= Config.NoConsume_PotionRequirement && item.buffType > 0 && item.active)) {
                int buffType = GetItemBuffType(item);

                if (buffType is -1)
                    return base.PreDrawTooltip(item, lines, ref x, ref y);

                object arg = new {
                    BuffName = Lang.GetBuffName(buffType),
                    MaxSpawn = Config.SpawnRateMaxValue
                };
                if (ItemSlot.ShiftInUse)
                    TagItem.DrawTagTooltips(lines, TagItem.GenerateDetailedTags(Mod, lines, arg), x, y);
            }

            return base.PreDrawTooltip(item, lines, ref x, ref y);
        }
    }
}
