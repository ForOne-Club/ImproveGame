namespace ImproveGame.Common.RenderTargetContents;

public class StackNumberContent (string number) : ARenderTargetContentByRequest
{
    private string _number = number;

    public override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        var font = FontAssets.ItemStack.Value;
        var size = font.MeasureString(_number);
        
        PrepareARenderTarget_AndListenToEvents(ref _target, device, (int) size.X + 3, (int) size.Y + 3, RenderTargetUsage.PreserveContents);
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);
        
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        spriteBatch.DrawItemStackString(_number, new Vector2(1.5f), 1);
        spriteBatch.End();
        
        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
}