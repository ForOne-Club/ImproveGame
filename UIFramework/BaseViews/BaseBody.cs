
using Terraria.GameInput;

namespace ImproveGame.UIFramework.BaseViews;

public abstract class BaseBody : View
{
    protected BaseBody()
    {
        Width.Percent = Height.Percent = 1f;
        Recalculate();
    }

    /// <summary>
    /// 启用 body
    /// </summary>
    public abstract bool Enabled { get; set; }

    /// <summary>
    /// 是否不可选中
    /// </summary>
    public virtual bool IsNotSelectable => false;

    /// <summary>
    /// 鼠标在当前 UI 某一个元素上时调用此方法，返回 <see langword="true"/> 此元素会占用光标，防止下层 UI 触发鼠标事件
    /// </summary>
    public abstract bool CanSetFocusTarget(UIElement target);

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        CheckWhetherRecalculate(out bool recalculate);

        if (recalculate)
        {
            Recalculate();
        }
    }

    private Vector2 _lastScreenSize = PlayerInput.OriginalScreenSize / Main.UIScale;
    /// <summary>
    /// 总是执行
    /// </summary>
    public virtual void CheckWhetherRecalculate(out bool recalculate)
    {
        recalculate = false;

        Vector2 currentScreenSize = PlayerInput.OriginalScreenSize / Main.UIScale;
        if (currentScreenSize != _lastScreenSize)
        {
            recalculate = true;
            _lastScreenSize = currentScreenSize;
        }
    }

    public virtual bool RenderTarget2DDraw => false;
    public virtual float RenderTarget2DOpacity => 1f;
    public virtual Vector2 RenderTarget2DScale => Vector2.One;
    public virtual Vector2 RenderTarget2DPosition => Vector2.Zero;
    public virtual Vector2 RenderTarget2DOrigin => Vector2.Zero;

    public override void Draw(SpriteBatch spriteBatch)
    {
        var rt2dPool = ImproveGame.Instance.RenderTargetPool;
        var device = Main.graphics.GraphicsDevice;

        if (RenderTarget2DDraw)
        {
            var originalRT2Ds = device.GetRenderTargets();

            if (originalRT2Ds != null)
            {
                foreach (var item in originalRT2Ds)
                {
                    if (item.renderTarget is RenderTarget2D rt)
                    {
                        rt.RenderTargetUsage = RenderTargetUsage.PreserveContents;
                    }
                }
            }

            // 刺客获取的屏幕大小不是正常的
            var rt2d = rt2dPool.Borrow((int)Math.Round(Main.screenWidth * Main.UIScale),
                (int)Math.Round(Main.screenHeight * Main.UIScale));

            var lastRenderTargetUsage = device.PresentationParameters.RenderTargetUsage;
            device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

            device.SetRenderTarget(rt2d);
            device.Clear(Color.Transparent);

            base.Draw(spriteBatch);

            spriteBatch.End();
            device.SetRenderTargets(originalRT2Ds);

            // 使用默认矩阵，因为图像已经是根据 UIZoom 矩阵 绘制的了。
            spriteBatch.Begin();
            spriteBatch.Draw(rt2d, RenderTarget2DPosition * Main.UIScale, null,
                Color.White * RenderTarget2DOpacity, 0f, RenderTarget2DOrigin * Main.UIScale, RenderTarget2DScale, 0, 0);

            device.PresentationParameters.RenderTargetUsage = lastRenderTargetUsage;

            rt2dPool.Return(rt2d);
        }
        else
        {
            base.Draw(spriteBatch);
        }
    }
}