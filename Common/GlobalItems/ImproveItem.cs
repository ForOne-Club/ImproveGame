using ImproveGame.Common.Players;
using ImproveGame.Interface.UIElements;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.GlobalItems
{
    public class ImproveItem : GlobalItem
    {
        // 铜，银，金
        public static readonly HashSet<int> Coins = new() { 71, 72, 73 };
        // BOSS 召唤物
        public static readonly HashSet<int> SummonItems_Boss = new() { 560, 43, 70, 1331, 1133, 5120, 4988, 556, 544, 557, 3601 };
        // 特殊事件 召唤物
        public static readonly HashSet<int> SummonItems_Events = new() { 4271, 361, 1315, 2767, 602, 1844, 1958 };

        public override void SetDefaults(Item item)
        {
            // 最大堆叠
            if (item.maxStack > 1 && Config.ItemMaxStack > item.maxStack && item.DamageType != DamageClass.Melee && !Coins.Contains(item.type))
            {
                item.maxStack = Config.ItemMaxStack;
                if (item.type == ItemID.PlatinumCoin && item.stack > 2000) item.maxStack = 2000;
            }

            // 允许任何饰品放入饰品时装栏
            if (item.accessory) item.canBePlacedInVanityRegardlessOfConditions = true;
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
                && !Config.AutoReuseWeaponExclusionList.Contains(itemd))
            {
                return true;
            }
            return base.CanAutoReuseItem(item, player);
        }

        // 物品消耗
        public override bool ConsumeItem(Item item, Player player)
        {
            // 所有召唤物不会消耗
            if (Config.NoConsume_SummonItem && (SummonItems_Boss.Contains(item.type) || SummonItems_Events.Contains(item.type)))
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

        // 额外提示
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // 更多信息
            if (Config.ShowItemMoreData)
            {
                tooltips.Add(new(Mod, "Rare", "Rare: " + item.rare));
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
                ItemSlot_BigBag.OpenItemGlow(sb);
                return true;
            }
            return true;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.GetGlobalItem<GlobalItemData>().InventoryGlow)
            {
                sb.Begin(null, Main.UIScaleMatrix);
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
