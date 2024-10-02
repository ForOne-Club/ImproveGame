﻿using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UI.QuickShimmer;

public class ShimmerLootItemSlot : BaseItemSlot
{
    public readonly List<Item> Loots;
    public readonly int Index;

    public override Item Item
    {
        get
        {
            return Loots.IndexInRange(Index) ? Loots[Index] : AirItem;
        }
    }

    public ShimmerLootItemSlot(List<Item> items, int index)
    {
        this.Loots = items;
        Index = index;
        SetBaseItemSlotValues(true, true);
        SetSizePixels(43, 43);
        ItemIconMaxWidthAndHeight = 27;
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        if (Main.LocalPlayer.ItemAnimationActive) return;
        
        Main.playerInventory = true;

        if (Item.IsAir)
            return;

        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Loot(), Item, Item.stack);
        Item.TurnToAir();
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        ItemIconMaxWidthAndHeight = 27f * HoverTimer.Lerp(1f, 0.9f);
        ItemIconOffset = HoverTimer.Lerp(new Vector2(), GetInnerDimensions().Size() * 0.15f);
        ItemIconOffset.X *= 0;
        ItemIconResize = HoverTimer.Lerp(new Vector2(), -GetInnerDimensions().Size() * 0.15f);
        ItemColor = HoverTimer.Lerp(Color.White, Color.Gray);

        base.DrawSelf(spriteBatch);

        if (HoverTimer.Opening || HoverTimer.Closing || IsMouseHovering)
        {
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            Main.spriteBatch.End();

            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            // 此处设置是为了确实中间在不使用 SpriteBatch 的合批绘制模式的时候 RasterizerState 失效
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;

            Main.spriteBatch.Begin(0, BlendState.AlphaBlend,
                SamplerState.LinearClamp, DepthStencilState.Default,
                rasterizerState, null, Main.UIScaleMatrix);

            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensionsSize();

            spriteBatch.Draw(ModAsset.Open.Value, pos + size / 3f * new Vector2(2, 1), null,
                HoverTimer.Lerp(Color.Transparent, Color.White), 0f,
                ModAsset.Open.Size() / 2f, HoverTimer.Lerp(0.6f, 0.75f), 0, 0);
        }
    }
}
