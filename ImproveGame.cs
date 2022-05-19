using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.GlobalPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame
{
    // 更新任务
    // 神庙电池（现版本右键使用会消耗），松露虫，光女召唤物下版本更新加入不消耗
    // Tile 工具：自动钓鱼，自动采集，自动挖矿
    // Buff Tile 在背包也可以获得 Buff （已完成）
    // 刷怪率 UI
    public class ImproveGame : Mod
    {
        public static Effect npcEffect;
        public static Effect strokeEffect;

        // 额外BUFF栏
        public override uint ExtraPlayerBuffSlots => 22;

        public override void Load()
        {
            npcEffect = Assets.Request<Effect>("npc", AssetRequestMode.ImmediateLoad).Value;
            strokeEffect = Assets.Request<Effect>("stroke", AssetRequestMode.ImmediateLoad).Value;
            // 加载前缀信息
            Utils.LoadPrefixInfo();
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
            // 管理员作弊之无视防御
            On.Terraria.Main.CalculateDamageNPCsTake += Main_CalculateDamageNPCsTake;
            // 使存钱罐中物品生效，如同放入背包一样
            On.Terraria.Player.VanillaPreUpdateInventory += Player_VanillaPreUpdateInventory;
            // 旗帜更新
            On.Terraria.SceneMetrics.ScanAndExportToMain += SceneMetrics_ScanAndExportToMain;
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

        private void SceneMetrics_ScanAndExportToMain(On.Terraria.SceneMetrics.orig_ScanAndExportToMain orig, SceneMetrics self, SceneMetricsScanSettings settings)
        {
            orig(self, settings);
            // 随身旗帜（增益站）
            if (Utils.GetConfig().NoPlace_BUFFTile)
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
            }
        }

        // 使存钱罐中物品如同放在背包
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
        }

        private double Main_CalculateDamageNPCsTake(On.Terraria.Main.orig_CalculateDamageNPCsTake orig, int Damage, int Defense)
        {
            // 管理员作弊专用（）
            if (Main.LocalPlayer.GetModPlayer<SaveAndLoadDataPlayer>().IgnoreDefense
                && (Main.netMode == NetmodeID.MultiplayerClient || Main.netMode == NetmodeID.SinglePlayer))
            {
                return Damage * 2;
            }
            else
            {
                return orig(Damage, Defense);
            }
        }

        private int Main_DamageVar(On.Terraria.Main.orig_DamageVar orig, float dmg, float luck)
        {
            if (Utils.GetConfig().BanDamageVar)
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
                if (Utils.GetConfig().TownNPCSpawnInNight)
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
                return (int)Math.Pow(2, Utils.GetConfig().TownNPCSpawnSpeed);
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
                            Utils.GetPlatformCount(inv, ref count);
                            return count;
                        }
                        else if (inv[slot].type == ModContent.ItemType<Content.Items.WallPlace>())
                        {
                            int count = 0;
                            Utils.GetWallCount(inv, ref count);
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

        // 物品吸取速度
        private void Player_PullItem_Common(On.Terraria.Player.orig_PullItem_Common orig, Player player, Item item, float xPullSpeed)
        {
            if (Utils.GetConfig().GrabDistance > 0)
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

        // 墓碑掉落
        private void Player_DropTombstone(On.Terraria.Player.orig_DropTombstone orig, Player self, int coinsOwned, Terraria.Localization.NetworkText deathText, int hitDirection)
        {
            if (!Utils.GetConfig().BanTombstone)
            {
                orig(self, coinsOwned, deathText, hitDirection);
            }
        }

        // 前缀保存
        private void Player_dropItemCheck(On.Terraria.Player.orig_dropItemCheck orig, Player self)
        {
            if (Main.reforgeItem.type > ItemID.None && self.GetModPlayer<SaveAndLoadDataPlayer>().ReforgeItemPrefix > 0)
            {
                Main.reforgeItem.GetGlobalItem<ItemVar>().recastCount =
                    self.GetModPlayer<SaveAndLoadDataPlayer>().ReforgeItemPrefix;
                self.GetModPlayer<SaveAndLoadDataPlayer>().ReforgeItemPrefix = 0;
            }
            orig(self);
        }
    }
}