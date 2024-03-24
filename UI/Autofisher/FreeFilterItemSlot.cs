using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Tiles;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.Autofisher;

public class FreeFilterItemSlot : GenericItemSlot
{
    private readonly AnimationTimer _disableAnimTimer = new (3);

    public static TEAutofisher Autofisher => AutofishPlayer.LocalPlayer.Autofisher;

    public FreeFilterItemSlot(List<Item> items, int index) : base(items, index)
    {
        ItemIconScale = 0.8f;

        SetBaseItemSlotValues(true, false);
        SetSizePixels(44f, 44f);
        
        bool disabled = Autofisher.ExcludedItems.Any(i => ItemExtensions.IsSameItem(i.Item, Item));
        if (disabled)
            _disableAnimTimer.Open();
        else
            _disableAnimTimer.Close();
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        if (Autofisher is null)
            return;
        Autofisher.ToggleItem(Item);
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (Autofisher is null)
            return;
        bool disabled = Autofisher.ExcludedItems.Any(i => ItemExtensions.IsSameItem(i.Item, Item));
        if (disabled)
        {
            if (!_disableAnimTimer.AnyOpen)
                _disableAnimTimer.OpenAndResetTimer();
        }
        else
        {
            if (!_disableAnimTimer.AnyClose)
                _disableAnimTimer.CloseAndResetTimer();
        }

        _disableAnimTimer.Update();
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Autofisher is null)
            return;

        ItemIconMaxWidthAndHeight = 32f * _disableAnimTimer.Lerp(1f, 0.9f);
        ItemIconScale = HoverTimer.Lerp(0.8f, 0.94f);
        ItemColor = _disableAnimTimer.Lerp(Color.White, Color.Gray);
        ItemIconResize = _disableAnimTimer.Lerp(new Vector2(), -GetInnerDimensions().Size() * 0.1f);

        base.DrawSelf(spriteBatch);

        bool itemExcluded = Autofisher.ExcludedItems.Any(i => ItemExtensions.IsSameItem(i.Item, Item));
        if (_disableAnimTimer.Opening || _disableAnimTimer.Closing || itemExcluded)
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

            spriteBatch.Draw(ModAsset.DisabledItem.Value, pos + size * 0.7f, null,
                _disableAnimTimer.Lerp(Color.Transparent, Color.White), 0f,
                ModAsset.DisabledItem.Size() / 2f, _disableAnimTimer.Lerp(0.8f, 1f), 0, 0);
        }
    }
}