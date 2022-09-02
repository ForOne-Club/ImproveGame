using ImproveGame.Common.Players;
using ImproveGame.Interface.UIElements;
using Mono.Cecil.Cil;
using MonoMod.Cil;
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

        public override void SetDefaults(Item item)
        {
            // 最大堆叠，修改条件
            // 物品最大值 == 1
            // 不为有伤害的近战武器
            // 不为铜币，银币，金币
            if (item.maxStack > 1
                && !(item.DamageType == DamageClass.Melee && item.damage > 0)
                && !GoldList.Contains(item.type))
            {
                item.maxStack = ModContent.GetInstance<Configs.ImproveConfigs>().ItemMaxStack;
                if (item.type == ItemID.PlatinumCoin && item.maxStack > 2000)
                {
                    item.maxStack = 2000;
                }
            }

            // 所有饰品可放在时装槽
            if (item.accessory)
            {
                item.canBePlacedInVanityRegardlessOfConditions = true;
            }
        }

        // 用这个和公式来加成工具速度，这样就不需要Reload了
        // 同时不会太BT，原先的写法的使用速度实际上是 8*0.75=6 非常快，因为有个 pickSpeed -0.25 在ImprovePlayer
        // Ju 2022.6.27: 去掉 ImprovePlayer 的额外速度，工具速度提升方式：减少工具使用间隔。（并非为提升工具速度）
        private void ActuallyUseMiningTool(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(MoveType.After,
                i => i.Match(OpCodes.Ldarg_0),
                i => i.Match(OpCodes.Ldarg_1),
                i => i.Match(OpCodes.Ldloc_0)))
                return;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Action<Player>>((player) =>
            {
                player.SetItemTime((int)MathHelper.Max(1, MathF.Round(player.itemTime * (1f - Config.ExtraToolSpeed))));
            });
        }
        private void TryHittingWall(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(MoveType.After,
                i => i.Match(OpCodes.Div),
                i => i.Match(OpCodes.Stfld)))
                return;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Action<Player>>((player) =>
            {
                player.SetItemTime((int)MathHelper.Max(1, MathF.Round(player.itemTime * (1f - Config.ExtraToolSpeed))));
            });
        }

        public override bool? CanAutoReuseItem(Item item, Player player)
        {
            // 自动挥舞
            ItemDefinition itemd = new(item.type);
            if (item.damage > 0 && Config.AutoReuseWeapon
                && !Config.AutoReuseWeapon_ExclusionList.Contains(itemd))
            {
                return true;
            }
            return base.CanAutoReuseItem(item, player);
        }

        // 物品消耗
        public override bool ConsumeItem(Item item, Player player)
        {
            // 所有召唤物不会消耗
            if (Config.NoConsume_SummonItem && (SummonsList1.Contains(item.type) || SummonsList2.Contains(item.type)))
            {
                return false;
            }
            return base.ConsumeItem(item, player);
        }

        public override bool CanBeConsumedAsAmmo(Item ammo, Item weapon, Player player)
        {
            if (Config.NoConsume_Ammo && ammo.stack >= 3996 && ammo.ammo > 0)
            {
                return false;
            }
            return base.CanBeConsumedAsAmmo(ammo, weapon, player);
        }

        // 前缀回滚
        public override bool AllowPrefix(Item item, int pre)
        {
            GlobalItemData itemVar = item.GetGlobalItem<GlobalItemData>();
            if (itemVar.recastCount == 0)
            {
                return true;
            }
            if (Config.ImprovePrefix) // 新的重铸机制
            {
                // 饰品
                if (PrefixLevel.ContainsKey(pre))
                {
                    if (item.accessory)
                    {
                        if (itemVar.recastCount < 2)
                        {
                            return PrefixLevel[pre] >= 1;
                        }
                        else if (itemVar.recastCount < 4)
                        {
                            return PrefixLevel[pre] >= 2;
                        }
                        else if (itemVar.recastCount < 6)
                        {
                            return PrefixLevel[pre] >= 3;
                        }
                        else
                        {
                            return PrefixLevel[pre] >= 4;
                        }
                    }
                    else
                    {
                        // 工具
                        if (pre == 15 && (item.axe > 0 || item.hammer > 0 || item.pick > 0))
                        {
                            return true;
                        }
                        // 召唤武器，但是不算入鞭子
                        if (pre == 57 && item.DamageType == DamageClass.Summon)
                        {
                            return true;
                        }
                        if (itemVar.recastCount < 3)
                        {
                            return PrefixLevel[pre] >= 0;
                        }
                        else if (itemVar.recastCount < 6)
                        {
                            return PrefixLevel[pre] >= 1;
                        }
                        else
                        {
                            return PrefixLevel[pre] >= 2;
                        }
                    }
                }
            }
            return true;
        }

        // 重铸次数统计
        public override void PostReforge(Item item)
        {
            GlobalItemData itemVar = item.GetGlobalItem<GlobalItemData>();
            itemVar.recastCount++;
        }

        // 额外提示
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // 重铸次数
            if (Config.ShowPrefixCount && (item.accessory ||
                (item.damage > 0 && item.maxStack == 1 && item.ammo == AmmoID.None &&
                item.DamageType != DamageClass.Generic)))
            {
                GlobalItemData itemVar = item.GetGlobalItem<GlobalItemData>();
                tooltips.Add(new TooltipLine(Mod, "ReforgeCount", Language.GetTextValue($"Mods.ImproveGame.Tips.PrefixCount") + itemVar.recastCount));
            }
            // 更多信息
            if (Config.ShowItemMoreData)
            {
                tooltips.Add(new(Mod, "Type", "Type: " + item.type));
                tooltips.Add(new(Mod, "useTime", "UseTime: " + item.useTime));
                tooltips.Add(new(Mod, "UseAnimation", "UseAnimation: " + item.useAnimation));
                if (item.shoot > ProjectileID.None)
                {
                    tooltips.Add(new(Mod, "Shoot", "Shoot: " + item.shoot));
                    tooltips.Add(new(Mod, "ShootSpeed", "ShootSpeed: " + item.shootSpeed));
                }
                if (item.ammo > ItemID.None)
                    tooltips.Add(new(Mod, "Ammo", "Ammo: " + item.ammo));
                if (item.buffType > 0)
                {
                    tooltips.Add(new(Mod, "BuffType", "BuffType: " + item.buffType));
                    tooltips.Add(new(Mod, "BuffTime", "BuffTime: " + item.buffTime));
                }
                if (item.tileWand > -1)
                {
                    tooltips.Add(new(Mod, "TileWand", "TileWand: " + item.tileWand));
                }
                if (item.createTile > -1)
                {
                    tooltips.Add(new(Mod, "CreateTile", "CreateTile: " + item.createTile));
                    if (item.placeStyle > 0)
                        tooltips.Add(new(Mod, "PlaceStyle", "PlaceStyle: " + item.placeStyle));
                }
                if (item.createWall > -1)
                    tooltips.Add(new(Mod, "CreateWall", "CreateWall: " + item.createWall));
            }
        }

        // 额外拾取距离
        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            grabRange += Config.GrabDistance * 16;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.GetGlobalItem<GlobalItemData>().InventoryGlow)
            {
                ArrayItemSlot.OpenItemGlow(sb, item);
                return true;
            }
            return true;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.GetGlobalItem<GlobalItemData>().InventoryGlow)
            {
                ArrayItemSlot.CloseItemGlow(sb);
                item.GetGlobalItem<GlobalItemData>().InventoryGlow = false;
            }
        }

        /// <summary>
        /// 物品是否可吸附处理
        /// </summary>
        /// <param name="item"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public override bool ItemSpace(Item item, Player player)
        {
            if (!DataPlayer.TryGet(player, out var dataPlayer))
                return false;

            if (Config.SuperVault && dataPlayer.SuperVault is not null && HasItemSpace(dataPlayer.SuperVault, item))
            {
                return true;
            }

            if (Config.SuperVoidVault && player.TryGetModPlayer<ImprovePlayer>(out var improvePlayer))
            {
                // 猪猪钱罐
                if (improvePlayer.PiggyBank && HasItemSpace(player.bank.item, item))
                {
                    return true;
                }
                // 保险箱
                if (improvePlayer.Safe && HasItemSpace(player.bank2.item, item))
                {
                    return true;
                }
                // 护卫熔炉
                if (improvePlayer.DefendersForge && HasItemSpace(player.bank3.item, item))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Load()
        {
            // 对 Tile 操作的工具
            IL.Terraria.Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += ActuallyUseMiningTool;
            // 对 Wall 操作的工具
            IL.Terraria.Player.ItemCheck_UseMiningTools_TryHittingWall += TryHittingWall;
        }
    }
}
