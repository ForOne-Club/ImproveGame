using ImproveGame.Common.Players;
using ImproveGame.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.GlobalItems
{
    public class ApplyBuffItem : GlobalItem
    {
        internal static List<int> BuffTypesShouldHide = new();

        // 特殊药水
        public static readonly List<int> SpecialPotions = new() { 2350, 2351, ItemID.WormholePotion, ItemID.PotionOfReturn };
        // 增益 Tile 巴斯特雕像，篝火，红心灯笼，星星瓶，向日葵，弹药箱，施法桌，水晶球，蛋糕块，利器站，水蜡烛，和平蜡烛
        public static readonly List<List<int>> BUFFTiles = new() { new() { 506, 0, 215 }, new() { 215, 0, 87 }, new() { 42, 9, 89 }, new() { 42, 7, 158 }, new() { 27, 0, 146 }, new() { 287, 0, 93 }, new() { 354, 0, 150 }, new() { 125, 0, 29 }, new() { 621, 0, 192 }, new() { 377, 0, 159 }, new() { 49, 0, 86 }, new() { 372, 0, 157 } };

        public override void UpdateInventory(Item item, Player player) {
            int buffType = GetItemBuffType(item);
            if (buffType is not -1 && InfBuffPlayer.Get(player).CheckInfBuffEnable(buffType)) {
                // 饱食三级Buff不应该覆盖，而是取最高级
                bool wellFed3Enabled = player.FindBuffIndex(BuffID.WellFed3) != -1;
                if (buffType == BuffID.WellFed && (player.FindBuffIndex(BuffID.WellFed2) != -1 || wellFed3Enabled))
                        return;
                if (buffType == BuffID.WellFed2 && wellFed3Enabled)
                    return;

                player.AddBuff(buffType, 2);
            }
            // 我发现游戏暂停时，不会调用UpdateInventory，导致InventoryGlow没了，所以我调用放到了ImproveItem里面
            //UpdateInventoryGlow(item, player);
        }

        public static void UpdateInventoryGlow(Item item) {
            int buffType = GetItemBuffType(item);
            if (buffType is not -1) {
                BuffTypesShouldHide.Add(buffType);
                item.GetGlobalItem<GlobalItemData>().InventoryGlow = true;
            }
            // 非增益药剂
            if (MyUtils.Config.NoConsume_Potion && item.stack >= 30 && SpecialPotions.Contains(item.type)) {
                item.GetGlobalItem<GlobalItemData>().InventoryGlow = true;
            }
            // 随身增益站：旗帜
            if (MyUtils.Config.NoPlace_BUFFTile_Banner) {
                if (item.createTile == TileID.Banners) {
                    int style = item.placeStyle;
                    int frameX = style * 18;
                    int frameY = 0;
                    if (style >= 90) {
                        frameX -= 1620;
                        frameY += 54;
                    }
                    if (frameX >= 396 || frameY >= 54) {
                        item.GetGlobalItem<GlobalItemData>().InventoryGlow = true;
                    }
                }
            }
            // 弹药
            if (MyUtils.Config.NoConsume_Ammo && item.stack >= 3996 && item.ammo > 0) {
                item.GetGlobalItem<GlobalItemData>().InventoryGlow = true;
            }
        }

        public static int GetItemBuffType(Item item) {
            if (MyUtils.Config.NoConsume_Potion) {
                // 普通药水
                if (item.stack >= 30 && item.active) {
                    if (item.buffType > 0)
                        return item.buffType;
                    // 其他Mod的，自行添加了引用
                    if (ModIntegrationsSystem.ModdedPotionBuffs.ContainsKey(item.type))
                        return ModIntegrationsSystem.ModdedPotionBuffs[item.type];
                }
            }
            // 随身增益站：普通
            if (MyUtils.Config.NoPlace_BUFFTile) {
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
            for (int i = 0; i < BUFFTiles.Count; i++) {
                if (item.createTile == BUFFTiles[i][0] && item.placeStyle == BUFFTiles[i][1]) {
                    buffType = BUFFTiles[i][2];
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
            if (MyUtils.Config.NoConsume_Potion && item.stack >= 30 && (item.buffType > 0 || SpecialPotions.Contains(item.type))) {
                return false;
            }
            return base.ConsumeItem(item, player);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            if (!item.TryGetGlobalItem<GlobalItemData>(out var global) || !global.InventoryGlow)
                return;

            if (IsBuffTileItem(item, out _) || item.type == ItemID.HoneyBucket ||
                (item.stack >= 30 && item.buffType > 0 && item.active)) {
                int buffType = GetItemBuffType(item);

                if (buffType is -1) return;

                if (Main.mouseMiddle && Main.mouseMiddleRelease) {
                    UISystem.Instance.BuffTrackerGUI.Open();
                }

                if (!InfBuffPlayer.Get(Main.LocalPlayer).CheckInfBuffEnable(buffType)) {
                    tooltips.Add(new(Mod, "BuffDisabled", MyUtils.GetTextWith("Tips.BuffDisabled", new { BuffName = Lang.GetBuffName(buffType) })) {
                        OverrideColor = Color.SkyBlue
                    });
                    return;
                }
                else {
                    tooltips.Add(new(Mod, "BuffEnabled", MyUtils.GetText("Tips.BuffEnabled")) {
                        OverrideColor = Color.SkyBlue
                    });
                }

                tooltips.Add(new(Mod, "BuffApplied", MyUtils.GetTextWith("Tips.BuffApplied", new { BuffName = Lang.GetBuffName(buffType) })) {
                    OverrideColor = Color.LightGreen
                });

                if (MyUtils.Config.HideNoConsumeBuffs) {
                    tooltips.Add(new(Mod, "BuffHided", MyUtils.GetText("Tips.BuffHided")) {
                        OverrideColor = Color.LightGreen
                    });
                }
            }
        }
    }
}
