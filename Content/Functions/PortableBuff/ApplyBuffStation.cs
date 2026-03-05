namespace ImproveGame.Content.Functions.PortableBuff;

/// <summary>
/// 把“随身增益站”状态写回到 <see cref="Main.SceneMetrics"/>。
/// </summary>
/// <remarks>
/// 这里不直接给玩家加 Buff，而是“伪造附近有对应设施”的场景数据。
/// <br/>来源是 <c>InfiniteBuffPlayer.PostUpdateBuffs</c> 每帧写入的 <c>HasXxx</c> 标记。
/// <br/>其中蜡烛走“计数”逻辑，其他设施走“布尔存在”逻辑。
/// </remarks>
public class ApplyBuffStation : ModSystem
{
    /// <summary>
    /// 本帧是否拥有篝火效果。
    /// </summary>
    public static bool HasCampfire { get; set; }

    /// <summary>
    /// 本帧是否拥有心灯效果。
    /// </summary>
    public static bool HasHeartLantern { get; set; }

    /// <summary>
    /// 本帧是否拥有向日葵效果。
    /// </summary>
    public static bool HasSunflower { get; set; }

    /// <summary>
    /// 本帧是否拥有花园侏儒效果。
    /// </summary>
    public static bool HasGardenGnome { get; set; }

    /// <summary>
    /// 本帧是否拥有瓶中星效果。
    /// </summary>
    public static bool HasStarInBottle { get; set; }

    /// <summary>
    /// 本帧是否拥有水蜡烛效果。
    /// </summary>
    public static bool HasWaterCandle { get; set; }

    /// <summary>
    /// 本帧是否拥有和平蜡烛效果。
    /// </summary>
    public static bool HasPeaceCandle { get; set; }

    /// <summary>
    /// 本帧是否拥有暗影蜡烛效果。
    /// </summary>
    public static bool HasShadowCandle { get; set; }

    /// <summary>
    /// 清空本帧缓存的“随身增益站”标记。
    /// </summary>
    /// <remarks>
    /// 一帧内可能多次进入原版回调，所以每帧都要先清空，避免把上一帧状态带进来。
    /// </remarks>
    public static void Reset()
    {
        HasCampfire = false;
        HasHeartLantern = false;
        HasSunflower = false;
        HasGardenGnome = false;
        HasStarInBottle = false;
        HasWaterCandle = false;
        HasPeaceCandle = false;
        HasShadowCandle = false;
    }

    /// <summary>
    /// 回填蜡烛类场景计数，让原版按“附近有该蜡烛”处理。
    /// </summary>
    /// <param name="tileCounts">原版图格统计。本实现只借这个时机回填计数。</param>
    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        // 蜡烛是“计数”逻辑，需递增对应计数器。
        if (HasWaterCandle)
            Main.SceneMetrics.WaterCandleCount++;
        if (HasPeaceCandle)
            Main.SceneMetrics.PeaceCandleCount++;
        if (HasShadowCandle)
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
        if (Main.netMode == NetmodeID.Server)
            return;

        // 这些效果是“是否存在”的布尔逻辑，直接置 true 即可。
        if (HasCampfire)
            Main.SceneMetrics.HasCampfire = true;
        if (HasHeartLantern)
            Main.SceneMetrics.HasHeartLantern = true;
        if (HasSunflower)
            Main.SceneMetrics.HasSunflower = true;
        if (HasGardenGnome)
            Main.SceneMetrics.HasGardenGnome = true;
        if (HasStarInBottle)
            Main.SceneMetrics.HasStarInBottle = true;
    }
}
