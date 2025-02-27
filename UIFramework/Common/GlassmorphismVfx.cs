using ImproveGame.UI.ModernConfig;
using ImproveGame.UIFramework.Graphics2D;
using MonoMod.Cil;
using System.Reflection;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.WorldBuilding;
using static Terraria.Localization.NetworkText;

namespace ImproveGame.UIFramework.Common;

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
    internal static RenderTarget2D[] GlassCovers = [];

    public override void Load()
    {
        // _targetPool = new RenderTargetPool();
        Filters.Scene.OnPostDraw += () => { };//RenderGlassmorphismVfx
        Main.OnRenderTargetsInitialized += InitializeTarget;
        Main.OnRenderTargetsReleased += ReleaseTarget;

        Main.RunOnMainThread(() =>
        {
            _blurredTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y);
            _uiTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y);
            _helperTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y);
        });
        IL_Main.DoDraw += RenderGlassmorphismVfx_ILEmit;
    }

    private void RenderGlassmorphismVfx_ILEmit(ILContext il)
    {
        //这部分代码负责在主页面开启screenTarget捕获
        ILCursor cursor = new(il);
        //"Sepia"是饥荒世界的滤镜，这里世界生成的时候也会开启，这里用for查找到最后一个
        for (int n = 0; n < 5; n++)
            if (!cursor.TryGotoNext(i => i.MatchLdstr("Sepia")))
                return;
        //神人螺线直接Index+=14了，这里是导航到or指令前面
        //具体说来是drawToScreen || netMode == 2 || flag
        //这里只要有一个成立就不会开启screenTarget
        //其中flag表示  不启用饥荒滤镜
        if (!cursor.TryGotoNext(i => i.MatchOr()))
            return;
        cursor.EmitDelegate(() =>
        {
            return !MyUtils.GlassVfxEnabled;//
        });
        cursor.EmitAnd();
        //↑这里我加入了一个 *不启用设置预览的Render绘制*然后取与
        //也就是说如果既不要饥荒滤镜也不要毛玻璃就不开screenTarget捕获，很合理



        //找到DrawMenu之前，我们要在这里先进行毛玻璃的生成
        if (!cursor.TryGotoNext(i => i.MatchCallOrCallvirt(typeof(Main).GetMethod(nameof(Main.DrawMenu), BindingFlags.NonPublic | BindingFlags.Instance))))
            return;
        cursor.Index -= 2;
        cursor.EmitLdloc(11);//这个是一个bool值，表示当前是否开启了屏幕捕获
        cursor.EmitDelegate<Action<bool>>(flag =>
        {
            if (flag)
                RenderGlassmorphismVfx();
        });
        //这里用EmitCall和brfalse还有打标签之类的应该也是可以的，但是比较麻烦，干脆EmitDelegate了


        if (!cursor.TryGotoNext(i => i.MatchCallOrCallvirt(typeof(Main).GetMethod(nameof(Main.DrawInterface), BindingFlags.NonPublic | BindingFlags.Instance))))
            return;

        if (!cursor.TryGotoPrev(i => i.MatchCallOrCallvirt(typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.End), BindingFlags.Public | BindingFlags.Instance, []))))
            return;
        //导航到DrawInterface前，我们要在那里构造毛玻璃
        //不用原来的OnPostDraw是因为我的绘制预览把那个EndCapture延后了
        cursor.Index++;

        cursor.EmitLdloc(11);
        cursor.EmitDelegate<Action<bool>>(flag =>
        {
            if (flag)
                RenderGlassmorphismVfx();
        });
    }

    public override void Unload()
    {
        // Main.RunOnMainThread(_targetPool.Dispose);
        //Filters.Scene.OnPostDraw -= RenderGlassmorphismVfx;
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
            ModernConfigUI.Glass?.Dispose();

            _blurredTarget = null;
            _uiTarget = null;
            _helperTarget = null;
            GlassCovers = null;
            ModernConfigUI.Glass = null;
        });
    }

    public override void PostSetupContent()
    {
        Main.RunOnMainThread(() =>
        {
            RenderInitialized = true;
            GlassCovers = new RenderTarget2D[EventTriggerManager.LayerCount + 1];
            for (var i = 0; i < GlassCovers.Length; i++)
                GlassCovers[i] = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y);
            ModernConfigUI.Glass = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y);
        });
    }
    public static bool RenderInitialized;

    public static void InitializeTarget(int width, int height)
    {
        var instance = ModContent.GetInstance<GlassmorphismVfx>();
        instance._blurredTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
        instance._uiTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
        instance._helperTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
        for (var i = 0; i < GlassCovers.Length; i++)
            GlassCovers[i] = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
        ModernConfigUI.Glass = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
    }

    private void ReleaseTarget()
    {
        _blurredTarget?.Dispose();
        _uiTarget?.Dispose();
        _helperTarget?.Dispose();
        foreach (var t in GlassCovers)
            t?.Dispose();
        ModernConfigUI.Glass?.Dispose();
    }

    private void RenderGlassmorphismVfx()
    {

        if (!GlassVfxAvailable || !RenderInitialized)//Main.gameMenu || 
            return;
        var device = Main.instance.GraphicsDevice;
        var batch = Main.spriteBatch;
        device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

        var needReBegin = batch.beginCalled;
        var captureScreen = device.GetRenderTargets().Length > 0;
        if (needReBegin)
            batch.End();

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
        if (!Main.gameMenu)
            PlayerInput.SetZoom_UI();
        if (!Main.InGameUI.IsVisible && !Main.ingameOptionsWindow)
            EventTriggerManager.MakeGlasses(ref GlassCovers, _blurredTarget, _uiTarget);

        ModernConfigUI.MakeGlass(_blurredTarget, _uiTarget);
        if (!Main.gameMenu)
            PlayerInput.SetZoom_World();

        if (captureScreen)
        {
            device.SetRenderTarget(Main.screenTarget);
            device.Clear(Color.Black);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            batch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            batch.End();
        }
        else
            device.SetRenderTarget(null);

        if (needReBegin)
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
    }

    public void ApplyGaussBlur(RenderTarget2D target)
    {
        // if (!_targetPool.TryBorrow(Main.ScreenSize.ToVector2(), out var helperTarget))
        //     return;

        var shader = ModAsset.GaussBlur.Value;
        var device = Main.graphics.GraphicsDevice;
        var batch = Main.spriteBatch;

        shader.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2());
        shader.Parameters["uIntensity"].SetValue(UIStyle.AcrylicIntensity);

        int times = GlassVfxEnabled ? 6 : 4;
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