using ImproveGame.Common.Systems;
using ImproveGame.Content.Items;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.ConstructCore
{
    internal class PreviewRenderer : ModSystem
    {
        internal static RenderTarget2D PreviewTarget;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            On.Terraria.Main.SetDisplayMode += RefreshTarget;
            On.Terraria.Main.CheckMonoliths += DrawTarget;

            Main.RunOnMainThread(() => {
                PreviewTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false, default, default, default, RenderTargetUsage.PreserveContents);
            });
        }

        public override void Unload()
        {
            PreviewTarget = null;
        }

        private void RefreshTarget(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            if (!Main.gameInactive && (width != Main.screenWidth || height != Main.screenHeight))
                PreviewTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height, false, default, default, default, RenderTargetUsage.PreserveContents);

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
                        Main.spriteBatch.Draw(PreviewTarget, Vector2.Zero, color);
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

            position += Main.screenPosition;
            if (Main.LocalPlayer.gravDir is -1) // 为啥？我不到啊...
            {
                position.Y += 8f;
            }

            // 对齐左上角 + 转换到屏幕坐标
            Vector2 originPos = position.ToTileCoordinates().ToWorldCoordinates(0f, 0f) - Main.screenPosition;

            DrawPreviewFromTag(sb, tag, originPos, 1f, Main.LocalPlayer.gravDir is -1);
        }

        private void DrawTarget(On.Terraria.Main.orig_CheckMonoliths orig)
        {
            Main.spriteBatch.Begin();

            Main.graphics.GraphicsDevice.SetRenderTarget(PreviewTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            if (Main.LocalPlayer.HeldItem?.type == ModContent.ItemType<ConstructWand>() &&
                WandSystem.ConstructMode == WandSystem.Construct.Place &&
                !string.IsNullOrEmpty(WandSystem.ConstructFilePath) &&
                File.Exists(WandSystem.ConstructFilePath))
            {
                DrawStrcturePreview(Main.spriteBatch);
            }

            Main.spriteBatch.End();
            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            orig();
        }

        public static bool DrawPreviewFromTag(SpriteBatch sb, TagCompound tag, Vector2 origin, float scale = 1f, bool flip = false)
        {
            List<TileDefinition> data = (List<TileDefinition>)tag.GetList<TileDefinition>("StructureData");

            if (data is null || data.Count is 0)
            {
                // 此处应有Logger.Warn
                return false;
            }

            var spriteEffects = flip ? SpriteEffects.FlipVertically : SpriteEffects.None;

            var samplerState = Main.graphics.GraphicsDevice.SamplerStates[0];
            Main.graphics.GraphicsDevice.SamplerStates[0] = Main.DefaultSamplerState;

            Color color = Color.White;
            int width = tag.GetInt("Width");
            int height = tag.GetInt("Height");

            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    int index = y + x * (height + 1);

                    TileDefinition tileData = data[index];

                    if (!int.TryParse(tileData.Wall, out int wallType))
                    {
                        string[] parts = tileData.Wall.Split('/');
                        if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TryFind(parts[1], out ModWall modWallType))
                            wallType = modWallType.Type;

                        else wallType = 0;
                    }

                    int drawY = flip ? height - y : y;
                    var position = origin + new Point(x, drawY).ToWorldCoordinates(0f, flip ? 0f : 8f);

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

                    if (!int.TryParse(tileData.Tile, out int tileType))
                    {
                        string[] parts = tileData.Tile.Split('/');

                        if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TryFind(parts[1], out ModTile modTileType))
                            tileType = modTileType.Type;

                        else tileType = 0;
                    }

                    int drawY = flip ? height - y : y;
                    var position = origin + new Point(x, drawY).ToWorldCoordinates(0f, flip ? 0f : 8f);

                    if (tileType > 0) // Tile
                    {
                        Main.instance.LoadTiles(tileType);
                        Texture2D texture = TextureAssets.Tile[tileType].Value;
                        Rectangle? value = new Rectangle(tileData.TileFrameX, tileData.TileFrameY, 16, 16);
                        Vector2 pos = position;
                        sb.Draw(texture, pos * scale, value, color, 0f, new Vector2(0f, 8f), scale, spriteEffects, 0f);
                    }
                }
            }

            Main.graphics.GraphicsDevice.SamplerStates[0] = samplerState;
            return true;
        }
    }
}
