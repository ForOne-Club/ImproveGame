namespace ImproveGame.Content.Functions.PortableBuff;

public class ApplyBuffStation : ModSystem
{
    public static bool HasCampfire { get; set; }
    public static bool HasHeartLantern { get; set; }
    public static bool HasSunflower { get; set; }
    public static bool HasGardenGnome { get; set; }
    public static bool HasStarInBottle { get; set; }
    public static bool HasWaterCandle { get; set; }
    public static bool HasPeaceCandle { get; set; }
    public static bool HasShadowCandle { get; set; }

    // 在这里再重设各种bool，因为下面的方法可能一帧更新多次
    public static void Reset()
    {
        HasWaterCandle = false;
        HasPeaceCandle = false;
        HasShadowCandle = false;
        HasCampfire = false;
        HasHeartLantern = false;
        HasSunflower = false;
        HasGardenGnome = false;
        HasStarInBottle = false;
    }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        // 蜡烛
        if (HasWaterCandle)
            Main.SceneMetrics.WaterCandleCount++;
        if (HasPeaceCandle)
            Main.SceneMetrics.PeaceCandleCount++;
        if (HasShadowCandle)
            Main.SceneMetrics.ShadowCandleCount++;
    }

    // 这个一般来说只会在客户端执行
    public override void ResetNearbyTileEffects()
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        // 原版就没拿Buff判断篝火心灯，所以这里得专门判断
        if (HasCampfire)
            Main.SceneMetrics.HasCampfire = true;
        if (HasHeartLantern)
            Main.SceneMetrics.HasHeartLantern = true;
        // 顺便带上其他的好了
        if (HasSunflower)
            Main.SceneMetrics.HasSunflower = true;
        if (HasGardenGnome)
            Main.SceneMetrics.HasGardenGnome = true;
        if (HasStarInBottle)
            Main.SceneMetrics.HasStarInBottle = true;
    }
}