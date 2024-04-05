using System.Diagnostics;

namespace ImproveGame.Core;

public class CountRefreshRate : ModSystem
{
    public static Stopwatch RefreshRateStopwatch = new();
    
    /// <summary>
    /// 刷新率因子，可以乘在AnimationTimer的缓动上，来根据帧率实时调节动画速度，实现高帧率下的丝滑动画。
    /// 在绘制中调用AnimationTimer.Update()方法时传入此参数，即可实现高帧缓动。
    /// </summary>
    public static float CurrentRefreshRateFactor = 1f;

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        CurrentRefreshRateFactor = GetRefreshRateFactor(RefreshRateStopwatch);
        CurrentRefreshRateFactor = MathHelper.Clamp(CurrentRefreshRateFactor, 0.1f, 10f);
    }
}