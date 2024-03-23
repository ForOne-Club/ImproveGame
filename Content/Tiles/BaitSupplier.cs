using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace ImproveGame.Content.Tiles;

public class BaitSupplier : TETileBase
{
    public static float GlowFactor
    {
        get
        {
            // 这个计时是宝藏袋源码，实现渐变
            float glowFactor = Main.GlobalTimeWrappedHourly;

            glowFactor %= 4f;
            glowFactor /= 2f;

            if (glowFactor >= 1f)
            {
                glowFactor = 2f - glowFactor;
            }

            glowFactor = glowFactor * 0.6f + 0.6f;
            return glowFactor;
        }
    }

    public override ModTileEntity GetTileEntity() => ModContent.GetInstance<TEBaitSupplier>();

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

    public int ItemType() => ModContent.ItemType<Items.Placeable.BaitSupplier>();

    #region 基本物块信息

    public override int Hook_AfterPlacement_NoEntity(int i, int j, int type, int style, int direction, int alternate)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NetMessage.SendTileSquare(Main.myPlayer, i - 0, j - 2, 2, 3);
        }

        return 0;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ItemType();
        player.noThrow = 2;
    }

    public override bool RightClick(int i, int j) {
        SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
        ToggleTile(i, j);
        return true;
    }

    public override void HitWire(int i, int j)
    {
        ToggleTile(i, j);
    }

    public void ToggleTile(int i, int j)
    {
        Tile tile = Main.tile[i, j];
        int topX = i - tile.TileFrameX % 36 / 18;
        int topY = j - tile.TileFrameY % 54 / 18;

        short frameAdjustment = (short)(tile.TileFrameY >= 54 ? -54 : 54);

        for (int x = topX; x < topX + 2; x++)
        {
            for (int y = topY; y < topY + 3; y++)
            {
                Main.tile[x, y].TileFrameY += frameAdjustment;

                if (Wiring.running)
                {
                    Wiring.SkipWire(x, y);
                }
            }
        }

        if (Main.netMode != NetmodeID.SinglePlayer)
            NetMessage.SendTileSquare(-1, topX, topY, 2, 3);
    }

    public override void ModifyObjectData()
    {
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.Origin = new Point16(0, 2);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
        // TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
    }

    public override bool ModifyObjectDataAlternate(ref int alternateStyle)
    {
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
        alternateStyle = 1;
        return true;
    }

    // 不能同时放置多个
    // 又感觉没啥必要，还是不要了
    // public int PlacementPreviewHook_CheckIfCanPlace(int x, int y, int type, int style = 0, int direction = 1, int alternate = 0)
    // {
    //     return TileEntity.ByID.All(pair =>
    //         pair.Value is not TEBaitSupplier tileEntity ||
    //         !WorldGen.InWorld(tileEntity.Position.X, tileEntity.Position.Y, 10) ||
    //         !Main.tile[tileEntity.Position].HasTile ||
    //         Main.tile[tileEntity.Position].TileType != Type)
    //         ? 0 : 1;
    // }

    public override IEnumerable<Item> GetItemDrops(int i, int j)
    {
        yield return new Item(ItemType());
    }

    #endregion

    #region 绘制

    public override void AnimateTile(ref int frame, ref int frameCounter)
    {
        if (++frameCounter >= 12)
        {
            frameCounter = 0;
            frame = ++frame % 10;
        }
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
    {
        var tile = Main.tile[i, j];
        if (tile.TileFrameY < 54)
        {
            frameYOffset = 0;
        }
        else
        {
            frameYOffset = Main.tileFrame[type] * 54;
        }
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        var tile = Main.tile[i, j];

        if (!TileDrawing.IsVisible(tile))
            return;

        if (tile.TileFrameY >= 54)
        {
            Color color = Color.White;

            Vector2 offscreenVector = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                offscreenVector = Vector2.Zero;

            int width = 16;
            int offsetY = 0;
            int height = 16;
            short frameX = tile.TileFrameX;
            short frameY = tile.TileFrameY;
            int addFrX = 0;
            int addFrY = 0;

            TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX,
                ref frameY); // calculates the draw offsets
            TileLoader.SetAnimationFrame(Type, i, j, ref addFrX, ref addFrY);

            Rectangle drawRectangle = new(tile.TileFrameX, tile.TileFrameY + addFrY, 16, 16);

            spriteBatch.Draw(ModAsset.BaitSupplier_Glow.Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + offsetY) +
                offscreenVector,
                drawRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // 智能光标，要多画一层覆盖
            Color lightColor = Lighting.GetColor(i, j);
            if (tile.IsTileFullbright) lightColor = Color.White;

            int selectionLevel = 0;
            if (Main.InSmartCursorHighlightArea(i, j, out bool actuallySelected))
            {
                selectionLevel = 1;
                if (actuallySelected)
                {
                    selectionLevel = 2;
                }
            }

            if (selectionLevel == 0)
                return;

            int averageBrightness = (lightColor.R + lightColor.G + lightColor.B) / 3;

            if (averageBrightness <= 10)
                return;

            Color selectionGlowColor = Colors.GetSelectionGlowColor(selectionLevel == 2, averageBrightness);
            spriteBatch.Draw(ModAsset.BaitSupplier_Highlight.Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + offsetY) +
                offscreenVector,
                drawRectangle, selectionGlowColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }

    #endregion
}