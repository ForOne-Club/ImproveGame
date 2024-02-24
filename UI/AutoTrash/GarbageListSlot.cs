using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.AutoTrash;

/// <summary>
/// 垃圾列表槽
/// </summary>
public class GarbageListSlot : GenericItemSlot
{
    public static AutoTrashPlayer AutoTrashPlayer =>
        Main.LocalPlayer.GetModPlayer<AutoTrashPlayer>();
    public static Texture2D TrashTexture => ModAsset.TakeOutFromTrash.Value;

    public GarbageListSlot(List<Item> items, int index) : base(items, index)
    {
        HoverTimer.Close();
        ItemIconScale = 0.85f;

        SetBaseItemSlotValues(true, false);
        SetSizePixels(44f, 44f);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        List<Item> recentlyThrownAwayItems = AutoTrashPlayer.Instance.RecentlyThrownAwayItems;

        int trashIndex = recentlyThrownAwayItems.FindIndex(i => i.type == Item.type);
        if (trashIndex > -1)
        {
            Main.LocalPlayer.QuickSpawnItem(null, Item.type, Item.stack);
        }

        AutoTrashPlayer.RemoveItem(Item);
        SoundEngine.PlaySound(SoundID.Grab);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        ItemIconMaxWidthAndHeight = 32f * HoverTimer.Lerp(1f, 0.9f);
        ItemIconOffset = HoverTimer.Lerp(new Vector2(), GetInnerDimensions().Size() * 0.15f);
        ItemIconOffset.X *= 0;
        ItemIconResize = HoverTimer.Lerp(new Vector2(), -GetInnerDimensions().Size() * 0.15f);

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

            Vector2 boxSize = (size - new Vector2(8f)) * HoverTimer.Lerp(0.5f, 1f);

            // SDFRectangle.NoBorder(pos + (size - boxSize) / 2f, boxSize, new Vector4(8f), HoverTimer.Lerp(Color.Transparent, UIColor.TitleBg2 * 0.75f));
            spriteBatch.Draw(TrashTexture, pos + size / 3f * new Vector2(2, 1), null,
                HoverTimer.Lerp(Color.Transparent, Color.White), 0f,
                TrashTexture.Size() / 2f, HoverTimer.Lerp(0.5f, 0.65f), 0, 0);
        }
    }
}
