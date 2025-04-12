namespace ImproveGame.UIFramework;

/// <summary>
/// 模糊效果处理系统，继承自 ModSystem
/// 负责创建和管理模糊渲染目标
/// </summary>
public class BlurMakeSystem : ModSystem
{
    public static bool EnableBlur { get; internal set; } = true;

    /// <summary>
    /// 单点模糊, 每个 UI 都单独计算模糊
    /// </summary>
    public static bool SingleBlur { get; internal set; } = true;

    /// <summary>
    /// 开启复古和迷幻效果后将不可用
    /// </summary>
    public static bool BlurAvailable => !Main.gameMenu && EnableBlur && Lighting.NotRetro;

    public static float BlurZoomMultiplierDenominator
    {
        get; internal set => field = Math.Max(value, 1f);
    } = 4f;

    /// <summary> 模糊迭代次数 </summary>
    public static int BlurIterationCount { get; internal set; } = 3;

    /// <summary> 模糊偏移乘数 </summary>
    public static float BlurIterationOffsetMultiplier { get; internal set; } = 1f;
    public static BlurMixingNumber BlurMixingNumber { get; internal set; } = BlurMixingNumber.Three;

    /// <summary>
    /// 静态模糊渲染目标，用于存储模糊后的画面
    /// </summary>
    public static RenderTarget2D BlurRenderTarget { get; private set; }

    public static RenderTarget2D UserInterfaceRenderTarget { get; private set; }

    public override void Unload()
    {
        Main.RunOnMainThread(() => BlurRenderTarget?.Dispose());
    }

    public override void Load()
    {
        On_Main.DrawPlayerChatBubbles += On_Main_DrawPlayerChatBubbles;

        On_Main.DrawInterface += On_Main_DrawInterface;
    }

    private void On_Main_DrawPlayerChatBubbles(On_Main.orig_DrawPlayerChatBubbles orig, Main self)
    {
        if (BlurAvailable)
        {
            WatchRenderTarget();

            if (SingleBlur)
            {
                var batch = Main.spriteBatch;
                var device = Main.graphics.GraphicsDevice;

                OriginalRenderTargetBindings = device.GetRenderTargets();
                device.SetRenderTarget(UserInterfaceRenderTarget);

                batch.End();

                batch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.Identity);
                batch.Draw(Main.screenTarget, Vector2.Zero, null, Color.White);
            }
        }

        orig(self);
    }

    private void On_Main_DrawInterface(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
    {
        if (!BlurAvailable)
        {
            orig(self, gameTime);
            return;
        }

        var batch = Main.spriteBatch;

        if (!SingleBlur)
        {
            MakeKawaseBlur(Main.screenTarget);
            orig(self, gameTime);
            return;
        }

        orig(self, gameTime);

        var device = Main.graphics.GraphicsDevice;
        OriginalRenderTargetBindings.RestoreRenderTargets(device);

        batch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.Identity);
        batch.Draw(UserInterfaceRenderTarget, Vector2.Zero, null, Color.White);
        batch.End();
    }

    private static void WatchRenderTarget()
    {
        var originalWidth = Main.screenTarget.Width;
        var originalHeight = Main.screenTarget.Height;

        var width = (int)Math.Ceiling(originalWidth / BlurZoomMultiplierDenominator);
        var height = (int)Math.Ceiling(originalHeight / BlurZoomMultiplierDenominator);

        var device = Main.graphics.GraphicsDevice;

        if (BlurRenderTarget is null || BlurRenderTarget.Width != width || BlurRenderTarget.Height != height)
        {
            BlurRenderTarget?.Dispose();
            BlurRenderTarget = new RenderTarget2D(
                Main.graphics.GraphicsDevice, width, height, false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);
        }

        if (!SingleBlur) return;

        if (UserInterfaceRenderTarget is null || UserInterfaceRenderTarget.Width != originalWidth || UserInterfaceRenderTarget.Height != originalHeight)
        {
            UserInterfaceRenderTarget?.Dispose();
            UserInterfaceRenderTarget = new RenderTarget2D
                (Main.graphics.GraphicsDevice, originalWidth, originalHeight, false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);
        }
    }

    private static RenderTargetBinding[] OriginalRenderTargetBindings { get; set; }

    public static void MakeKawaseBlur()
    {
        BlurHelper.KawaseBlur(UserInterfaceRenderTarget, BlurRenderTarget, BlurIterationCount, BlurIterationOffsetMultiplier, BlurZoomMultiplierDenominator, BlurMixingNumber);
    }

    public static void MakeKawaseBlur(RenderTarget2D renderTarget)
    {
        BlurHelper.KawaseBlur(renderTarget, BlurRenderTarget, BlurIterationCount, BlurIterationOffsetMultiplier, BlurZoomMultiplierDenominator, BlurMixingNumber);
    }

    ///// <summary>
    ///// 模糊实现和设置 RenderTarget
    ///// </summary>
    //public class BlurMakeLayer() : GameInterfaceLayer("BlurMakeLayer", InterfaceScaleType.UI)
    //{
    //    /// <summary>
    //    /// 绘制方法，实现模糊效果
    //    /// </summary>
    //    /// <returns>总是返回 true，表示绘制成功</returns>
    //    public override bool DrawSelf()
    //    {
    //        if (!BlurAvailable) return true;

    //        var batch = Main.spriteBatch;

    //        if (!SingleBlur)
    //        {
    //            batch.End();
    //            KawaseBlur(Main.screenTarget);
    //            batch.Begin();
    //        }

    //        return true;
    //    }
    //}

    ///// <summary>
    ///// 恢复 RenderTarget，并渲染当前画面
    ///// </summary>
    //public class RestoreRenderTargetLayer() : GameInterfaceLayer("RestoreRenderTargetLayer", InterfaceScaleType.UI)
    //{
    //    public override bool DrawSelf()
    //    {
    //        var device = Main.graphics.GraphicsDevice;
    //        OriginalRenderTargetBindings.RestoreRenderTargets(device);

    //        var batch = Main.spriteBatch;
    //        batch.End();

    //        batch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.Identity);
    //        batch.Draw(UserInterfaceRenderTarget, Vector2.Zero, null, Color.White);

    //        return true;
    //    }
    //}
}