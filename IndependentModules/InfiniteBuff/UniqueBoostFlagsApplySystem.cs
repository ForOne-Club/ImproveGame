namespace ImproveGame.IndependentModules.InfiniteBuff;

/// <summary>
/// 把“随身增益站”状态写回到 <see cref="Main.SceneMetrics"/>。
/// </summary>
public class UniqueBoostFlagsApplySystem : ModSystem
{
    /// <summary>
    /// 回填蜡烛类场景计数，让原版按“附近有该蜡烛”处理。
    /// </summary>
    /// <param name="tileCounts">原版图格统计。本实现只借这个时机回填计数。</param>
    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        // 蜡烛是 “计数” 逻辑，需递增对应计数器。
        if (UniqueBoostFlags.HasWaterCandle)
            Main.SceneMetrics.WaterCandleCount++;

        if (UniqueBoostFlags.HasPeaceCandle)
            Main.SceneMetrics.PeaceCandleCount++;

        if (UniqueBoostFlags.HasShadowCandle)
            Main.SceneMetrics.ShadowCandleCount++;
    }

    /// <summary>
    /// 回填布尔类场景标记（篝火、心灯等）。
    /// </summary>
    /// <remarks>
    /// 服务端不会使用本地场景判定，所以直接返回。
    /// </remarks>
    public override void ResetNearbyTileEffects()
    {
        if (Main.netMode == NetmodeID.Server) return;

        // 这些效果是 “是否存在” 的布尔逻辑，直接置 true 即可。
        if (UniqueBoostFlags.HasCampfire)
            Main.SceneMetrics.HasCampfire = true;

        if (UniqueBoostFlags.HasHeartLantern)
            Main.SceneMetrics.HasHeartLantern = true;

        if (UniqueBoostFlags.HasSunflower)
            Main.SceneMetrics.HasSunflower = true;

        if (UniqueBoostFlags.HasGardenGnome)
            Main.SceneMetrics.HasGardenGnome = true;

        if (UniqueBoostFlags.HasStarInBottle)
            Main.SceneMetrics.HasStarInBottle = true;
    }
}