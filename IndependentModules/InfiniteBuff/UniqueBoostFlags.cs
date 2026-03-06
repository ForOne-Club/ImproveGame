namespace ImproveGame.IndependentModules.InfiniteBuff;

public static class UniqueBoostFlags
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
    public static void ResetState()
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

    public static void UpdateStateByType(int type)
    {
        switch (type)
        {
            case BuffID.Campfire:
                HasCampfire = true;
                break;
            case BuffID.HeartLamp:
                HasHeartLantern = true;
                break;
            case BuffID.StarInBottle:
                HasStarInBottle = true;
                break;
            case BuffID.Sunflower:
                HasSunflower = true;
                break;
            case BuffID.WaterCandle:
                HasWaterCandle = true;
                break;
            case BuffID.PeaceCandle:
                HasPeaceCandle = true;
                break;
            case BuffID.ShadowCandle:
                HasShadowCandle = true;
                break;
        }
    }
}