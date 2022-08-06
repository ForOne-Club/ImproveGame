using ImproveGame.Common.Systems;
using ImproveGame.Content.Items;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace ImproveGame.Common.ConstructCore
{
    internal class PreviewRenderer : ModSystem
    {
        internal static RenderTarget2D PreviewTarget;
        internal static bool ResetPreviewTarget;
        private bool _canDrawLastFrame;

        internal static string UIPreviewPath;
        internal static RenderTarget2D UIPreviewTarget; // 不知道为什么，Preview绘制就不会裂开
        internal static bool ResetUIPreviewTarget;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            On.Terraria.Main.SetDisplayMode += RefreshTarget;
            On.Terraria.Main.CheckMonoliths += DrawTarget;
        }

        public override void Unload()
        {
            PreviewTarget = null;
            UIPreviewTarget = null;
        }

        private void RefreshTarget(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            orig(width, height, fullscreen);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int rulerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
            if (rulerIndex != -1)
            {
                layers.Insert(rulerIndex, new LegacyGameInterfaceLayer("ImproveGame: Strcture Preview", delegate
                {
                    if (Main.LocalPlayer.HeldItem?.type == ModContent.ItemType<ConstructWand>() &&
                        WandSystem.ConstructMode == WandSystem.Construct.Place &&
                        !string.IsNullOrEmpty(WandSystem.ConstructFilePath) &&
                        File.Exists(WandSystem.ConstructFilePath))
                    {
                        var color = Color.White * 0.6f;
                        var drawPosition = Main.MouseWorld;
                        drawPosition = drawPosition.ToTileCoordinates().ToWorldCoordinates(0f, 0f) - Main.screenPosition;
                        var spriteEffects = Main.LocalPlayer.gravDir is -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

                        var tag = FileOperator.GetTagFromFile(WandSystem.ConstructFilePath);

                        if (tag is null)
                            return true;

                        // 添加Origin偏移
                        drawPosition.X -= tag.GetInt("OriginX") * 16f;
                        drawPosition.Y -= tag.GetInt("OriginY") * 16f;
                        if (Main.LocalPlayer.gravDir is -1) // 做一个重力转换
                        {
                            float oppositeY = (drawPosition.Y - Main.screenHeight / 2);
                            drawPosition.Y -= oppositeY * 2f;
                            drawPosition.Y -= PreviewTarget.Height; // 由于FlipVertically特性，这里要调回来
                        }
                        drawPosition -= new Vector2(2f, 2f * Main.LocalPlayer.gravDir);

                        Main.spriteBatch.Draw(PreviewTarget, drawPosition, null, color, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
                    }
                    return true;
                }, InterfaceScaleType.Game));
            }
        }

        private static void DrawStrcturePreview(SpriteBatch sb)
        {
            var tag = FileOperator.GetTagFromFile(WandSystem.ConstructFilePath);

            if (tag is null)
                return;

            // 缩放适配（碰巧碰出来的）
            var position = Main.MouseScreen;
            float oppositeX = (position.X - Main.screenWidth / 2) / Main.GameZoomTarget;
            float oppositeY = (position.Y - Main.screenHeight / 2) / Main.GameZoomTarget;
            position.X -= oppositeX * (Main.GameZoomTarget - 1f);
            position.Y -= oppositeY * (Main.GameZoomTarget - 1f);
            if (Main.LocalPlayer.gravDir is -1) // 不知道为啥
            {
                position.Y -= 8f;
            }

            position += Main.screenPosition;

            int width = tag.GetInt("Width");
            int height = tag.GetInt("Height");
            var color = Color.GreenYellow;

            // 对齐左上角 + 转换到屏幕坐标
            Vector2 originPos = position.ToTileCoordinates().ToWorldCoordinates(0f, 0f) - Main.screenPosition;
            // 添加Origin偏移
            originPos.X -= tag.GetInt("OriginX") * 16f;
            originPos.Y -= tag.GetInt("OriginY") * 16f * Main.LocalPlayer.gravDir;
            if (Main.LocalPlayer.gravDir is -1) // 做一个重力转换
            {
                oppositeY = (originPos.Y - Main.screenHeight / 2);
                originPos.Y -= oppositeY * 2f;
                originPos.Y -= 24f; // 不知道啊不知道
            }

            DrawBorder(originPos, (width + 1) * 16f, (height + 1) * 16f, color * 0.35f, color); // 背景边框
            DrawPreviewFromTag(sb, tag, originPos, 1f);
        }

        private void DrawTarget(On.Terraria.Main.orig_CheckMonoliths orig)
        {
            bool canDraw = Main.LocalPlayer.HeldItem?.type == ModContent.ItemType<ConstructWand>() &&
                WandSystem.ConstructMode == WandSystem.Construct.Place;
            if (((canDraw && !_canDrawLastFrame) || ResetPreviewTarget) && !string.IsNullOrEmpty(WandSystem.ConstructFilePath) && File.Exists(WandSystem.ConstructFilePath))
            {
                if (PreviewTarget is null)
                {
                    var tag = FileOperator.GetTagFromFile(WandSystem.ConstructFilePath);

                    if (tag is not null)
                    {
                        int width = tag.GetInt("Width");
                        int height = tag.GetInt("Height");
                        PreviewTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width * 16 + 20, height * 16 + 20, false, default, default, default, RenderTargetUsage.PreserveContents);
                    }
                }
                if (PreviewTarget is not null)
                {
                    DrawPreviewToRender(PreviewTarget, WandSystem.ConstructFilePath);
                    ResetPreviewTarget = false;
                }
            }
            _canDrawLastFrame = canDraw;

            if (!string.IsNullOrEmpty(UIPreviewPath) && File.Exists(UIPreviewPath) && ResetUIPreviewTarget)
            {
                DrawPreviewToRender(UIPreviewTarget, UIPreviewPath);
                ResetUIPreviewTarget = false;
            }

            orig();
        }

        private static void DrawPreviewToRender(RenderTarget2D renderTarget, string filePath)
        {
            Main.spriteBatch.Begin();

            Main.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            var tag = FileOperator.GetTagFromFile(filePath);

            if (tag is null)
            {
                Main.spriteBatch.End();
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
                return;
            }

            int width = tag.GetInt("Width");
            int height = tag.GetInt("Height");
            var color = Color.GreenYellow;
            var position = Vector2.Zero + new Vector2(2f, 2f);
            DrawBorder(position, (width + 1) * 16f, (height + 1) * 16f, color * 0.35f, color); // 背景边框
            DrawPreviewFromTag(Main.spriteBatch, tag, position, 1f);

            Main.spriteBatch.End();
            Main.graphics.GraphicsDevice.SetRenderTarget(null);
        }

        public static bool DrawPreviewFromTag(SpriteBatch sb, TagCompound tag, Vector2 origin, float scale = 1f)
        {
            List<TileDefinition> data = (List<TileDefinition>)tag.GetList<TileDefinition>("StructureData");

            if (data is null || data.Count is 0)
            {
                // 此处应有Logger.Warn
                return false;
            }

            var spriteEffects = SpriteEffects.None;

            Color color = Color.White;
            int width = tag.GetInt("Width");
            int height = tag.GetInt("Height");

            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    int index = y + x * (height + 1);

                    TileDefinition tileData = data[index];

                    int wallType = FileOperator.ParseWallType(tileData.Wall);

                    var position = origin + new Point(x, y).ToWorldCoordinates(0f, 8f);

                    if (wallType > 0) // Wall
                    {
                        Main.instance.LoadWall(wallType);
                        Texture2D textureWall = TextureAssets.Wall[wallType].Value;

                        int wallFrame = Main.wallFrame[wallType] * 180;
                        Rectangle value = new(tileData.WallFrameX, tileData.WallFrameY + wallFrame, 32, 32);
                        Vector2 pos = position - new Vector2(8f, 8f);
                        sb.Draw(textureWall, pos * scale, value, color, 0f, new Vector2(0f, 8f), scale, spriteEffects, 0f);
                    }
                }
            }

            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    int index = y + x * (height + 1);

                    TileDefinition tileData = data[index];

                    int tileType = FileOperator.ParseTileType(tileData.Tile);

                    var position = origin + new Point(x, y).ToWorldCoordinates(0f, 8f);

                    // 这绘制...不想再写第二遍了
                    if (tileType != -1) // Tile
                    {
                        Main.instance.LoadTiles(tileType);
                        Texture2D texture = TextureAssets.Tile[tileType].Value;
                        var normalTileRect = new Rectangle(tileData.TileFrameX, tileData.TileFrameY, 16, 16);

                        int tileItemType = GetTileItem(tileType, tileData.TileFrameX, tileData.TileFrameY);
                        TileObjectData tileObjectData = null;
                        if (tileItemType is not -1)
                        {
                            tileObjectData = TileObjectData.GetTileData(tileType, MaterialCore.ItemToPlaceStyle[tileItemType]);
                        }
                        bool multiTile = tileObjectData is not null && (tileObjectData.CoordinateFullWidth > 18 || tileObjectData.CoordinateFullHeight > 18);

                        if (tileData.BlockType is not BlockType.HalfBlock or BlockType.Solid && !multiTile)
                        {
                            if (TileID.Sets.Platforms[tileType])
                            {
                                if (tileData.PlatformSlopeDrawType is 1 or 2)
                                {
                                    Rectangle value = new(198, tileData.TileFrameY, 16, 16);
                                    if (tileData.PlatformSlopeDrawType is 2)
                                        value.X = 324;

                                    Main.spriteBatch.Draw(texture, (position + new Vector2(0f, 16f)) * scale, value, color, 0f, new Vector2(0f, 8f), scale, spriteEffects, 0f);
                                }
                                else if (tileData.PlatformSlopeDrawType is 3 or 4)
                                {
                                    Rectangle value = new(162, tileData.TileFrameY, 16, 16);
                                    if (tileData.PlatformSlopeDrawType is 4)
                                        value.X = 306;

                                    Main.spriteBatch.Draw(texture, (position + new Vector2(0f, 16f)) * scale, value, color, 0f, new Vector2(0f, 8f), scale, spriteEffects, 0f);
                                }
                                goto EndSlopeDraw;
                            }
                            if (TileID.Sets.HasSlopeFrames[tileType])
                            {
                                Main.spriteBatch.Draw(texture, position * scale, normalTileRect, color, 0f, new Vector2(0f, 8f), scale, spriteEffects, 0f);
                                continue;
                            }

                            int num2 = 2;
                            for (int i = 0; i < 8; i++)
                            {
                                int num3 = i * -2;
                                int num4 = 16 - i * 2;
                                int num5 = 16 - num4;
                                int num6;
                                switch (tileData.BlockType)
                                {
                                    case BlockType.SlopeDownLeft:
                                        num3 = 0;
                                        num6 = i * 2;
                                        num4 = 14 - i * 2;
                                        num5 = 0;
                                        break;
                                    case BlockType.SlopeDownRight:
                                        num3 = 0;
                                        num6 = 16 - i * 2 - 2;
                                        num4 = 14 - i * 2;
                                        num5 = 0;
                                        break;
                                    case BlockType.SlopeUpLeft:
                                        num6 = i * 2;
                                        break;
                                    default:
                                        num6 = 16 - i * 2 - 2;
                                        break;
                                }

                                sb.Draw(texture, (position + new Vector2(num6, i * num2 + num3)) * scale, new Rectangle(tileData.TileFrameX + num6, tileData.TileFrameY + num5, num2, num4), color, 0f, new Vector2(0f, 8f), scale, SpriteEffects.None, 0f);
                            }
                        }
                        EndSlopeDraw:;

                        if (!multiTile)
                        {
                            int num7 = ((int)tileData.BlockType <= 3) ? 14 : 0;
                            sb.Draw(texture, (position + new Vector2(0f, num7)) * scale, new Rectangle(tileData.TileFrameX, tileData.TileFrameY + num7, 16, 2), color, 0f, new Vector2(0f, 8f), scale, spriteEffects, 0f);
                        }

                        if (tileData.BlockType is BlockType.HalfBlock)
                        {
                            position.Y += 8f;
                            sb.Draw(texture, (position + new Vector2(0f, 4f)) * scale, new Rectangle(144, 66, 16, 4), color, 0f, new Vector2(0f, 8f), scale, spriteEffects, 0f);
                            sb.Draw(texture, position * scale, normalTileRect.Modified(0, 0, 0, -4), color, 0f, new Vector2(0f, 8f), scale, spriteEffects, 0f);
                        }
                        if (tileData.BlockType is BlockType.Solid || TileID.Sets.Platforms[tileType] || multiTile)
                        {
                            sb.Draw(texture, position * scale, normalTileRect, color, 0f, new Vector2(0f, 8f), scale, spriteEffects, 0f);
                        }
                    }
                }
            }
            return true;
        }
    }
}
