using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Players;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame
{
    // 更新任务
    // 神庙电池（现版本右键使用会消耗），松露虫，光女召唤物下版本更新加入不消耗 ×
    // Tile 工具：自动钓鱼，自动采集，自动挖矿
    // Buff Tile 在背包也可以获得 Buff （已完成）
    // 刷怪率 UI
    public class ImproveGame : Mod
    {
        // 额外BUFF槽
        public override uint ExtraPlayerBuffSlots => (uint)MyUtils.Config().ExtraPlayerBuffSlots;

        public override void Load()
        {
            // 加载前缀信息
            MyUtils.LoadPrefixInfo();
            // 还原哥布林重铸槽中物品的重铸次数
            On.Terraria.Player.dropItemCheck += Player_dropItemCheck;
            // 死亡是否掉落墓碑
            On.Terraria.Player.DropTombstone += Player_DropTombstone;
            // 抓取距离修改
            On.Terraria.Player.PullItem_Common += Player_PullItem_Common;
            // 晚上刷新 NPC
            IL.Terraria.Main.UpdateTime += Main_UpdateTime;
            // 城镇NPC入住速度修改
            IL.Terraria.Main.UpdateTime_SpawnTownNPCs += Main_UpdateTime_SpawnTownNPCs;
            // 修改空间法杖显示平台剩余数量
            IL.Terraria.UI.ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
            // 伤害波动
            On.Terraria.Main.DamageVar += Main_DamageVar;
            // 使存钱罐中物品生效，如同放入背包一样
            On.Terraria.Player.VanillaPreUpdateInventory += Player_VanillaPreUpdateInventory;
            // 旗帜更新
            On.Terraria.SceneMetrics.ScanAndExportToMain += SceneMetrics_ScanAndExportToMain;
            // 拾取物品处理方法
            On.Terraria.Player.PickupItem += Player_PickupItem;
        }

        /// <summary>
        /// 拾取物品的时候
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="player"></param>
        /// <param name="playerIndex"></param>
        /// <param name="worldItemArrayIndex"></param>
        /// <param name="itemToPickUp"></param>
        /// <returns></returns>
        private Item Player_PickupItem(On.Terraria.Player.orig_PickupItem orig, Player player, int playerIndex, int worldItemArrayIndex, Item itemToPickUp)
        {
            ImprovePlayer improvePlayer = ImprovePlayer.G(player);
            // 智能虚空保险库
            if (MyUtils.Config().SmartVoidVault)
            {
                if (itemToPickUp.type != ItemID.None && itemToPickUp.stack > 0 && !itemToPickUp.IsACoin)
                {
                    if (MyUtils.Config().SuperVault && MyUtils.HasItem(player.GetModPlayer<DataPlayer>().SuperVault, itemToPickUp))
                    {
                        itemToPickUp = MyUtils.StackItemToInv(player.whoAmI, player.GetModPlayer<DataPlayer>().SuperVault,
                            itemToPickUp, GetItemSettings.PickupItemFromWorld);
                    }
                    if (player.IsVoidVaultEnabled && MyUtils.HasItem(player.bank4.item, itemToPickUp))
                    {
                        itemToPickUp = MyUtils.StackItemToInv(player.whoAmI, player.bank4.item, itemToPickUp, GetItemSettings.PickupItemFromWorld);
                    }
                    // 超级虚空保险库
                    if (MyUtils.Config().SuperVoidVault)
                    {
                        if (improvePlayer.PiggyBank && MyUtils.HasItem(player.bank.item, itemToPickUp))
                        {
                            itemToPickUp = MyUtils.StackItemToInv(player.whoAmI, player.bank.item, itemToPickUp, GetItemSettings.PickupItemFromWorld);
                        }
                        if (improvePlayer.Safe && MyUtils.HasItem(player.bank2.item, itemToPickUp))
                        {
                            itemToPickUp = MyUtils.StackItemToInv(player.whoAmI, player.bank2.item, itemToPickUp, GetItemSettings.PickupItemFromWorld);
                        }
                        if (improvePlayer.DefendersForge && MyUtils.HasItem(player.bank3.item, itemToPickUp))
                        {
                            itemToPickUp = MyUtils.StackItemToInv(player.whoAmI, player.bank3.item, itemToPickUp, GetItemSettings.PickupItemFromWorld);
                        }
                    }
                }
            }
            Item item = orig(player, playerIndex, worldItemArrayIndex, itemToPickUp);
            if (MyUtils.Config().SuperVault && item.type != ItemID.None && item.stack > 0 && !item.IsACoin)
            {
                item = MyUtils.StackItemToInv(player.whoAmI, player.GetModPlayer<DataPlayer>().SuperVault, item, GetItemSettings.PickupItemFromWorld);
            }
            // 超级虚空保险库
            if (MyUtils.Config().SuperVoidVault)
            {
                if (item.type != ItemID.None && item.stack > 0 && !item.IsACoin)
                {
                    if (improvePlayer.PiggyBank)
                    {
                        item = MyUtils.StackItemToInv(player.whoAmI, player.bank.item, item, GetItemSettings.PickupItemFromWorld);
                    }
                    if (improvePlayer.Safe && item.type != ItemID.None && item.stack > 0)
                    {
                        item = MyUtils.StackItemToInv(player.whoAmI, player.bank2.item, item, GetItemSettings.PickupItemFromWorld);
                    }
                    if (improvePlayer.DefendersForge && item.type != ItemID.None && item.stack > 0)
                    {
                        item = MyUtils.StackItemToInv(player.whoAmI, player.bank3.item, item, GetItemSettings.PickupItemFromWorld);
                    }
                }
            }
            Main.item[worldItemArrayIndex] = item;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, worldItemArrayIndex);
            }
            return item;
        }

        /// <summary>
        /// 旗帜BUFF在背包生效
        /// </summary>
        /// <param name="self"></param>
        /// <param name="player"></param>
        /// <param name="item"></param>
        private static void AddBannerBuff(SceneMetrics self, Player player, Item item)
        {
            if (item.createTile == TileID.Banners)
            {
                int style = item.placeStyle;
                int frameX = style * 18;
                int frameY = 0;
                if (style >= 90)
                {
                    frameX -= 1620;
                    frameY += 54;
                }
                if (frameX >= 396 || frameY >= 54)
                {
                    int styleX = frameX / 18 - 21;
                    for (int num4 = frameY; num4 >= 54; num4 -= 54)
                    {
                        styleX += 90;
                    }
                    self.NPCBannerBuff[styleX] = true;
                    self.hasBanner = true;
                }
            }
        }

        /// <summary>
        /// 旗帜增益
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="settings"></param>
        private void SceneMetrics_ScanAndExportToMain(On.Terraria.SceneMetrics.orig_ScanAndExportToMain orig, SceneMetrics self, SceneMetricsScanSettings settings)
        {
            orig(self, settings);
            // 随身旗帜（增益站）
            if (MyUtils.Config().NoPlace_BUFFTile_Banner)
            {
                Player player = Main.LocalPlayer;
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    Item item = player.inventory[i];
                    if (item.type == ItemID.None)
                        continue;
                    AddBannerBuff(self, player, item);
                }
                for (int i = 0; i < player.bank.item.Length; i++)
                {
                    Item item = player.bank.item[i];
                    if (item.type == ItemID.None)
                        continue;
                    AddBannerBuff(self, player, item);
                }
                for (int i = 0; i < player.bank2.item.Length; i++)
                {
                    Item item = player.bank2.item[i];
                    if (item.type == ItemID.None)
                        continue;
                    AddBannerBuff(self, player, item);
                }
                for (int i = 0; i < player.bank3.item.Length; i++)
                {
                    Item item = player.bank3.item[i];
                    if (item.type == ItemID.None)
                        continue;
                    AddBannerBuff(self, player, item);
                }
                for (int i = 0; i < player.bank4.item.Length; i++)
                {
                    Item item = player.bank4.item[i];
                    if (item.type == ItemID.None)
                        continue;
                    AddBannerBuff(self, player, item);
                }
                if (MyUtils.Config().SuperVault)
                {
                    for (int i = 0; i < player.GetModPlayer<DataPlayer>().SuperVault.Length; i++)
                    {
                        Item item = player.GetModPlayer<DataPlayer>().SuperVault[i];
                        if (item.type == ItemID.None)
                            continue;
                        AddBannerBuff(self, player, item);
                    }
                }
            }
        }

        /// <summary>
        /// 使存钱罐中物品如同放在背包
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void Player_VanillaPreUpdateInventory(On.Terraria.Player.orig_VanillaPreUpdateInventory orig, Player self)
        {
            orig(self);
            for (int i = 0; i < self.bank.item.Length; i++)
            {
                self.VanillaUpdateInventory(self.bank.item[i]);
            }
            for (int i = 0; i < self.bank2.item.Length; i++)
            {
                self.VanillaUpdateInventory(self.bank2.item[i]);
            }
            for (int i = 0; i < self.bank3.item.Length; i++)
            {
                self.VanillaUpdateInventory(self.bank3.item[i]);
            }
            for (int i = 0; i < self.bank4.item.Length; i++)
            {
                self.VanillaUpdateInventory(self.bank4.item[i]);
            }
            if (MyUtils.Config().SuperVault)
            {
                DataPlayer dataPlayer = self.GetModPlayer<DataPlayer>();
                for (int i = 0; i < dataPlayer.SuperVault.Length; i++)
                {
                    self.VanillaUpdateInventory(dataPlayer.SuperVault[i]);
                }
            }
        }

        /// <summary>
        /// 伤害波动
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="dmg"></param>
        /// <param name="luck"></param>
        /// <returns></returns>
        private int Main_DamageVar(On.Terraria.Main.orig_DamageVar orig, float dmg, float luck)
        {
            if (MyUtils.Config().BanDamageVar)
                return (int)Math.Round(dmg);
            else
                return orig(dmg, luck);
        }

        // NPC 晚上刷新
        private void Main_UpdateTime(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchCall(typeof(Main), "UpdateTime_StartDay"),
                i => i.MatchCall(typeof(Main), "HandleMeteorFall")))
                return;
            c.EmitDelegate(() =>
            {
                if (MyUtils.Config().TownNPCSpawnInNight)
                {
                    MethodInfo methodInfo = typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.Static | BindingFlags.NonPublic);
                    methodInfo.Invoke(null, null);
                }
            });
        }

        // NPC 刷新速度
        private void Main_UpdateTime_SpawnTownNPCs(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdsfld(typeof(Main), nameof(Main.checkForSpawns)),
                i => i.Match(OpCodes.Ldc_I4_1)))
                return;
            c.EmitDelegate<Func<int, int>>((JiaJi) =>
            {
                return (int)Math.Pow(2, MyUtils.Config().TownNPCSpawnSpeed);
            });
        }

        // 空间法杖计算剩余平台数
        private void ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(ILContext il)
        {
            // 计算剩余平台
            var c = new ILCursor(il);
            if (!c.TryGotoNext(MoveType.After,
                i => i.Match(OpCodes.Pop),
                i => i.Match(OpCodes.Ldc_I4_M1)))
                return;
            c.Emit(OpCodes.Ldarg_1); // 玩家物品槽
            c.Emit(OpCodes.Ldarg_2); // content
            c.Emit(OpCodes.Ldarg_3); // 物品在物品槽的位置
            c.EmitDelegate<Func<int, Item[], int, int, int>>((num11, inv, content, slot) =>
                {
                    if (content == 13)
                    {
                        if (inv[slot].type == ModContent.ItemType<Content.Items.SpaceWand>())
                        {
                            int count = 0;
                            MyUtils.GetPlatformCount(inv, ref count);
                            return count;
                        }
                        else if (inv[slot].type == ModContent.ItemType<Content.Items.WallPlace>())
                        {
                            int count = 0;
                            MyUtils.GetWallCount(inv, ref count);
                            return count;
                        }
                        return -1;
                    }
                    else
                    {
                        return -1;
                    }
                });
        }

        /// <summary>
        /// 物品吸取速度
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="player"></param>
        /// <param name="item"></param>
        /// <param name="xPullSpeed"></param>
        private void Player_PullItem_Common(On.Terraria.Player.orig_PullItem_Common orig, Player player, Item item, float xPullSpeed)
        {
            if (MyUtils.Config().GrabDistance > 0)
            {
                Vector2 velocity = (player.Center - item.Center).SafeNormalize(Vector2.Zero);
                if (item.velocity.Length() + velocity.Length() > 15f)
                {
                    item.velocity = velocity * 15f;
                }
                else
                {
                    item.velocity = velocity * (item.velocity.Length() + 1);
                }
            }
            else
            {
                orig(player, item, xPullSpeed);
            }
        }

        /// <summary>
        /// 墓碑掉落
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="coinsOwned"></param>
        /// <param name="deathText"></param>
        /// <param name="hitDirection"></param>
        private void Player_DropTombstone(On.Terraria.Player.orig_DropTombstone orig, Player self, int coinsOwned, Terraria.Localization.NetworkText deathText, int hitDirection)
        {
            if (!MyUtils.Config().BanTombstone)
            {
                orig(self, coinsOwned, deathText, hitDirection);
            }
        }

        /// <summary>
        /// 前缀保存
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void Player_dropItemCheck(On.Terraria.Player.orig_dropItemCheck orig, Player self)
        {
            if (Main.reforgeItem.type > ItemID.None && self.GetModPlayer<DataPlayer>().ReforgeItemPrefix > 0)
            {
                Main.reforgeItem.GetGlobalItem<GlobalItemData>().recastCount =
                    self.GetModPlayer<DataPlayer>().ReforgeItemPrefix;
                self.GetModPlayer<DataPlayer>().ReforgeItemPrefix = 0;
            }
            orig(self);
        }
    }
}