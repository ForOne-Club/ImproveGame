using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;

namespace ImproveGame.Common.RenderTargetContents;

public class ItemSlotContent(ThemeType theme) : ARenderTargetContentByRequest
{
    internal ThemeType Theme = theme;

    public override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        var size = new Vector2(40);

        PrepareARenderTarget_AndListenToEvents(ref _target, device, (int) size.X, (int) size.Y,
            RenderTargetUsage.PreserveContents);
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        var border = UIStyle.ItemSlotBorderSize;
        var round = new Vector4(UIStyle.ItemSlotBorderRound);
        var bgColor = UIStyle.ItemSlotBg;
        var borderColor = UIStyle.ItemSlotBorder;
        
        float uiScale = Main.UIScale;
        Main.UIScale = 1f;
        SDFRectangle.HasBorder(Vector2.Zero, size, round, bgColor, border, borderColor);
        Main.UIScale = uiScale;

        spriteBatch.End();

        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
}