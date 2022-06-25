using ImproveGame.Common.Players;
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
    public class ImproveItem : GlobalItem
    {
        // 金币识别码
        public static readonly List<int> GoldList = new() { 71, 72, 73 };
        // BOSS召唤物识别码
        public static readonly List<int> SummonsList1 = new() { 560, 43, 70, 1331, 1133, 5120, 4988, 556, 544, 557, 3601 };
        // 事件召唤物识别码
        public static readonly List<int> SummonsList2 = new() { 4271, 361, 1315, 2767, 602, 1844, 1958 };
        // 前缀等级
        public static readonly Dictionary<int, int> PrefixLevel = new();

        public override void SetDefaults(Item item) {
            // 最大堆叠，修改条件
            // 物品最大值 == 1
            // 不为有伤害的近战武器
            // 不为铜币，银币，金币
            if (item.maxStack > 1
                && !(item.DamageType == DamageClass.Melee && item.damage > 0)
                && !GoldList.Contains(item.type)) {
                item.maxStack = ModContent.GetInstance<Configs.ImproveConfigs>().ItemMaxStack;
                if (item.type == ItemID.PlatinumCoin && item.maxStack > 18888) {
                    item.maxStack = 18888;
                }
            }

            // 所有饰品可放在时装槽
            if (item.accessory) {
                item.canBePlacedInVanityRegardlessOfConditions = true;
            }
        }

        // 用这个和公式来加成工具速度，这样就不需要Reload了
        // 同时不会太BT，原先的写法的使用速度实际上是 8*0.75=6 非常快，因为有个 pickSpeed -0.25 在ImprovePlayer
        public override float UseSpeedMultiplier(Item item, Player player) {
            if ((item.axe > 0 || item.pick > 0 || item.hammer > 0) && item.useTime > 8 && MyUtils.Config.ImproveToolSpeed) {
                return item.useTime / 8f;
            }
            return base.UseSpeedMultiplier(item, player);
        }


        public override bool? CanAutoReuseItem(Item item, Player player) {
            // 自动挥舞
            ItemDefinition itemd = new(item.type);
            if (item.damage > 0 && MyUtils.Config.AutoReuseWeapon
                && !MyUtils.Config.AutoReuseWeapon_ExclusionList.Contains(itemd)) {
                return true;
            }
            return base.CanAutoReuseItem(item, player);
        }

        // 物品消耗
        public override bool ConsumeItem(Item item, Player player) {
            // 所有召唤物不会消耗
            if ((SummonsList1.Contains(item.type) || SummonsList2.Contains(item.type)) && MyUtils.Config.NoConsume_SummonItem) {
                return false;
            }

            return base.ConsumeItem(item, player);
        }

        public override bool CanBeConsumedAsAmmo(Item ammo, Item weapon, Player player) {
            if (MyUtils.Config.NoConsume_Ammo && ammo.stack >= 3996 && ammo.ammo > 0) {
                return false;
            }
            return base.CanBeConsumedAsAmmo(ammo, weapon, player);
        }

        // 前缀回滚
        public override bool AllowPrefix(Item item, int pre) {
            GlobalItemData itemVar = item.GetGlobalItem<GlobalItemData>();
            if (itemVar.recastCount == 0) {
                return true;
            }
            if (MyUtils.Config.ImprovePrefix) // 新的重铸机制
            {
                // 饰品
                if (PrefixLevel.ContainsKey(pre)) {
                    if (item.accessory) {
                        if (itemVar.recastCount < 2) {
                            return PrefixLevel[pre] >= 1;
                        }
                        else if (itemVar.recastCount < 4) {
                            return PrefixLevel[pre] >= 2;
                        }
                        else if (itemVar.recastCount < 6) {
                            return PrefixLevel[pre] >= 3;
                        }
                        else {
                            return PrefixLevel[pre] >= 4;
                        }
                    }
                    else {
                        // 工具
                        if (pre == 15 && (item.axe > 0 || item.hammer > 0 || item.pick > 0)) {
                            return true;
                        }
                        // 召唤武器，但是不算入鞭子
                        if (pre == 57 && item.DamageType == DamageClass.Summon) {
                            return true;
                        }
                        if (itemVar.recastCount < 3) {
                            return PrefixLevel[pre] >= 0;
                        }
                        else if (itemVar.recastCount < 6) {
                            return PrefixLevel[pre] >= 1;
                        }
                        else {
                            return PrefixLevel[pre] >= 2;
                        }
                    }
                }
            }
            return true;
        }

        // 重铸次数统计
        public override void PostReforge(Item item) {
            GlobalItemData itemVar = item.GetGlobalItem<GlobalItemData>();
            itemVar.recastCount++;
        }

        // 额外提示
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            // 重铸次数
            if (MyUtils.Config.ShowPrefixCount && (item.accessory ||
                (item.damage > 0 && item.maxStack == 1 && item.ammo == AmmoID.None &&
                item.DamageType != DamageClass.Generic))) {
                GlobalItemData itemVar = item.GetGlobalItem<GlobalItemData>();
                TooltipLine tooltip = new TooltipLine(Mod, "重铸次数", Language.GetTextValue($"Mods.ImproveGame.Tips.PrefixCount") + itemVar.recastCount);
                tooltips.Add(tooltip);
            }
            // 更多信息
            if (MyUtils.Config.ShowItemMoreData) {
                tooltips.Add(new(Mod, "Type", "Type: " + item.type));
                tooltips.Add(new(Mod, "useTime", "UseTime: " + item.useTime));
                tooltips.Add(new(Mod, "UseAnimation", "UseAnimation: " + item.useAnimation));
                tooltips.Add(new(Mod, "Shoot", "Shoot: " + item.shoot));
                tooltips.Add(new(Mod, "ShootSpeed", "ShootSpeed: " + item.shootSpeed));
                tooltips.Add(new(Mod, "Ammo", "Ammo: " + item.ammo));
                tooltips.Add(new(Mod, "BuffType", "BuffType: " + item.buffType));
                tooltips.Add(new(Mod, "BuffTime", "BuffTime: " + item.buffTime));
                tooltips.Add(new(Mod, "CreateTile", "CreateTile: " + item.createTile));
                tooltips.Add(new(Mod, "PlaceStyle", "PlaceStyle: " + item.placeStyle));
                tooltips.Add(new(Mod, "CreateWall", "CreateWall: " + item.createWall));
            }
        }

        // 额外拾取距离
        public override void GrabRange(Item item, Player player, ref int grabRange) {
            grabRange += MyUtils.Config.GrabDistance * 16;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            if (item.GetGlobalItem<GlobalItemData>().InventoryGlow) {
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                    DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
                Color lerpColor;
                float time = ImprovePlayer.G(Main.LocalPlayer).PlayerTimer;
                if (time % 60f < 30) {
                    lerpColor = Color.Lerp(Color.White * 0.25f, Color.Transparent, (float)(time % 60f % 30 / 29));
                }
                else {
                    lerpColor = Color.Lerp(Color.Transparent, Color.White * 0.25f, (float)(time % 60f % 30 / 29));
                }
                MyAssets.ItemEffect.Parameters["uColor"].SetValue(lerpColor.ToVector4());
                MyAssets.ItemEffect.CurrentTechnique.Passes["Test"].Apply();
                return true;
            }
            return true;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            if (item.GetGlobalItem<GlobalItemData>().InventoryGlow) {
                item.GetGlobalItem<GlobalItemData>().InventoryGlow = false;
                sb.End();
                sb.Begin(0, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                    DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            }
        }

        /// <summary>
        /// 物品是否可吸附处理
        /// </summary>
        /// <param name="item"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public override bool ItemSpace(Item item, Player player) {
            ImprovePlayer improvePlayer = ImprovePlayer.G(player);

            if (MyUtils.Config.SuperVault && MyUtils.HasItemSpace(player.GetModPlayer<DataPlayer>().SuperVault, item)) {
                return true;
            }
            if (MyUtils.Config.SuperVoidVault) {
                // 猪猪钱罐
                if (improvePlayer.PiggyBank && MyUtils.HasItemSpace(player.bank.item, item)) {
                    return true;
                }
                // 保险箱
                if (improvePlayer.Safe && MyUtils.HasItemSpace(player.bank2.item, item)) {
                    return true;
                }
                // 护卫熔炉
                if (improvePlayer.DefendersForge && MyUtils.HasItemSpace(player.bank3.item, item)) {
                    return true;
                }
            }
            return false;
        }
    }
}
