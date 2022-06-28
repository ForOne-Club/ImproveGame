using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace ImproveGame
{
    partial class MyUtils
    {
        /// <summary>
        /// 快捷开关箱子
        /// </summary>
        /// <param name="player">玩家实例</param>
        /// <param name="chestID">箱子ID（对于便携储存是-2/-3/-4/-5，对于其他箱子是在<see cref="Main.chest"/>的索引）</param>
        public static void ToggleChest(ref Player player, int chestID, int x = -1, int y = -1, SoundStyle? sound = null) {
            if (player.chest == chestID) {
                player.chest = -1;
                if (sound is null) {
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                else {
                    SoundEngine.PlaySound(sound.Value);
                }
            }
            else {
                x = x == -1 ? player.Center.ToTileCoordinates().X : x;
                y = y == -1 ? player.Center.ToTileCoordinates().Y : y;
                // 以后版本TML会加的东西，只不过现在stable还没有，现在就先放在这里吧
                //player.OpenChest(x, y, chestID);
                player.chest = chestID;
                for (int i = 0; i < 40; i++) {
                    ItemSlot.SetGlow(i, -1f, chest: true);
                }

                player.chestX = x;
                player.chestY = y;
                player.SetTalkNPC(-1);
                Main.SetNPCShopIndex(0);

                UILinkPointNavigator.ForceMovementCooldown(120);
                if (PlayerInput.GrappleAndInteractAreShared)
                    PlayerInput.Triggers.JustPressed.Grapple = false;

                if (sound is null) {
                    SoundEngine.PlaySound(SoundID.MenuOpen);
                }
                else {
                    SoundEngine.PlaySound(sound.Value);
                }
            }
            Main.playerInventory = true;
            Recipe.FindRecipes();
        }

        /// <summary>
        /// 尝试破坏物块，需要有镐子，并且挖的动。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool TryKillTile(int x, int y, Player player) {
            Tile tile = Main.tile[x, y];
            if (tile.HasTile && !Main.tileHammer[Main.tile[x, y].TileType]) {
                if (player.HasEnoughPickPowerToHurtTile(x, y) && WorldGen.CanKillTile(x, y)) {
                    if ((tile.TileType == 2 || tile.TileType == 477 || tile.TileType == 492 || tile.TileType == 23 || tile.TileType == 60 || tile.TileType == 70 || tile.TileType == 109 || tile.TileType == 199 || Main.tileMoss[tile.TileType] || TileID.Sets.tileMossBrick[tile.TileType])) {
                        player.PickTile(x, y, 10000);
                    }
                    player.PickTile(x, y, 10000);
                }
            }
            return !Main.tile[x, y].HasTile;
        }

        /// <summary>
        /// 你猜干嘛用的，bongbong！！！
        /// </summary>
        /// <param name="position"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void BongBong(Vector2 position, int width, int height) {
            if (Main.rand.NextBool(6)) {
                Gore.NewGore(null, position + new Vector2(Main.rand.Next(16), Main.rand.Next(16)), Vector2.Zero, Main.rand.Next(61, 64));
            }
            if (Main.rand.NextBool(2)) {
                int index = Dust.NewDust(position, width, height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                Main.dust[index].velocity *= 1.4f;
            }
            if (Main.rand.NextBool(3)) {
                int index = Dust.NewDust(position, width, height, DustID.Torch, 0f, 0f, 100, default, 2.5f);
                Main.dust[index].noGravity = true;
                Main.dust[index].velocity *= 5f;
                index = Dust.NewDust(position, width, height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[index].velocity *= 3f;
            }
        }

        /// <summary>
        /// 先炸掉，然后再放物块
        /// </summary>
        public static bool BongBongPlace(int i, int j, Item item, Player player, bool mute = false, bool forced = false, bool playSound = false) {
            // 物块魔杖特判    
            if (item.tileWand > 0) {
                if (CheckWandUsability(item, player, out int index) && index != -1)
                    TryConsumeItem(ref player.inventory[index], player, true);
                else return false;
            }

            TryKillTile(i, j, player);
            bool success = WorldGen.PlaceTile(i, j, item.createTile, mute, forced, player.whoAmI, item.placeStyle);
            if (success) {
                BongBong(new Vector2(i, j) * 16f, 16, 16);
                if (playSound) {
                    SoundEngine.PlaySound(SoundID.Item14, Main.MouseWorld);
                }
            }
            return success;
        }

        /// <summary>
        /// 判断物块是否相同
        /// </summary>
        public static bool NotSameTile(int i, int j, int tileType, int tileStyle) {
            return (Main.tile[i, j].TileType == tileType && Main.tile[i, j].TileFrameY != tileStyle * 18)
                             || Main.tile[i, j].TileType != tileType;
        }

        /// <summary>
        /// 魔法移除物块方法
        /// </summary>
        public static void NormalKillTiles(Player player, Rectangle rect, Action<int, int> removeAction = null) {
            // 获得背包中最好的镐子
            Item item = player.GetBestPickaxe();
            int minI = rect.X;
            int maxI = rect.X + rect.Width - 1;
            int minJ = rect.Y;
            int maxJ = rect.Y + rect.Height - 1;
            for (int i = 0; i < player.hitTile.data.Length; i++) {
                HitTile.HitTileObject hitTileObject = player.hitTile.data[i];
                hitTileObject.timeToLive = 10000;
            }

            for (int i = minI; i <= maxI; i++) {
                for (int j = minJ; j <= maxJ; j++) {
                    if (removeAction is not null) {
                        removeAction(i, j);
                    }
                    Tile tile = Main.tile[i, j];
                    if (!Main.tileAxe[tile.TileType] && !Main.tileHammer[tile.TileType]) {
                        player.PickTile(i, j, item != null ? item.pick : 1);
                        player.hitTile.data[player.hitTile.HitObject(i, j, 1)].timeToLive = 10000;
                    }
                }
            }
            for (int i = 0; i < player.hitTile.data.Length; i++) {
                HitTile.HitTileObject hitTileObject = player.hitTile.data[i];
                hitTileObject.timeToLive = 60;
            }
        }

        /// <summary>
        /// 遍历 Tile
        /// </summary>
        public static void ForeachTile(Rectangle rect, Action<int, int> action) {
            int minI = rect.X;
            int maxI = rect.X + rect.Width - 1;
            int minJ = rect.Y;
            int maxJ = rect.Y + rect.Height - 1;
            for (int i = minI; i <= maxI; i++) {
                for (int j = minJ; j <= maxJ; j++) {
                    action(i, j);
                }
            }
        }
    }
}
