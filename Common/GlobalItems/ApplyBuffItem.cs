using ImproveGame.Common.Systems;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using System.Collections.ObjectModel;
using Terraria.GameInput;

namespace ImproveGame.Common.GlobalItems
{
    public class ApplyBuffItem : GlobalItem
    {
        // 特殊药水
        public static readonly List<int> SpecialPotions = new() {
            ItemID.RecallPotion,
            ItemID.TeleportationPotion,
            ItemID.WormholePotion,
            ItemID.PotionOfReturn
        };

        public static readonly List<List<int>> BuffTiles = new() {
            new() { TileID.CatBast, -1, BuffID.CatBast }, // 巴斯特雕像
            new() { TileID.Campfire, -1, BuffID.Campfire }, // 篝火
            new() { TileID.Fireplace, -1, BuffID.Campfire }, // 壁炉
            new() { TileID.HangingLanterns, 9, BuffID.HeartLamp }, // 红心灯笼
            new() { TileID.HangingLanterns, 7, BuffID.StarInBottle }, // 星星瓶
            new() { TileID.Sunflower, -1, BuffID.Sunflower }, // 向日葵
            new() { TileID.AmmoBox, -1, BuffID.AmmoBox }, // 弹药箱
            new() { TileID.BewitchingTable, -1, BuffID.Bewitched }, // 施法桌
            new() { TileID.CrystalBall, -1, BuffID.Clairvoyance }, // 水晶球
            new() { TileID.SliceOfCake, -1, BuffID.SugarRush }, // 蛋糕块
            new() { TileID.SharpeningStation, -1, BuffID.Sharpened }, // 利器站
            new() { TileID.WaterCandle, -1, BuffID.WaterCandle }, // 水蜡烛
            new() { TileID.PeaceCandle, -1, BuffID.PeaceCandle } // 和平蜡烛
        };

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
            if (Config.NoConsume_Potion && item.stack >= Config.NoConsume_PotionRequirement && SpecialPotions.Contains(item.type) && globalItemNotNull)
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
            for (int i = 0; i < BuffTiles.Count; i++) {
                if (item.createTile == BuffTiles[i][0] && (item.placeStyle == BuffTiles[i][1] || BuffTiles[i][1] == -1)) {
                    buffType = BuffTiles[i][2];
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
            
            if (Config.NoConsume_Potion && item.stack >= Config.NoConsume_PotionRequirement && (item.buffType > 0 || SpecialPotions.Contains(item.type))) {
                return false;
            }
            return base.ConsumeItem(item, player);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            if (!item.TryGetGlobalItem<GlobalItemData>(out var global) || !global.InventoryGlow)
                return;

            if (IsBuffTileItem(item, out _) || item.type == ItemID.HoneyBucket || item.type == ItemID.GardenGnome ||
                (item.stack >= Config.NoConsume_PotionRequirement && item.buffType > 0 && item.active)) {
                int buffType = GetItemBuffType(item);

                if (buffType is -1 && item.type != ItemID.GardenGnome) return;

                if (Main.mouseMiddle && Main.mouseMiddleRelease) {
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
