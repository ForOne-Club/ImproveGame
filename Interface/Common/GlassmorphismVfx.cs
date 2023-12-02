using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using Terraria.Graphics.Effects;

namespace ImproveGame.Interface.Common;

[Autoload(Side = ModSide.Client)]
public class GlassmorphismVfx : ModSystem
{
    // RenderTargetPool疑似有问题，会一直创建新RT2D而不是从池里取，所以暂时不用
    // private RenderTargetPool _targetPool;

    /// <summary>
    /// 高斯模糊处理后的原版画面RT2D
    /// </summary>
    private RenderTarget2D _blurredTarget;

    /// <summary>
    /// 带有UI画面的RT2D
    /// </summary>
    private RenderTarget2D _uiTarget;

    /// <summary>
    /// 绘制高斯模糊时帮忙的RT2D
    /// </summary>
    private RenderTarget2D _helperTarget;

    /// <summary>
    /// 用于类云母效果的每个UI一人一个的RT2D
    /// </summary>
    internal static RenderTarget2D[] GlassCovers;

    public override void Load()
    {
        // _targetPool = new RenderTargetPool();
        Filters.Scene.OnPostDraw += RenderGlassmorphismVfx;
        Main.OnRenderTargetsInitialized += InitializeTarget;
        Main.OnRenderTargetsReleased += ReleaseTarget;

        Main.RunOnMainThread(() =>
        {
            _blurredTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y);
            _uiTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y);
            _helperTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y);
        });
    }

    public override void Unload()
    {
        // Main.RunOnMainThread(_targetPool.Dispose);
        Filters.Scene.OnPostDraw -= RenderGlassmorphismVfx;
        Main.OnRenderTargetsInitialized -= InitializeTarget;
        Main.OnRenderTargetsReleased -= ReleaseTarget;

        Main.RunOnMainThread(() =>
        {
            _blurredTarget?.Dispose();
            _uiTarget?.Dispose();
            _helperTarget?.Dispose();
            if (GlassCovers is not null)
                foreach (var t in GlassCovers)
                    t?.Dispose();

            _blurredTarget = null;
            _uiTarget = null;
            _helperTarget = null;
            GlassCovers = null;
        });
    }

    public override void PostSetupContent()
    {
        Main.RunOnMainThread(() =>
        {
            GlassCovers = new RenderTarget2D[EventTrigger.LayerCount + 1];
            for (var i = 0; i < GlassCovers.Length; i++)
                GlassCovers[i] = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y);
        });
    }

    private void InitializeTarget(int width, int height)
    {
        _blurredTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
        _uiTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
        _helperTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
        for (var i = 0; i < GlassCovers.Length; i++)
            GlassCovers[i] = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
    }

    private void ReleaseTarget()
    {
        _blurredTarget?.Dispose();
        _uiTarget?.Dispose();
        _helperTarget?.Dispose();
        foreach (var t in GlassCovers)
            t?.Dispose();
    }

    private void RenderGlassmorphismVfx()
    {
        if (Main.gameMenu || !GlassVfxEnabled || Main.InGameUI.IsVisible || Main.ingameOptionsWindow)
            return;

        // // 对原版游戏画面高斯模糊处理的 target
        // if (!_targetPool.TryBorrow(Main.ScreenSize.ToVector2(), out var blurredTarget))
        // {
        //     return;
        // }
        //
        // // 储存 UI 绘制内容的 target
        // if (!_targetPool.TryBorrow(Main.ScreenSize.ToVector2(), out var uiTarget))
        // {
        //     _targetPool.TryReturn(blurredTarget);
        //     return;
        // }

        var device = Main.instance.GraphicsDevice;
        var batch = Main.spriteBatch;

        // “保存”原来的Rt2d
        device.SetRenderTarget(Main.screenTargetSwap);
        device.Clear(Color.Black);
        batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        batch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
        batch.End();

        // 高斯模糊处理
        // 先把 Main.screenTarget 绘制到 blurredTarget
        device.SetRenderTarget(_blurredTarget);
        batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        batch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
        batch.End();

        // 再对 blurredTarget 调用 ApplyGaussBlur
        ApplyGaussBlur(_blurredTarget);

        if (GlassVfxType is GlassType.MicaLike)
        {
            EventTrigger.MakeGlasses(ref GlassCovers, _blurredTarget, _uiTarget);
            device.SetRenderTarget(null);
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            batch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            batch.End();
            return;
        }

        // 在 uiTarget 上绘制 UI
        device.SetRenderTarget(_uiTarget);
        device.Clear(Color.Transparent);
        batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
        SDFRectangle.DontDrawShadow = true;
        EventTrigger.DrawAll();
        SDFRectangle.DontDrawShadow = false;
        batch.End();

        // 在 Main.screenTarget 上绘制最终结果
        var shader = ModAsset.Mask.Value;
        device.SetRenderTarget(Main.screenTarget);
        batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        shader.CurrentTechnique.Passes["Mask"].Apply();
        device.Textures[1] = _blurredTarget;
        device.Textures[2] = _uiTarget;
        batch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        batch.End();

        // 最后绘制到游戏显示的实际画布上
        device.SetRenderTarget(null);
        batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        batch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
        batch.End();

        // 复原
        device.Textures[0] = null;
        device.Textures[1] = null;
        device.Textures[2] = null;
        // _targetPool.TryReturn(blurredTarget);
        // _targetPool.TryReturn(uiTarget);
    }

    public void ApplyGaussBlur(RenderTarget2D target)
    {
        // if (!_targetPool.TryBorrow(Main.ScreenSize.ToVector2(), out var helperTarget))
        //     return;

        var shader = ModAsset.GaussBlur.Value;
        var device = Main.graphics.GraphicsDevice;
        var batch = Main.spriteBatch;

        shader.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2());
        shader.Parameters["uIntensity"].SetValue(UIColor.AcrylicIntensity);

        int times = GlassVfxType is GlassType.MicaLike ? 6 : 4;
        for (int i = 1; i <= times; i++)
        {
            shader.Parameters["uRange"].SetValue(1.2f * i);

            device.SetRenderTarget(_helperTarget);
            device.Clear(Color.Transparent);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            shader.CurrentTechnique.Passes["BlurX"].Apply();
            batch.Draw(target, Vector2.Zero, Color.White);
            batch.End();

            device.SetRenderTarget(target);
            device.Clear(Color.Transparent);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            shader.CurrentTechnique.Passes["BlurY"].Apply();
            batch.Draw(_helperTarget, Vector2.Zero, Color.White);
            batch.End();
        }

        // _targetPool.TryReturn(helperTarget);
    }
}