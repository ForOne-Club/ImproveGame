using ImproveGame.Common.Players;
using ImproveGame.Content.Tiles;
using ImproveGame.Entitys;
using ImproveGame.Interface.GUI;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI;

namespace ImproveGame.Common.Systems
{
    /// <summary>
    /// 绘制方框
    /// </summary>
    public class DrawSystem : ModSystem
    {
        public override void Load() {
            On.Terraria.Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;
        }

        private void Main_DrawInterface_36_Cursor(On.Terraria.Main.orig_DrawInterface_36_Cursor orig) {
            if (Main.cursorOverride == 15) {
                // 修正鼠标坐标
                Main.mouseX -= 12;
                Main.mouseY -= 12;
                Main.cursorScale = 1f;
            }
            orig.Invoke();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
            if (MouseTextIndex != -1) {
                layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
                    "ImproveGame: BorderRect",
                    delegate {
                        DrawBorderRect();
                        // 鼠标显示物块信息
                        /*Point point = Main.MouseWorld.ToTileCoordinates();
                        if (Main.tile[point].HasTile) {
                            MyUtils.DrawString(Main.MouseScreen + new Vector2(30, 20), Main.tile[point].TileType.ToString(), Color.White, Color.Red);
                            MyUtils.DrawString(Main.MouseScreen + new Vector2(30, 40), Main.tile[point].TileFrameX.ToString(), Color.White, Color.Red);
                            MyUtils.DrawString(Main.MouseScreen + new Vector2(30, 60), (Main.tile[point].TileFrameX / 18).ToString(), Color.White, Color.Red);
                        }*/
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
                layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
                    "ImproveGame: Pools Select",
                    delegate {
                        Point16 fisherPos = AutofishPlayer.LocalPlayer.Autofisher;
                        if (fisherPos.X > 0 && fisherPos.Y > 0 && AutofisherGUI.Visible) {
                            DrawPoolsBorder();
                        }
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
        }

        private static void DrawPoolsBorder() {
            // 绘制现有定位点光标
            var autofisher = AutofishPlayer.LocalPlayer.GetAutofisher();
            if (autofisher is not null && autofisher.locatePoint.X > 0 && autofisher.locatePoint.Y > 0) {
                var position = autofisher.locatePoint.ToWorldCoordinates() - Main.screenPosition;
                var color = Color.SkyBlue;
                var tex = TextureAssets.Cursors[15].Value;
                var origin = tex.Size() / 2f;
                Main.spriteBatch.Draw(tex, position, null, color, 0f, origin, 0.8f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
            }

            if (!WandSystem.SelectPoolMode)
                return;

            if (Main.mouseRight && Main.mouseRightRelease) {
                UISystem.Instance.AutofisherGUI.ToggleSelectPool();
                return;
            }

            Main.cursorOverride = 15;
            Main.cursorColor = Color.SkyBlue;

            Vector2 fisherPos = AutofishPlayer.LocalPlayer.Autofisher.ToVector2();
            fisherPos.X += 1f; // 修正到物块中心
            fisherPos.Y += 1f; // 修正到物块中心

            int extraRangeX = 12;
            int extraRangeY = 10;
            Point screenOverdrawOffset = Main.GetScreenOverdrawOffset();
            Rectangle drawRange = new(((int)Main.screenPosition.X >> 4) - extraRangeX + screenOverdrawOffset.X,
                ((int)Main.screenPosition.Y >> 4) - extraRangeY + screenOverdrawOffset.Y,
                (Main.screenWidth >> 4) + (extraRangeX - screenOverdrawOffset.X << 1),
                (Main.screenHeight >> 4) + (extraRangeY - screenOverdrawOffset.Y << 1));

            // 这个是拿来记录悬停的相邻液体的
            bool[,] mouseHovering = new bool[drawRange.Width + 1, drawRange.Height + 1];
            // 递归可不能把已经检查过的重新检查，不然死循环了
            bool[,] tileChecked = new bool[drawRange.Width + 1, drawRange.Height + 1];
            Point16 mouseTilePosition = Main.MouseWorld.ToTileCoordinates16();
            RecursionSetHovered(mouseTilePosition.X, mouseTilePosition.Y);
            // 通过递归设置鼠标悬停的区域
            void RecursionSetHovered(int i, int j) {
                if (i < drawRange.Left || j < drawRange.Top || i > drawRange.Right || j > drawRange.Bottom || tileChecked[i - drawRange.X, j - drawRange.Y])
                    return;
                if (Math.Abs(fisherPos.X - i) > TEAutofisher.checkWidth || Math.Abs(fisherPos.Y - j) > TEAutofisher.checkHeight)
                    return;

                tileChecked[i - drawRange.X, j - drawRange.Y] = true;
                var tile = Framing.GetTileSafely(i, j);
                if (tile.LiquidAmount > 0 && !WorldGen.SolidTile(i, j) && i >= drawRange.Left && j >= drawRange.Top && i <= drawRange.Right && j <= drawRange.Bottom) {
                    mouseHovering[i - drawRange.X, j - drawRange.Y] = true;
                    Main.LocalPlayer.mouseInterface = true;
                    Main.cursorColor = new(50, 255, 50);
                    // 递归临近的四个物块
                    RecursionSetHovered(i - 1, j);
                    RecursionSetHovered(i + 1, j);
                    RecursionSetHovered(i, j - 1);
                    RecursionSetHovered(i, j + 1);
                    return;
                }
                return; // 虽然不是必要的，但是写上感觉规范点
            }

            for (int i = drawRange.Left; i < drawRange.Right; i++) {
                for (int j = drawRange.Top; j < drawRange.Bottom; j++) {
                    var tile = Framing.GetTileSafely(i, j);
                    if (tile.LiquidAmount > 0 && !WorldGen.SolidTile(i, j)) {
                        Vector2 worldPosition = new(i << 4, j << 4);

                        // 液体离格子顶部的高度
                        int liquidHeightToTileTop = 16 - (int)MathHelper.Lerp(0f, 16f, tile.LiquidAmount / 255f);
                        var blockDrawPosition = new Vector2(worldPosition.X, worldPosition.Y + liquidHeightToTileTop) - Main.screenPosition;
                        var rect = new Rectangle?(new Rectangle(0, 0, 16, 16 - liquidHeightToTileTop));

                        bool selectable = true;
                        float opacity = 0.2f;
                        Color color = Color.SkyBlue;
                        if (mouseHovering[i - drawRange.X, j - drawRange.Y])
                            color = new(50, 255, 50);
                        // 不在可选范围内，黑白
                        if (Math.Abs(fisherPos.X - i) > TEAutofisher.checkWidth || Math.Abs(fisherPos.Y - j) > TEAutofisher.checkHeight) {
                            color = new(60, 60, 60);
                            opacity = 0.3f;
                            selectable = false;
                        }
                        if (Lighting.Brightness(i, j) < 0.001f)
                            color = Color.Transparent;
                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, blockDrawPosition, rect, color * opacity);

                        // 画边缘线，旁边不是可用水体或者是可选不可选分界线时
                        var leftTile = Framing.GetTileSafely(i - 1, j);
                        var rightTile = Framing.GetTileSafely(i + 1, j);
                        var upTile = Framing.GetTileSafely(i, j - 1);
                        var bottomTile = Framing.GetTileSafely(i, j + 1);
                        if (leftTile.LiquidAmount == 0 || WorldGen.SolidTile(leftTile) || selectable && (Math.Abs(fisherPos.X - (i - 1)) > TEAutofisher.checkWidth || Math.Abs(fisherPos.Y - j) > TEAutofisher.checkHeight)) {
                            Terraria.Utils.DrawLine(Main.spriteBatch, new Vector2(worldPosition.X, worldPosition.Y + liquidHeightToTileTop), new Vector2(worldPosition.X, worldPosition.Y + 18), color, color, 2f);
                        }
                        if (rightTile.LiquidAmount == 0 || WorldGen.SolidTile(rightTile) || selectable && (Math.Abs(fisherPos.X - (i + 1)) > TEAutofisher.checkWidth || Math.Abs(fisherPos.Y - j) > TEAutofisher.checkHeight)) {
                            Terraria.Utils.DrawLine(Main.spriteBatch, new Vector2(worldPosition.X + 16, worldPosition.Y + liquidHeightToTileTop), new Vector2(worldPosition.X + 16, worldPosition.Y + 18), color, color, 2f);
                        }
                        if (upTile.LiquidAmount == 0 || WorldGen.SolidTile(upTile) || selectable && (Math.Abs(fisherPos.X - i) > TEAutofisher.checkWidth || Math.Abs(fisherPos.Y - (j - 1)) > TEAutofisher.checkHeight)) {
                            Terraria.Utils.DrawLine(Main.spriteBatch, new Vector2(worldPosition.X, worldPosition.Y + liquidHeightToTileTop), new Vector2(worldPosition.X + 16, worldPosition.Y + liquidHeightToTileTop), color, color, 2f);
                        }
                        if (bottomTile.LiquidAmount == 0 || WorldGen.SolidTile(bottomTile)) {
                            Terraria.Utils.DrawLine(Main.spriteBatch, new Vector2(worldPosition.X, worldPosition.Y + 16), new Vector2(worldPosition.X + 16, worldPosition.Y + 16), color, color, 2f);
                        }
                        // 下面的是不可选的，分界线要在下面的绘制（也就是我不可选，我上面的可选）不然会被覆盖，我希望分界线是绿色的
                        if (!selectable && upTile.LiquidAmount > 0 && !WorldGen.SolidTile(upTile) && Math.Abs(fisherPos.X - i) <= TEAutofisher.checkWidth && Math.Abs(fisherPos.Y - (j - 1)) <= TEAutofisher.checkHeight) {
                            color = Color.SkyBlue;
                            if (mouseHovering[i - drawRange.X, j - 1 - drawRange.Y])
                                color = new(50, 255, 50);
                            if (Lighting.Brightness(i, j - 1) < 0.02f)
                                color = Color.Transparent;
                            Terraria.Utils.DrawLine(Main.spriteBatch, new Vector2(worldPosition.X - 2, worldPosition.Y), new Vector2(worldPosition.X + 16, worldPosition.Y), color, color, 2f);
                        }

                        if (mouseHovering[i - drawRange.X, j - drawRange.Y] && Main.mouseLeft && Main.mouseLeftRelease) {
                            AutofishPlayer.LocalPlayer.SetLocatePoint(AutofishPlayer.LocalPlayer.GetAutofisher(), mouseTilePosition);
                            UISystem.Instance.AutofisherGUI.ToggleSelectPool();
                            if (Main.netMode == NetmodeID.MultiplayerClient) {
                                NetAutofish.ClientSendLocatePoint(AutofishPlayer.LocalPlayer.Autofisher, mouseTilePosition);
                            }
                            return;
                        }
                    }
                }
            }
        }

        public static readonly Box[] boxs = new Box[10];

        private static void DrawBorderRect() {
            for (int i = 0; i < boxs.Length; i++) {
                if (boxs[i] != null) {
                    Box box = boxs[i];
                    if (box.PreView is not null) {
                        Main.spriteBatch.Draw(box.PreView, new Vector2(box.X, box.Y) * 16f - Main.screenPosition, null, Color.White * 0.5f, 0, Vector2.Zero, 1f, 0, 0);
                    }

                    box.Draw();

                    string text = "";
                    if (box.ShowWidth)
                        text = box.Width.ToString();
                    if (box.ShowHeight)
                        text = box.ShowWidth ? $"{box.Width}×{box.Height}" : box.Height.ToString();
                    Vector2 size = FontAssets.MouseText.Value.MeasureString(box.Width.ToString()) * 1.2f;
                    Vector2 position = Main.MouseScreen + new Vector2(16, -size.Y + 6);
                    Terraria.Utils.DrawBorderString(Main.spriteBatch, text, position, box.borderColor, 1.2f);

                    boxs[i] = null;
                }
            }
        }
    }
}
